using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCType {
    // peasants:
    Vagabond,
    Peasant,
    Hunter,
    Worker,

    // animals:
    Slime
}

public class PeasantAI : AIBase {
    internal enum PeasantAIState {
        WaitingForCoin,
        WaitingForWeapon,

        // hunter states
        LookingForAnimal,
        ChasingAnimal,

        // worker states
        LookingForConstruction,
        LookingForRepairs,
        LookingForTrees,

        WaitingForTrophyCoin
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

    protected override void Start() {
        base.Start();
        issueTask(AITask.typeUpdateTask(type));
    }
    /// Protected --

    protected override void Update() {
        attackElapsed += Time.deltaTime;
        dropTrophyCoinsForPlayerUpdate();
        base.Update();
    }
    /// Protected --

    /// Protected --
    protected override void updateStateMachine() {
        switch (type) {
            case NPCType.Vagabond:
                vagabondStateMachine();
                break;
            case NPCType.Peasant:
                peasantStateMachine();
                break;
            case NPCType.Hunter:
                hunterStateMachine();
                break;
            case NPCType.Worker:
                workerStateMachine();
                break;
        }
    }

    /// Private -- 

    /// State machines

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

    private void hunterStateMachine() {
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
            case PeasantAIState.LookingForConstruction:
            case PeasantAIState.LookingForRepairs:
            case PeasantAIState.LookingForTrees:
                lookForConstructionUpdate();
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

            Coin coin = coinController.lookForCoin(transform.position, 1000f, CoinDropType.ByPlayer);
            if (coin != null) {
                coinController.reserveCoinForPickup(coin, gameObject);
                var task = AITask.pickupCoinTask(coin);
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
                                becomeHunter();
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

    private void lookForConstructionUpdate() {
        idleRoamUpdate();

        lookupElapsed += Time.deltaTime;
        if (lookupElapsed > lookupInterval) {
            lookupElapsed = 0;

            float searchRadius = Constants.instance.PEASANT_ANIMAL_LOOKUP_RADIUS;
            targetAnimal = findClosestAliveAnimal(searchRadius);

            if (targetAnimal != null) {
                state = PeasantAIState.ChasingAnimal;
            }
        }
    }

    private void lookForAnimalUpdate() {
        idleRoamUpdate();

        lookupElapsed += Time.deltaTime;
        if (lookupElapsed > lookupInterval) {
            lookupElapsed = 0;

            float searchRadius = Constants.instance.PEASANT_ANIMAL_LOOKUP_RADIUS;
            targetAnimal = findClosestAliveAnimal(searchRadius);

            if (targetAnimal != null) {
                state = PeasantAIState.ChasingAnimal;
            }
        }
    }

    private void chaseAnimalUpdate() {
        Animal animal = targetAnimal;
        if (animal == null || !animal.isAlive) {
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
            Coin coin = coinController.lookForCoin(transform.position, searchRange, CoinDropType.ByAnimal);
            if (coin != null) {
                coinController.reserveCoinForPickup(coin, gameObject);
                var task = AITask.pickupCoinTask(coin);
                task.onComplete = () => {
                    if (task.success) {
                        trophyCoinsAmount++;
                    }
                    state = PeasantAIState.LookingForAnimal;
                };
                issueTask(task);
            }
        }
    }

    private void dropTrophyCoinsForPlayerUpdate() {
        playerLookupElapsed += Time.deltaTime;
        if (playerLookupElapsed >= playerLookupInterval) {
            playerLookupElapsed = 0;
            if (trophyCoinsAmount > 0) {
                float range = Constants.instance.PEASANT_PLAYER_DELIVER_PROXIMITY;
                if (Vector2.Distance(player.transform.position, transform.position) < range) {
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

    private void becomeHunter() {
        type = NPCType.Hunter;
        state = PeasantAIState.LookingForAnimal & PeasantAIState.WaitingForCoin;
        issueTask(AITask.typeUpdateTask(type));
    }

    private void becomeWorker() {
        type = NPCType.Worker;
        state = PeasantAIState.LookingForConstruction;
        issueTask(AITask.typeUpdateTask(type));
    }

    private Animal findClosestAliveAnimal(float radius) {
        Animal[] allAnimals = FindObjectsOfType<Animal>();
        Animal closest = null;
        float closestRange = float.MaxValue;
        foreach (Animal animal in allAnimals) {
            if (!animal.isAlive)continue;
            float r = Vector2.Distance(animal.transform.position, transform.position);
            if (r < radius) {
                if (r < closestRange) {
                    closestRange = r;
                    closest = animal;
                }
            }
        }
        return closest;
    }
}