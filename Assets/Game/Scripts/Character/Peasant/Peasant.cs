using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peasant : MonoBehaviour {
    private PathfindController pathfindController;
    private CoinController coinController;

    private CharacterMovement movement;
    private PeasantAI ai;

    void Awake() {
        movement = GetComponent<CharacterMovement>();
        coinController = FindObjectOfType<CoinController>();
        ai = GetComponent<PeasantAI>();
        ai.onTaskIssued += onTaskReceived;
    }

    void Start() {
        pathfindController = FindObjectOfType<PathfindController>();
    }

    void Update() {

    }

    /// Public -- 

    /// Private -- 

    private void onTaskReceived(AITask task) {
        switch (task.type) {
            case AITaskType.Move:
                task.state = AITaskState.InProgress;
                moveTo(task.position, onComplete: (bool finished) => {
                    task.state = finished ? AITaskState.Finished : AITaskState.Failed;
                });
                break;
            case AITaskType.PickupCoin:
                task.state = AITaskState.InProgress;
                Coin coin = task.target.GetComponent<Coin>();
                moveTo(coin.transform.position, onComplete: (bool finished) => {
                    if (!finished) {
                        task.state = AITaskState.Failed;
                    } else {
                        if (coin != null) {
                            coinController.pickup(coin, transform.position, byPlayer: false);
                            task.state = AITaskState.Finished;
                        } else {
                            task.state = AITaskState.Failed;
                        }
                    }
                });
                break;
            default:
                Debug.Log("Undefined behavior for command: " + task.type);
                break;
        }
    }

    private void moveTo(Vector3 worldDest, System.Action<bool> onComplete = null) {
        Vector2 worldPos = new Vector2(transform.position.x, transform.position.y);
        List<Vector2> path = pathfindController.findPathWorld(worldPos, worldDest);

        // remove point of current position from the path
        if (path != null && path.Count > 0) {
            path.RemoveAt(0);
        } 
        if (path == null) {
            onComplete?.Invoke(false);
        } else {
            movement.movePath(path, onComplete);
        }
    }
}