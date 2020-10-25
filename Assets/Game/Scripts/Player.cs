using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    [SerializeField] private BuildingSprite buildingPrefab;

    private Animator animator;
    private float movementSpeed = 1f;
    private Vector3 direction = Vector2.zero;

    private PathfindController pathfindController;
    private TilemapController tilemapController;
    private List<Vector2> currentPath;
    private GameUI ui;

    private RadialMenuSegmentData highlightedBuildingData;

    void Start() {
        animator = GetComponent<Animator>();
        ui = FindObjectOfType<GameUI>();
        pathfindController = FindObjectOfType<PathfindController>();
        tilemapController = FindObjectOfType<TilemapController>();
        ui.radialMenu.onClicked += onBuildingSelected;
        ui.radialMenu.onHighlighted += onBuildingHighlighted;
    }

    void Update() {
        // handlePointAndClickMovement();

        if (currentPath != null) {
            updatePathMovement();
        } else {
            updateManualMovement();
        }

        updateAnimation();

        if (direction != Vector3.zero) {
            if (highlightedBuildingData != null) {
                tilemapController.highlightForBuild(transform.position, highlightedBuildingData.areaType);
            }
        }
    }

    /// Private -- 

    private void onBuildingHighlighted(RadialMenuSegmentData data) {
        tilemapController.highlightForBuild(transform.position, data.areaType);
        highlightedBuildingData = data;
    }

    private void onBuildingSelected(RadialMenuSegmentData data) {
        tilemapController.removeHighlightForBuild();
        highlightedBuildingData = null;
        if (data != null) {
            BuildingSprite building = Instantiate(buildingPrefab);
            building.transform.position = transform.position;
            building.build();
            tilemapController.markUnwalkable(building.transform.position, TilemapAreaType.S_3x3);
            tilemapController.removeHighlightForBuild();
            ui.setHintVisible(false);
            leaveBuildArea();
        }
    }

    // TODO: proper path for leaving area to nearest free tile ignoring walls?
    private void leaveBuildArea() {
        currentPath = new List<Vector2>() { new Vector2(transform.position.x - 0.5f, transform.position.y - 0.5f) };
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

    private void handlePointAndClickMovement() {
        if (Input.GetMouseButton(0)) {
            Vector2 worldPosition = new Vector2(transform.position.x, transform.position.y);
            Vector2 worldDestination = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentPath = pathfindController.findPathWorld(worldPosition, worldDestination);

            // remove point of current position from the path
            if (currentPath != null && currentPath.Count > 0) {
                currentPath.RemoveAt(0);
            }
        }
    }

    private void updateManualMovement() {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        direction = new Vector3(h, v, 0);
        direction = direction.normalized * movementSpeed * Time.deltaTime;
        if (tilemapController.isWalkable(transform.position + direction)) {
            transform.position += direction;
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