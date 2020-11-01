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
    private float lookForCoinInterval = 0.1f;
    private float lookForCoinElapsed = 0f;

    private float lookForWeaponInterval = 0.1f;
    private float lookForWeaponElapsed = 0f;

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

    }

    /// Updates

    private void waitForCoinUpdate() {
        idleRoamUpdate();

        lookForCoinElapsed += Time.deltaTime;
        if (lookForCoinElapsed > lookForCoinInterval) {
            lookForCoinElapsed = 0;

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

        lookForWeaponElapsed += Time.deltaTime;
        if (lookForWeaponElapsed > lookForWeaponInterval) {
            lookForWeaponElapsed = 0;

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

    }

    private void chaseAnimalUpdate() {

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
}