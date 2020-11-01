using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour {
    /// Public -- 
    [HideInInspector] public float movementSpeed;
    [HideInInspector] public Vector3 direction = Vector2.zero;

    /// Private -- 
    
    private Animator animator;
    private TilemapController tilemapController;

    private List<Vector2> currentPath;
    // true - finished, false - interrupted
    private System.Action<bool> onPathComplete;
    private bool isMoving { get {
        return currentPath != null || direction.x != 0 || direction.y != 0;
    }}

    void Start() {
        animator = GetComponentInChildren<Animator>();
        tilemapController = FindObjectOfType<TilemapController>();
    }

    void Update() {
        if (currentPath != null) {
            updatePathMovement();
        } else {
            updateManualMovement();
        }
        updateAnimation();
    }

    /// Public -- 

    public void leaveBuildArea() {
        if (!tilemapController.isWalkable(transform.position)) {
            Vector2 dest = tilemapController.nearestWalkablePosition(transform.position);
            movePath(new List<Vector2>() { new Vector2(dest.x, dest.y) });
        }
    }

    public void movePath(List<Vector2> path, System.Action<bool> onComplete = null) {
        if (currentPath != null) {
            onPathComplete?.Invoke(false);
        }
        currentPath = path;
        onPathComplete = onComplete;
    }

    public void lookAt(Vector3 targetPos) {
        
        if (transform.position.x > targetPos.x) {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x) * 1, transform.localScale.y);
        } else if (transform.position.x < targetPos.x) {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y);
        }
        GetComponentInChildren<Animator>().SetBool("isFront", transform.position.y > targetPos.y);
            
    }
    /// Private -- 

    private void updateAnimation() {
        if (direction.y < 0) {
            animator.SetBool("isFront", true);
        } else if (direction.y > 0) {
            animator.SetBool("isFront", false);
        }

        animator.SetBool("isWalking", direction.x != 0 || direction.y != 0);

        // flip sprite according to direction
        if (direction.x > 0) {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y);
        } else if (direction.x < 0) {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
    }

    private void updateManualMovement() {
        Vector3 movement = direction * movementSpeed * Time.deltaTime;
        if (tilemapController.isWalkable(transform.position + movement)) {
            transform.position += movement;
        }
    }

    private void updatePathMovement() {
        Vector2 worldPosition = new Vector2(transform.position.x, transform.position.y);
        direction = Vector2.zero;
        while (currentPath.Count > 0) {
            Vector2 point = currentPath[0];
            if (Vector2.Distance(point, worldPosition) < movementSpeed * Time.deltaTime) {
                currentPath.RemoveAt(0);
                continue;
            } else {
                Vector2 newPos = Vector2.MoveTowards(worldPosition, point, movementSpeed * Time.deltaTime);
                direction.x = point.x - worldPosition.x;
                direction.y = point.y - worldPosition.y;
                transform.position = newPos;
                break;
            }
        }
        if (currentPath.Count == 0) {
            currentPath = null;
            direction = Vector3.zero;
            onPathComplete?.Invoke(true);
            onPathComplete = null;
        }
    }
}