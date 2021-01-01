using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peasant : NPC {

    public PeasantAIParams aiParams { get; private set; }

    private PeasantTitle title;

    [SerializeField] private BoxCollider2D feetCollider;
    [SerializeField] private Sprite arrowSprite;

    private GameObject followTarget;
    private Vector3 ongoingMovementDestination;
    private float ongoingMovementElapsed;
    private float ongoingMovementInterval = 1f; // TODO: this must not be const
    private bool isOngoingMovement;

    override protected void Awake() {
        title = GetComponentInChildren<PeasantTitle>();
        aiParams = new PeasantAIParams() {
            squadIdleMinDelay = 0.5f,
            squadIdleMaxDelay = 1f,

            startMoveMinDelay = 0.2f,
            startMoveMaxDelay = 0.3f
        };

        base.Awake();
    }

    override protected void Start() {
        base.Start();
    }

    override protected void Update() {
        movement.movementSpeed = Constants.instance.PEASANT_SPEED;

        handleOngoingMovement();

        base.Update();
    }

    /// Public -- 

    public bool feetCollidesZone(GameObject gameObject) {
        var contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = true;
        Collider2D[] feetCollisions = new Collider2D[15];
        float count = feetCollider.OverlapCollider(contactFilter, feetCollisions);

        for (int i = 0; i < count; i++) {
            if (gameObject == feetCollisions[i].gameObject) {
                return true;
            }
        }
        return false;
    }

    /// Protected -- 

    override protected void onTaskReceived(AITask task) {
        if (task.executeDelay > 0) {
            LeanTween.delayedCall(task.executeDelay, () => {
                executeTask(task);
            });
        } else {
            executeTask(task);
        }
    }

    protected override void onTaskCancelled(AITask task) {
        switch (task.type) {
            case AITaskType.SquadIdle:
                isOngoingMovement = false;
                task.finish(reason: "cancelled");
                break;
            case AITaskType.SquadFollow:
                isOngoingMovement = false;
                task.finish(reason: "cancelled");
                break;
            default:
                Debug.LogWarning("Dont't know how to cancel: " + task.type);
                break;
        }
    }

    /// Private -- 

    private void executeTask(AITask task) {
        switch (task.type) {
            case AITaskType.Move:
                task.begin();
                moveTo(task.position, onComplete: (bool finished) => {
                    if (finished)task.finish(reason: "destination reached");
                    else task.fail(reason: "movement was interrupted");
                });
                break;
                // case AITaskType.Idle:
                //     task.begin(reason: "will idle for " + task.duration + "ms");
                //     LeanTween.delayedCall(task.duration, () => {
                //         task.finish(reason: "idle complete");
                //     });
                //     break;

            case AITaskType.SquadIdle:
            case AITaskType.SquadFollow:
                isOngoingMovement = true;
                followTarget = task.squad.leader.gameObject;
                break;

            case AITaskType.PickupCoin:
                task.begin();
                Coin coin = task.target.GetComponent<Coin>();
                moveTo(coin.transform.position, onComplete: (bool finished) => {
                    if (!finished) {
                        task.fail(reason: "movement to coin was interrupted");
                    } else {
                        if (coin != null) {
                            coinController.pickup(coin, transform.position, byPlayer : false);
                            task.finish(reason: "coin picked up");
                        } else {
                            task.fail(reason: "coin doesnt exist anymore");
                        }
                    }
                });
                break;
            case AITaskType.PickupWeapon:
                task.begin();
                Weapon weapon = task.target.GetComponent<Weapon>();
                moveTo(weapon.transform.position, onComplete: (bool finished) => {
                    if (!finished) {
                        task.fail(reason: "movement to weapon was interrupted");
                    } else {
                        if (weapon != null) {
                            weaponController.pickup(weapon, onComplete: () => {
                                task.finish(reason: "weapon picked up");
                            });
                        } else {
                            task.fail(reason: "weapon doesnt exist anymore");
                        }
                    }
                });
                break;
            case AITaskType.TypeUpdate:
                task.begin();
                title.updateTitle(task.peasantType);
                task.finish(reason: "type updated to " + task.peasantType);
                break;
            case AITaskType.Kill:
                task.begin();
                Animal animal = task.target.GetComponent<Animal>();
                performAttack(animal, onComplete: (bool finished) => {
                    if (finished)task.finish(reason: "attack performed, animal killed");
                    else task.fail(reason: "animal survived attack");
                });
                break;
            case AITaskType.DropCoins:
                task.begin();
                dropCoins(task.amountToDrop, onComplete: (bool finished) => {
                    if (finished)task.finish(reason: $"dropped {task.amountToDrop} coins");
                    else task.fail(reason: "could not drop coins");
                });
                break;
            case AITaskType.Construct:
                task.begin();
                Building building = task.target.GetComponent<Building>();
                build(building, onComplete: (finished) => {
                    if (finished)task.finish(reason: $"finished constructing {building.type}");
                    else task.fail(reason: "unable to finish construction");
                });
                dropCoins(task.amountToDrop, onComplete: (bool finished) => {
                    if (finished)task.finish(reason: $"dropped {task.amountToDrop} coins");
                    else task.fail(reason: "could not drop coins");
                });
                break;
            case AITaskType.ChopDown:
                task.begin();
                Tree tree = task.target.GetComponent<Tree>();
                performChop(tree, onComplete: (finished) => {
                    if (finished)task.finish(reason: $"tree was chopped down {tree}");
                    else task.fail(reason: "tree chopped, but it still stands");
                });
                break;
            default:
                Debug.LogWarning("Undefined peasant behavior for command: " + task.type);
                break;
        }
    }

    private void handleOngoingMovement() {
        if (isOngoingMovement) {
            ongoingMovementElapsed += Time.deltaTime;
            if (ongoingMovementElapsed >= ongoingMovementInterval) {
                ongoingMovementElapsed = 0;
                Vector2 worldPosition = new Vector2(transform.position.x, transform.position.y);
                Vector2 worldDestination = followTarget.transform.position;

                List<Vector2> path = pathfindController.findPathWorld(worldPosition, worldDestination);

                // remove point of current position from the path
                if (path != null && path.Count > 0) {
                    path.RemoveAt(0);
                }
                movement.movePath(path);
            }
        } else {
            ongoingMovementElapsed = ongoingMovementInterval;
        }
    }

    private void build(Building building, System.Action<bool> onComplete) {
        building.build(onComplete);
        movement.lookAt(transform.position + Vector3.down);
    }

    private void performChop(Tree tree, System.Action<bool> onComplete) {
        tree.chop(onComplete: (treeIsDown) => {
            onComplete(treeIsDown);
        });
    }

    private void performAttack(Animal animal, System.Action<bool> onComplete) {
        bool isHit = BabyUtils.chance(Constants.instance.PEASANT_HIT_CHANCE);
        bool didHit = false;
        Vector3 targetPos = Vector3.zero;
        if (isHit) {
            didHit = animal.hit(transform.position);
            targetPos = animal.getHitPosition();
        } else {
            targetPos = animal.getMissPosition();
        }
        Debug.Log($"{gameObject.name} didHit: {didHit}, animalAlive: {animal.isAlive}");
        movement.lookAt(targetPos);

        GameObject arrow = new GameObject();
        arrow.name = "Bow Arrow";

        float characterHeight = 0.3f;
        Vector3 arrowPos = transform.position + new Vector3(0, characterHeight);

        arrow.transform.position = arrowPos;

        SpriteRenderer spriteRenderer = arrow.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Objects";
        spriteRenderer.sprite = arrowSprite;

        float distance = Vector2.Distance(arrowPos, targetPos);
        float angle = BabyUtils.VectorAngle(arrowPos, targetPos);
        arrow.transform.Rotate(new Vector3(0, 0, -angle - 180));

        float flightSpeed = Constants.instance.PEASANT_ARROW_FLIGHT_SPEED;
        float flightTime = distance / flightSpeed;
        float hitTime = flightTime * 0.7f;
        LTDescr flightTween = LeanTween.move(arrow, targetPos, flightTime);
        flightTween.setEaseInOutExpo();
        flightTween.setOnComplete(() => {
            Destroy(arrow);
            onComplete?.Invoke(!animal.isAlive && didHit);
        });
        if (isHit && didHit) {
            LeanTween.delayedCall(hitTime, animal.animateHit);
        }
    }

    private void dropCoins(int amount, System.Action<bool> onComplete) {
        float direction = FindObjectOfType<Player>().transform.position.x < transform.position.x ? 1 : -1;
        for (int i = 0; i < amount; i++) {
            LeanTween.delayedCall(i * 0.2f, () => {
                coinController.dropCoin(transform.position, direction, CoinDropType.ByPeasant);
            });
        }
        onComplete?.Invoke(true);
    }
}