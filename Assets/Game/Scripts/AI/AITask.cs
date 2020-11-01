using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum AITaskType {
    Move,
    Stop,
    DropCoin,
    PickupCoin,
    PickupWeapon,
    Attack,
    TypeUpdate
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
    public Action onComplete;   

    // TODO: object for data
    public Vector3 position;
    public GameObject target;
    public NPCType peasantType;
    
    public bool success {get{ return state == AITaskState.Finished; }}
    public bool failed {get{ return state == AITaskState.Failed; }}

    private static long idCounter = 0;
    
    private AITask(AITaskType type) {
        this.type = type;
        this.state = AITaskState.Issued;
        
        this.id = idCounter++;
    }

    /// Static initializers --

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
    public static AITask pickupWeaponTask(Weapon weapon) {
        AITask task = new AITask(AITaskType.PickupWeapon);
        task.target = weapon.gameObject;
        return task;
    }
    public static AITask typeUpdateTask(NPCType type) {
        AITask task = new AITask(AITaskType.TypeUpdate);
        task.peasantType = type;
        return task;
    }

    /// Public -- 
    
    public void begin(string reason = null) {
        this.state = AITaskState.InProgress;
        this.reason = reason;
    }

    public void finish(string reason = null) {
        this.state = AITaskState.Finished;
        this.reason = reason;
    }

    public void fail(string reason = null) {
        this.state = AITaskState.Failed;
        this.reason = reason;
    }

    override public string ToString() {
        string s = $"Task #{id} Type: {type}, sate: {state}";
        if (reason != null) s += $" reason: \"{reason}\"";
        if (position != default(Vector3)) s += $" position: {position}";
        if (target != null) s += $" target: {target}";

        return s;
    }
}