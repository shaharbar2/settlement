using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : NPC {

    override protected void Start() {
        base.Start();
    }

    override protected void Update() {
        base.Update();
    }

    /// Private -- 

    override protected void onTaskReceived(AITask task) {
        Debug.Log("animal task received");
        switch (task.type) {
            case AITaskType.Move:
                task.begin();
                moveTo(task.position, onComplete: (bool finished) => {
                    if (finished)task.finish(reason: "destination reached");
                    else task.fail(reason: "movement was interrupted");
                });
                break;
            default:
                Debug.Log("Undefined animal behavior for command: " + task.type);
                break;
        }
    }
}