﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCType {
    // peasants:
    Vagabond,
    Peasant,
    Archer,
    Worker,
    Swordman,

    // animals:
    Slime
}

public class PeasantAI : AIBase {
    internal enum PeasantAIState {
        WaitingForCoin,
        WaitingForWeapon,

        // archer states
        LookingForAnimal,
        ChasingAnimal,

        // worker states
        LookingForWorkerJob,
        AssignedConstructionJob,
        AssignedRepairJob,
        AssignedLumberingJob,

        WaitingForTrophyCoin,

        Squad
    }

    [SerializeField] public NPCType type;
    [SerializeField] private PeasantAIState state;

    private int trophyCoinsAmount = 0;

    /// timings
    private float playerLookupInterval = 0.5f;
    private float playerLookupElapsed = 0f;

    private float lookupInterval = 0.1f;
    private float lookupElapsed = 0f;

    private float attackElapsed = 2f;

    private Animal targetAnimal = null;
    private Building targetBuilding = null;
    private Tree targetTree = null;

    private Peasant peasant;

    private Squad squad;

    /// Public -- 

    public void onSquadUpdate(SquadUpdate update) {
        Debug.Log("onSquadUpdate: " + update);
        switch (update.type) {
            case SquadUpdateType.UnitAdded:
                if (update.unit == peasant) {
                    squad = update.squad;
                    state = PeasantAIState.Squad;
                    squadModeTask(squad.mode);
                }
                break;
            case SquadUpdateType.UnitRemoved:
                if (update.unit == peasant) {
                    squad = null;
                    state = PeasantAIState.LookingForAnimal; // this needs to be a special state for when squad is disbanded
                }
                break;
            case SquadUpdateType.ModeUpdate:
                if (currentTask.cancellable) {
                    currentTask.cancel(reason: "squad mode update");
                    squadModeTask(squad.mode);
                }
                break;
            case SquadUpdateType.LeaderUpdate:
                // squad assigned new leader
                break;
        }
    }

    /// Protected --

    protected override void Awake() {
        base.Awake();
        peasant = GetComponent<Peasant>();
    }

    protected override void Start() {
        base.Start();
        issueTask(AITask.typeUpdateTask(type));
    }

    protected override void Update() {
        attackElapsed += Time.deltaTime;
        dropTrophyCoinsForPlayerUpdate();
        base.Update();
    }

    protected override void updateStateMachine() {
        if (squad != null) {
            // squadStateMachine();
        } else {
            switch (type) {
                case NPCType.Vagabond:
                    vagabondStateMachine();
                    break;
                case NPCType.Peasant:
                    peasantStateMachine();
                    break;
                case NPCType.Archer:
                    archerStateMachine();
                    break;
                case NPCType.Worker:
                    workerStateMachine();
                    break;
            }
        }
    }

    /// Private -- 

    /// State machines

    private void squadModeTask(SquadMode mode) {
        switch (mode) {
            case SquadMode.Forming:
                issueTask(AITask.squadFollowTask(squad));
                break;
            case SquadMode.Idle:
                issueTask(AITask.squadIdleTask(squad));
                break;
            case SquadMode.Enroute:
                issueTask(AITask.squadFollowTask(squad));
                break;
            default:
                Debug.Log("unkwnown squad mode: " + mode);
                break;
        }
    }

    private void vagabondStateMachine() {
        switch (state) {
            case PeasantAIState.WaitingForCoin:
                waitForCoinUpdate();
                break;
        }
    }

    private void peasantStateMachine() {
        switch (state) {
            case PeasantAIState.WaitingForWeapon:
                waitForWeaponUpdate();
                break;

        }
    }

    private void archerStateMachine() {
        switch (state) {
            case PeasantAIState.LookingForAnimal:
                lookForAnimalUpdate();
                break;
            case PeasantAIState.ChasingAnimal:
                chaseAnimalUpdate();
                break;
            case PeasantAIState.WaitingForTrophyCoin:
                waitForTrophyUpdate();
                break;
        }
    }

    private void workerStateMachine() {
        switch (state) {
            case PeasantAIState.LookingForWorkerJob:
                lookForWorkerJobUpdate();
                break;
            case PeasantAIState.AssignedConstructionJob:
                constructionJobUpdate();
                break;
            case PeasantAIState.AssignedLumberingJob:
                lumberingJobUpdate();
                break;
            case PeasantAIState.WaitingForTrophyCoin:
                waitForTrophyUpdate();
                break;
        }
    }

