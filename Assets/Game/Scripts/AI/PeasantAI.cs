using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PeasantType {
    Homeless,
    Peasant,
    Hunter
}

public class PeasantAI : MonoBehaviour {
    public delegate void PeasantAIEvent(AITask command);
    public event PeasantAIEvent onTaskIssued;

    internal enum PeasantAIState {
        WaitingForCoin,
        WaitingForWeapon,
        LookingForAnimal,
        ChasingAnimal
    }

    [SerializeField] public PeasantType type;
    [SerializeField] private PeasantAIState state;

    private CoinController coinController;
    private AITask currentTask;

    /// timings
    private float roamInterval = 5f;
    private float roamElapsed = 0f;

    private float lookForCoinInterval = 0.1f;
    private float lookForCoinElapsed = 0f;

    private float lookForWeaponInterval = 0.1f;
    private float lookForWeaponElapsed = 0f;

    void Awake() {

    }

    void Start() {
        coinController = FindObjectOfType<CoinController>();
    }

    void Update() {
        if (currentTask != null) {
            if (currentTask.failed) {
                onTaskFailed(currentTask);
            } else if (currentTask.success) {
                onTaskFinished(currentTask);
            }
        } else {
            switch (type) {
                case PeasantType.Homeless:
                    homelessStateMachine();
                    break;
                case PeasantType.Peasant:
                    peasantStateMachine();
                    break;
                case PeasantType.Hunter:
                    hunterStateMachine();
                    break;
            }
        }
    }

    /// Public -- 

    public void setup() {

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

    /// Task handlers

    private void onTaskFailed(AITask task) {
        Debug.Log($"Peasant failed: {task}");
        currentTask = null;
        task.onComplete?.Invoke();
    }

    private void onTaskFinished(AITask task) {
        Debug.Log($"Peasant finished: {task}");
        currentTask = null;
        task.onComplete?.Invoke();
    }

    private void issueTask(AITask task) {
        Debug.Log($"Issued peasant: {task}");
        currentTask = task;
        onTaskIssued?.Invoke(task);
    }

    /// Updates

    private void waitForCoinUpdate() {
        idleRoamUpdate();

        lookForCoinElapsed += Time.deltaTime;
        if (lookForCoinElapsed > lookForCoinInterval) {
            lookForCoinElapsed = 0;

            Coin coin = coinController.lookForCoin(transform.position, 10f);
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

            // foreach (Coin coin in worldCoins) {
            //     if (Vector2.Distance(pos, coin.transform.position) < distance) {
            //         GameObject reserver = null;
            //         if (!reservedCoins.TryGetValue(coin, out reserver))
            //             return coin;
            //     }
            // }

            // Coin coin = coinController.lookForCoin(transform.position, 10f);
            // if (coin != null) {
            //     var task = AITask.pickupCoinTask(coin);
            //     task.onComplete = () => {
            //         if (task.success) {
            //             becomePeasant();
            //         }
            //     };
            //     issueTask(task);
            // }
        }
    }

    private void lookForAnimalUpdate() {

    }

    private void chaseAnimalUpdate() {

    }

    /// Internal methods

    private void idleRoamUpdate() {
        roamElapsed += Time.deltaTime;
        if (roamElapsed > roamInterval) {
            roamElapsed = 0;

            Vector3 randomDelta = Vector3.zero;
            float range = 2f;
            randomDelta.x += Random.Range(-range, range);
            randomDelta.y += Random.Range(-range / 2, range / 2);
            var task = AITask.moveTask(transform.position + randomDelta);
            issueTask(task);
        }
    }

    private void becomePeasant() {
        type = PeasantType.Peasant;
        issueTask(AITask.typeUpdateTask());
    }
}