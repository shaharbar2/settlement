using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peasant : NPC {

    private PeasantTitle title;

    [SerializeField] private Sprite arrowSprite;

    override protected void Awake() {
        title = GetComponentInChildren<PeasantTitle>();
        base.Awake();
    }

    override protected void Start() {
        base.Start();
    }

    override protected void Update() {
        movement.movementSpeed = Constants.instance.PEASANT_SPEED;

        base.Update();
    }

    /// Public -- 

    /// Protected -- 

    override protected void onTaskReceived(AITask task) {
        switch (task.type) {
            case AITaskType.Move:
                task.begin();
                moveTo(task.position, onComplete: (bool finished) => {
                    if (finished)task.finish(reason: "destination reached");
                    else task.fail(reason: "movement was interrupted");
                });
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
                performAttack(task.target, onComplete: (bool finished) => {
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
            default:
                Debug.Log("Undefined peasant behavior for command: " + task.peasantType);
                break;
        }
    }

    /// Private -- 

    private void performAttack(GameObject target, System.Action<bool> onComplete) {
        Animal animal = target.GetComponent<Animal>();
        bool isHit = BabyUtils.chance(Constants.instance.PEASANT_HIT_CHANCE);
        bool didHit = false;
        Vector3 targetPos = Vector3.zero;
        if (isHit) {
            didHit = animal.hit(transform.position);
            targetPos = animal.getHitPosition();
        } else {
            targetPos = animal.getMissPosition();
        }
        Debug.Log($"{gameObject.name} didHit: {didHit}, animalAlive: {animal.isAlive}" );
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