    /// Updates

    private void waitForCoinUpdate() {
        idleRoamUpdate();

        lookupElapsed += Time.deltaTime;
        if (lookupElapsed > lookupInterval) {
            lookupElapsed = 0;
            float radius = Constants.instance.PEASANT_COIN_LOOKUP_RADIUS;
            Coin[] coins = coinController.lookForCoins(transform.position, radius, CoinDropType.ByPlayer);
            if (coins.Length > 0) {
                coinController.reserveCoinForPickup(coins[0], gameObject);
                var task = AITask.pickupCoinTask(coins[0]);
                task.onComplete = () => {
                    if (task.success) {
                        becomePeasant();
                    }
                };
                issueTask(task);
            }
        }
    }

    private void waitForWeaponUpdate() {
        idleRoamUpdate();

        lookupElapsed += Time.deltaTime;
        if (lookupElapsed > lookupInterval) {
            lookupElapsed = 0;

            Weapon weapon = weaponController.lookForWeapon(transform.position, 1000f);

            if (weapon != null) {
                weaponController.reserveWeaponForPickup(weapon, gameObject);
                var weaponType = weapon.type;
                var task = AITask.pickupWeaponTask(weapon);
                task.onComplete = () => {
                    if (task.success) {
                        switch (weaponType) {
                            case WeaponType.Bow:
                                becomeArcher();
                                return;
                            case WeaponType.Hammer:
                                becomeWorker();
                                return;
                            default:
                                throw new System.Exception($"Peasant picked up unknown weapon type: {weaponType}");
                        }
                    }
                };
                issueTask(task);
            }
        }
    }

    private void lookForWorkerJobUpdate() {
        idleRoamUpdate();

        lookupElapsed += Time.deltaTime;
        if (lookupElapsed > lookupInterval) {
            lookupElapsed = 0;

            targetBuilding = finder.closestRepairJob(radius: Constants.instance.PEASANT_BUILDING_LOOKUP_RADIUS);
            if (targetBuilding == null) {
                targetBuilding = finder.closestConstructionJob(radius: Constants.instance.PEASANT_BUILDING_LOOKUP_RADIUS);
                if (targetBuilding == null) {
                    targetTree = finder.closestLumberingJob(radius: Constants.instance.PEASANT_TREE_LOOKUP_RADIUS);
                    if (targetTree != null) {
                        state = PeasantAIState.AssignedLumberingJob;
                    }
                } else {
                    state = PeasantAIState.AssignedConstructionJob;
                }
            } else {
                state = PeasantAIState.AssignedRepairJob;
            }
        }
    }

    private void lookForAnimalUpdate() {
        idleRoamUpdate();

        lookupElapsed += Time.deltaTime;
        if (lookupElapsed > lookupInterval) {
            lookupElapsed = 0;

            targetAnimal = finder.closestAliveAnimal(radius: Constants.instance.PEASANT_ANIMAL_LOOKUP_RADIUS);

            if (targetAnimal != null) {
                state = PeasantAIState.ChasingAnimal;
            }
        }
    }

    private void constructionJobUpdate() {
        Building building = targetBuilding;
        if (building == null || building.state != BuildingState.AwaitingConstruction) {
            building = null;
            state = PeasantAIState.LookingForWorkerJob;
            return;
        }

        if (!peasant.feetCollidesZone(building.collisionZoneGameObject)) {
            var moveTask = AITask.moveTask(building.transform.position);
            moveTask.onComplete = () => {
                if (building != null) {
                    if (peasant.feetCollidesZone(building.collisionZoneGameObject)) {
                        var constructionTask = AITask.constructionTask(building);
                        constructionTask.onComplete = () => {
                            state = PeasantAIState.LookingForWorkerJob;
                        };
                        issueTask(constructionTask);
                    }
                }
            };
            issueTask(moveTask);
        }
    }

