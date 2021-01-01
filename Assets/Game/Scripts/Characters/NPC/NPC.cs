using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPC : MonoBehaviour {
    private Animator animator;

    protected PathfindController pathfindController;
    protected CoinController coinController;
    protected WeaponController weaponController;
    protected CharacterMovement movement;

    public AIBase ai {get; private set; }

    /// Protected -- 
    
    virtual protected void Awake() {
        animator = GetComponentInChildren<Animator>();
        movement = GetComponent<CharacterMovement>();
        ai = GetComponent<AIBase>();
        ai.onTaskIssued += onTaskReceived;
        ai.onTaskCancelled += onTaskCancelled;
    }

    virtual protected void Start() {
        pathfindController = FindObjectOfType<PathfindController>();
        coinController = FindObjectOfType<CoinController>();
        weaponController = FindObjectOfType<WeaponController>();
    }

    virtual protected void Update() {
        
    }

    protected abstract void onTaskReceived(AITask task);
    protected abstract void onTaskCancelled(AITask task);

    protected void moveTo(Vector3 worldDest, System.Action<bool> onComplete = null) {
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