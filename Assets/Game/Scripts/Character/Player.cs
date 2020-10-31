using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private CoinController coinController;
    private PathfindController pathfindController;

    private float pathfindMouseInterval = 1.2f;
    private float pathfindMouseElapsed = 1.2f;

    [SerializeField] private BoxCollider2D feetCollider;

    private ContactFilter2D feetContactFilter = new ContactFilter2D();
    private Collider2D[] feetCollisions = new Collider2D[15];

    private CharacterMovement movement;

    void Awake() {
        movement = GetComponent<CharacterMovement>();
    }
    
    void Start() {
        coinController = FindObjectOfType<CoinController>();
        pathfindController = FindObjectOfType<PathfindController>();
    }

    void Update() {
        movement.movementSpeed = Constants.instance.PLAYER_SPEED;
        if (Constants.instance.PLAYER_CONTROL_STYLE == PlayerControlStyle.POINTCLICK) {
            handlePointAndClickMovement();
        } else if (Constants.instance.PLAYER_CONTROL_STYLE == PlayerControlStyle.WASD) {
            updateWASDMovement();
        }

        if (Input.GetKeyDown(KeyCode.F)) {
            coinController.dropCoin(transform.position, transform.localScale.x);
        }

        detectFeetCollisions();
    }

    /// Private -- 

    private void detectFeetCollisions() {
        feetContactFilter.useTriggers = true;
        float count = feetCollider.OverlapCollider(feetContactFilter, feetCollisions);
        Coin coin = null;

        for (int i = 0; i < count; i++) {
            coin = feetCollisions[i].transform.parent.GetComponent<Coin>();
            if (coin != null) {
                coinController.pickup(coin, transform.position);
            }
        }
    }

    private void updateWASDMovement() {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        movement.direction = new Vector3(h, v, 0).normalized;
    }

    private void handlePointAndClickMovement() {
        if (Input.GetMouseButton(0)) {
            pathfindMouseElapsed += Time.deltaTime;
            if (pathfindMouseElapsed >= pathfindMouseInterval) {
                pathfindMouseElapsed = 0;
                Vector2 worldPosition = new Vector2(transform.position.x, transform.position.y);
                Vector2 worldDestination = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                List<Vector2> path = pathfindController.findPathWorld(worldPosition, worldDestination);

                // remove point of current position from the path
                if (path != null && path.Count > 0) {
                    path.RemoveAt(0);
                }
                movement.currentPath = path;
            }
        } else {
            pathfindMouseElapsed = pathfindMouseInterval;
        }
    }

}