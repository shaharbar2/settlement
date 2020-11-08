using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIBase : MonoBehaviour {
    public delegate void AIEvent(AITask command);
    public event AIEvent onTaskIssued;

    protected WorldObjectFinder finder;
    protected WeaponController weaponController;
    protected CoinController coinController;
    protected Player player;

    protected AITask currentTask;

    /// timings
    private float roamInterval = 5f;
    private float roamElapsed = 0f;

    protected virtual void Awake() {
    
    }

    protected virtual void Start() {
        coinController = FindObjectOfType<CoinController>();
        weaponController = FindObjectOfType<WeaponController>();
        finder = new WorldObjectFinder(gameObject);
    }

    protected virtual void Update() {
        if (currentTask != null) {
            if (currentTask.failed) {
                onTaskFailed(currentTask);
            } else if (currentTask.success) {
                onTaskFinished(currentTask);
            }
        } else {
           updateStateMachine();
        }
    }

    protected abstract void updateStateMachine();
      
      /// Task handlers

    private void onTaskFailed(AITask task) {
        Debug.Log($"{gameObject.name} failed: {task}");
        if (task.scheduled) currentTask = null;
        task.onComplete?.Invoke();
    }

    private void onTaskFinished(AITask task) {
        Debug.Log($"{gameObject.name} finished: {task}");
        if (task.scheduled) currentTask = null;
        task.onComplete?.Invoke();
    }

    protected void issueTask(AITask task) {
        if (task.scheduled) {
            currentTask = task;
            Debug.Log($"{gameObject.name} issued : {task}");
        } else {
            Debug.Log($"{gameObject.name} unscheduled issued: {task}");
        }
        onTaskIssued?.Invoke(task);
    }


    protected void idleRoamUpdate() {
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
}
