using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private Animator animator;
    private float movementSpeed = 1f;
    private Vector3 direction = Vector2.zero;

    private PathfindController pathfindController;
    private TilemapController tilemapController;
    private List<Vector2> currentPath;
    private GameUI ui;

    void Start() {
        animator = GetComponent<Animator>();
        ui = FindObjectOfType<GameUI>();
        pathfindController = FindObjectOfType<PathfindController>();
        tilemapController = FindObjectOfType<TilemapController>();
    }

    void Update() {
        // // Point n click movement
        // if (Input.GetMouseButton(0)) {
        //     Vector2 worldPosition = new Vector2(transform.position.x, transform.position.y);
        //     Vector2 worldDestination = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //     currentPath = pathfindController.findPathWorld(worldPosition, worldDestination);

        //     // remove point of current position from the path
        //     if (currentPath != null && currentPath.Count > 0) {
        //         currentPath.RemoveAt(0);
        //     }
        // }

        if (currentPath != null) {
            updatePathMovement();
        } else {
            updateManualMovement();
        }

        updateAnimation();
        checkBuildProximity();
    }

    /// Private -- 
    
    private void checkBuildProximity() {
        BuildingSprite building = FindObjectOfType<BuildingSprite>();
        if (!building.isBuilt) {
            if (tilemapController.tileDistance(transform.position, building.transform.position).magnitude < 2f) {
                tilemapController.highlightForBuild(building.transform.position, TilemapAreaType.S_3x3);
                ui.setHintVisible(true);
                if (Input.GetKeyDown(KeyCode.Space)) {
                    building.build();
                    tilemapController.markUnwalkable(building.transform.position, TilemapAreaType.S_3x3);
                    tilemapController.removeHighlightForBuild();
                    ui.setHintVisible(false);
                    leaveBuildArea();
                }
            } else {
                tilemapController.removeHighlightForBuild();
                ui.setHintVisible(false);
            }
        }
    }

    // TODO: proper path for leaving area to nearest free tile ignoring walls?
    private void leaveBuildArea() {
        currentPath = new List<Vector2>(){new Vector2(transform.position.x - 0.5f, transform.position.y - 0.5f)};
    }

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
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        direction = new Vector3(h, v, 0);
        direction = direction.normalized * movementSpeed * Time.deltaTime;
        if (tilemapController.isWalkable(transform.position + direction)){
            transform.position += direction;
        }
    }

    private void updatePathMovement() {
        Vector2 worldPosition = new Vector2(transform.position.x, transform.position.y);
        direction = Vector2.zero;
        while(currentPath.Count > 0) {
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