using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private CoinController coinController;
    private PathfindController pathfindController;
    private WeaponController weaponController;
    private GameUI ui;

    private float pathfindMouseInterval = 0.2f;
    private float pathfindMouseElapsed = 0.2f;

    [SerializeField] private BoxCollider2D feetCollider;

    private ContactFilter2D feetContactFilter = new ContactFilter2D();
    private Collider2D[] feetCollisions = new Collider2D[15];

    private Building collidedBuilding;
    private CharacterMovement movement;

    void Awake() {
        movement = GetComponent<CharacterMovement>();
    }
    
    void Start() {
        ui = FindObjectOfType<GameUI>();
        weaponController = FindObjectOfType<WeaponController>();
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

        detectFeetCollisions();
        
        if (collidedBuilding != null) {
            string interactHint = collidedBuilding.getInteractHint();
            if (interactHint != null) {
                ui.showHint(interactHint);
            }
        } else {
            ui.hideHint();
        }

        if (Input.GetKeyDown(Constants.instance.COIN_KEY_CODE)) {
            if (collidedBuilding != null && collidedBuilding.isUsableState) {
                Building building = collidedBuilding;
                BuildingData buildingData = BuildingConfiguration.instance.buildingDataFor(building.type);
                Vector3 from = transform.position;
                Vector3 to = collidedBuilding.getCoinsAnchor();
                coinController.spend(buildingData.costToUse, from, to, onComplete: () => {
                    WeaponType weaponType = WeaponController.weaponTypeForBuilding(building.type);
                    weaponController.drop(weaponType, building.getWeaponAnchor());
                });
            } else {
                coinController.dropCoin(transform.position, transform.localScale.x, CoinDropType.ByPlayer);
            }
        }
    }

    /// Private -- 

    private void detectFeetCollisions() {
        feetContactFilter.useTriggers = true;
        float count = feetCollider.OverlapCollider(feetContactFilter, feetCollisions);
        Coin coin = null;
        
        collidedBuilding = null;
        Building building = null;
        for (int i = 0; i < count; i++) {
            coin = feetCollisions[i].transform.parent.GetComponent<Coin>();
            building = feetCollisions[i].transform.parent.GetComponent<Building>();
            if (coin != null) {
                coinController.pickup(coin, transform.position, byPlayer: true);
            }
            if (building != null) {
                collidedBuilding = building;
            }
        }
    }

  
    private void updateWASDMovement() {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        movement.direction = new Vector3(h, v, 0).normalized;
    }

    private void handlePointAndClickMovement() {
        if (Input.GetMouseButton(0) && !ui.blockPlayerMouseMovement) {
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
                movement.movePath(path);
            }
        } else {
            pathfindMouseElapsed = pathfindMouseInterval;
        }
    }

}