    private void lumberingJobUpdate() {
        Tree tree = targetTree;
        if (tree == null || tree.state != TreeState.MarkedForChop) {
            tree = null;
            state = PeasantAIState.LookingForWorkerJob;
            return;
        }

        if (!peasant.feetCollidesZone(tree.collisionZoneGameObject)) {
            var moveTask = AITask.moveTask(tree.transform.position);
            moveTask.onComplete = () => {

            };
            issueTask(moveTask);
        } else {
            var chopTask = AITask.chopDownTask(tree);
            chopTask.onComplete = () => {
                if (chopTask.success) {
                    state = PeasantAIState.WaitingForTrophyCoin;
                } else {
                    state = PeasantAIState.AssignedLumberingJob;
                }
            };
            issueTask(chopTask);

            state = PeasantAIState.LookingForWorkerJob;
        }
    }

    private void chaseAnimalUpdate() {
        Animal animal = targetAnimal;
        if (animal == null || !animal.isAlive) {
            animal = null;
            state = PeasantAIState.LookingForAnimal;
            return;
        }

        float attackRange = Constants.instance.PEASANT_ANIMAL_ATTACK_RANGE;
        float d = Vector2.Distance(animal.transform.position, transform.position);
        if (d > attackRange) {
            Vector3 approach = Vector3.MoveTowards(transform.position, animal.transform.position, d - attackRange + 1f);
            float r = Constants.instance.PEASANT_APPROACH_DEVIATION;
            approach.x += Random.Range(-r, r);
            approach.y += Random.Range(-r / 2, r / 2);
            issueTask(AITask.moveTask(approach));
        } else {
            if (attackElapsed > Constants.instance.PEASANT_ATTACK_INTERVAL) {
                attackElapsed = 0;
                AITask attackTask = AITask.killTask(animal.gameObject);
                attackTask.onComplete = () => {
                    if (attackTask.success) {
                        state = PeasantAIState.WaitingForTrophyCoin;
                    }
                };
                issueTask(attackTask);
            }
        }
    } 


    private void waitForTrophyUpdate() {
        lookupElapsed += Time.deltaTime;
        if (lookupElapsed > lookupInterval) {
            lookupElapsed = 0;
            float searchRange = Constants.instance.PEASANT_ANIMAL_ATTACK_RANGE + 1f;
            Coin[] coins = coinController.lookForCoins(transform.position, searchRange, CoinDropType.ByAnimal);
            if (coins.Length > 0) {
                for (int i = 0; i < coins.Length; i++) {
                    Coin coin = coins[i];
                    coinController.reserveCoinForPickup(coin, gameObject);
                    var task = AITask.pickupCoinTask(coin);
                    bool isLastCoin = i == coins.Length - 1;
                    task.onComplete = () => {
                        if (task.success) {
                            trophyCoinsAmount++;
                        }
                        if (isLastCoin) {
                            returnToIdleState();
                        }
                    };
                    enqueueTask(task);
                }
            }
        }
    }

    private void returnToIdleState() {
        switch (type) {
            case NPCType.Archer:
                state = PeasantAIState.LookingForAnimal;
                break;
            case NPCType.Worker:
                state = PeasantAIState.LookingForWorkerJob;
                break;
            default:
                throw new System.Exception($"no idle state for peasant type {type}");
        }
    }

    private void dropTrophyCoinsForPlayerUpdate() {
        playerLookupElapsed += Time.deltaTime;
        if (playerLookupElapsed >= playerLookupInterval) {
            playerLookupElapsed = 0;
            if (trophyCoinsAmount > 0) {
                float range = Constants.instance.PEASANT_PLAYER_DELIVER_PROXIMITY;
                Vector3 playerPosition = finder.player().transform.position;
                if (Vector2.Distance(playerPosition, transform.position) < range) {
                    AITask task = AITask.dropCoinsTask(trophyCoinsAmount);
                    task.onComplete = () => {
                        if (task.success) {
                            trophyCoinsAmount = 0;
                        }
                    };
                    issueTask(task);
                }
            }
        }
    }

    /// Internal methods

    private void becomePeasant() {
        type = NPCType.Peasant;
        state = PeasantAIState.WaitingForWeapon;
        issueTask(AITask.typeUpdateTask(type));
    }

    private void becomeArcher() {
        type = NPCType.Archer;
        state = PeasantAIState.LookingForAnimal;
        issueTask(AITask.typeUpdateTask(type));
    }

    private void becomeWorker() {
        type = NPCType.Worker;
        state = PeasantAIState.LookingForWorkerJob;
        issueTask(AITask.typeUpdateTask(type));
    }
}