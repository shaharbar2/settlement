using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peasant : MonoBehaviour {
    private CoinController coinController;
    private PathfindController pathfindController;
    
    private CharacterMovement movement;
    private PeasantAI ai;

    private float moveInterval = 5f;
    private float moveElapsed = 4f;
    private bool stateMove = true;
    
    void Awake() {
        movement = GetComponent<CharacterMovement>();
        ai = GetComponent<PeasantAI>();
    }

    void Start() {
        coinController = FindObjectOfType<CoinController>();
        pathfindController = FindObjectOfType<PathfindController>();
    }

    void Update() {
        if (stateMove) {
            moveElapsed += Time.deltaTime;
            if (moveElapsed > moveInterval) {
                moveElapsed = 0;

                Vector3 randomDelta = Vector3.zero;
                float range = 2f;
                randomDelta.x += Random.Range(-range, range);
                randomDelta.y += Random.Range(-range / 2, range / 2);
                moveTo(transform.position + randomDelta);
            }
        }
    }

    public void moveTo(Vector3 worldDest) {
        Vector2 worldPos = new Vector2(transform.position.x, transform.position.y);
        List<Vector2> path = pathfindController.findPathWorld(worldPos, worldDest);

        // remove point of current position from the path
        if (path != null && path.Count > 0) {
            path.RemoveAt(0);
        }
        movement.currentPath = path;
    }
}