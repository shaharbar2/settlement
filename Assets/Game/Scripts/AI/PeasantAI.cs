using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCType {
    Homeless,
    Peasant,
    Hunter,

    Slime
}

public class PeasantAI : AIBase {
    internal enum PeasantAIState {
        WaitingForCoin,
        WaitingForWeapon,
        LookingForAnimal,
        ChasingAnimal
    }

    [SerializeField] public NPCType type;
    [SerializeField] private PeasantAIState state;

    /// timings
    private float lookupInterval = 0.1f;
    private float lookupElapsed = 0f;

    private Animal targetAnimal = null;

    protected override void Start() {
        base.Start();
        issueTask(AITask.typeUpdateTask(type));
    }
    /// Protected --

    /// Protected --
    protected override void updateStateMachine() {
        switch (type) {
            case NPCType.Homeless:
                homelessStateMachine();
                break;
            case NPCType.Peasant:
                peasantStateMachine();
                break;
            case NPCType.Hunter:
                hunterStateMachine();
                break;
        }
    }

    /// Private -- 

    /// State machines

    private void homelessStateMachine() {
        switch (state) {
            case PeasantAIState.WaitingForCoin:
                waitForCoinUpdate();
                break;
        }
    }

    private void peasantStateMachine() {
        switch (state) {
            case PeasantAIState.WaitingForCoin:
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
        }
    }

    /// Updates

    private void waitForCoinUpdate() {
        idleRoamUpdate();

        lookupElapsed += Time.deltaTime;
        if (lookupElapsed > lookupInterval) {
            lookupElapsed = 0;

            Coin coin = coinController.lookForCoin(transform.position, 1000f);
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
                var task = AITask.pickupWeaponTask(weapon);
                task.onComplete = () => {
                    if (task.success) {
                        becomeHunter();
                    }
                };
                issueTask(task);
            }
        }
    }

    private void lookForAnimalUpdate() {
        // idleRoamUpdate();

        lookupElapsed += Time.deltaTime;
        if (lookupElapsed > lookupInterval) {
            lookupElapsed = 0;

            float searchRadius = Constants.instance.PEASANT_ANIMAL_LOOKUP_RADIUS;
            targetAnimal = findClosestAnimal(searchRadius);

            if (targetAnimal != null) {
                state = PeasantAIState.ChasingAnimal;
            }
        }
    }

    private void chaseAnimalUpdate() {
        Animal animal = targetAnimal;
        if (animal == null) {
            state = PeasantAIState.LookingForAnimal;
            return;
        }

        float attackRange = Constants.instance.PEASANT_ANIMAL_ATTACK_RANGE;
        float d = Vector2.Distance(animal.transform.position, transform.position);
        if (d > attackRange) {
            Vector3 approach = Vector3.MoveTowards(transform.position, animal.transform.position, d - attackRange +1f);
            issueTask(AITask.moveTask(approach));
        } else {
            issueTask(AITask.attackTask(animal.gameObject));
        }
    }

    /// Internal methods

    private void becomePeasant() {
        type = NPCType.Peasant;
        issueTask(AITask.typeUpdateTask(type));
    }

    private void becomeHunter() {
        type = NPCType.Hunter;
        issueTask(AITask.typeUpdateTask(type));
    }

    private Animal findClosestAnimal(float radius) {
        Animal[] allAnimals = FindObjectsOfType<Animal>();
        Animal closest = null;
        float closestRange = float.MaxValue;
        foreach (Animal animal in allAnimals) {
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