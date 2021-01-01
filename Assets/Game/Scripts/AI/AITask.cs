using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum AITaskType {
    Move,
    SquadFollow,
    SquadIdle,
    // Idle,
    DropCoins,
    PickupCoin,
    PickupWeapon,
    Kill,
    Construct,
    Repair,
    ChopDown,
    TypeUpdate
}

public enum AITaskState {
    Issued,
    InProgress,
    Finished,
    Failed,
    Cancelled
}

public class AITask {
    public long id;
    public AITaskType type;
    public AITaskState state;
    public string reason;
    public Action onComplete;   
    public bool scheduled;
    public float executeDelay;
    public bool cancellable;

    // TODO: object for data
    public Vector3 position; 
    public GameObject target;
    public Squad squad; // for squad tasks
    public NPCType peasantType; // for type update
    public int amountToDrop; // for droping coins
    public float duration; // for idle
    public float disctance; // for follow

    public bool success {get{ return state == AITaskState.Finished; }}
    public bool failed {get{ return state == AITaskState.Failed; }}
    public bool cancelled {get{ return state == AITaskState.Cancelled; }}

    private static long idCounter = 0;
    
    private AITask(AITaskType type) {
        this.type = type;
        this.state = AITaskState.Issued;
        this.scheduled = true;
        this.id = idCounter++;
    }

    /// Static initializers --

    public static AITask moveTask(Vector3 position) {
        AITask task = new AITask(AITaskType.Move);
        task.position = position;
        return task;
    }
    public static AITask squadIdleTask(Squad squad) {
        AITask task = new AITask(AITaskType.SquadIdle);
        task.squad = squad;
        task.cancellable = true;
        return task;
    }
    public static AITask squadFollowTask(Squad squad) {
        AITask task = new AITask(AITaskType.SquadFollow);
        task.squad = squad;
        task.cancellable = true;
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
        task.scheduled = false;
        return task;
    }
    public static AITask killTask(GameObject target) {
        AITask task = new AITask(AITaskType.Kill);
        task.target = target;
        return task;
    }
    public static AITask dropCoinsTask(int amount) {
        AITask task = new AITask(AITaskType.DropCoins);
        task.amountToDrop = amount;
        task.scheduled = false;
        return task;
    }
    public static AITask constructionTask(Building building) {
        AITask task = new AITask(AITaskType.Construct);
        task.target = building.gameObject;
        return task;
    }
    public static AITask repairTask(Building building) {
        AITask task = new AITask(AITaskType.Repair);
        task.target = building.gameObject;
        return task;
    }
    public static AITask chopDownTask(Tree tree) {
        AITask task = new AITask(AITaskType.ChopDown);
        task.target = tree.gameObject;
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
        if (!scheduled) onComplete?.Invoke();
    }

    public void fail(string reason = null) {
        this.state = AITaskState.Failed;
        this.reason = reason;
        if (!scheduled) onComplete?.Invoke();
    }

    public void cancel(string reason = null) {
        this.state = AITaskState.Cancelled;
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