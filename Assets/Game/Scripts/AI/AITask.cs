using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AITaskType {
    Move,
    Stop,
    DropCoin,
    PickupCoin,
    Attack
}

public enum AITaskState {
    Issued,
    InProgress,
    Finished,
    Failed
}

public class AITask {
    public long id;
    public AITaskType type;
    public AITaskState state;
    public string reason;
    public Vector3 position;
    public GameObject target;

    private static long idCounter = 0;
    
    private AITask(AITaskType type) {
        this.type = type;
        this.state = AITaskState.Issued;
        this.id = idCounter++;
    }
    public static AITask moveTask(Vector3 position) {
        AITask task = new AITask(AITaskType.Move);
        task.position = position;
        return task;
    }
    public static AITask pickupCoinTask(Coin coin) {
        AITask task = new AITask(AITaskType.PickupCoin);
        task.target = coin.gameObject;
        return task;
    }

    override public string ToString() {
        return $"Task #{id} Type: {type}, sate: {state}, reason: {reason}, position: {position}, target: {target}";
    }
}