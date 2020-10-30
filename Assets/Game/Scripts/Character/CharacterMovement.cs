using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour {
    /// Public -- 
    public float movementSpeed;

    [HideInInspector] public Vector3 direction = Vector2.zero;
    [HideInInspector] public List<Vector2> currentPath;

    /// Private -- 
    
    private Animator animator;
    private TilemapController tilemapController;

    void Start() {
        animator = GetComponent<Animator>();
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
            currentPath = new List<Vector2>() { new Vector2(dest.x, dest.y) };
        }
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
        }
    }
}