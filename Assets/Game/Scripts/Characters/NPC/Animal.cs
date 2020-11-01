using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : NPC {

    public delegate void AnimalEvent(Animal animal);
    public event AnimalEvent onDeath;

    public bool isAlive { get { return hp > 0; } }

    private int hp = 2;
    private Vector3 lastAttackFrom;

    override protected void Start() {
        base.Start();
    }

    override protected void Update() {
        base.Update();
    }

    /// Public -- 

    public Vector3 getHitPosition() {
        return transform.position + new Vector3(0, 0.1f);
    }
    public Vector3 getMissPosition() {
        float xOffset = Random.Range(0.2f, 0.5f) * (BabyUtils.randomBool ? -1 : 1);
        return transform.position + new Vector3(xOffset, 0f);
    }

    public bool hit(Vector3 from) {
        if (hp > 0) {
            lastAttackFrom = from;
            hp -= 1;
            return true;
        }
        return false;
    }

    public void animateHit() {
        if (hp <= 0) {
            animateDeath();
        }
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.material = new Material(spriteRenderer.material);
        spriteRenderer.material.SetColor("_Color", Color.black);

        // ToDo: lol, please make proper animation curve here
        float blinkTime = 0.1f;
        LeanTween.delayedCall(blinkTime, () => {
            spriteRenderer.material.SetColor("_Color", Color.white);
            LeanTween.delayedCall(blinkTime, () => {
                spriteRenderer.material.SetColor("_Color", Color.black);
                LeanTween.delayedCall(blinkTime, () => {
                    spriteRenderer.material.SetColor("_Color", Color.white);
                });
            });
        });
    }

    /// Protected -- 

    override protected void onTaskReceived(AITask task) {
        Debug.Log("animal task received");
        switch (task.type) {
            case AITaskType.Move:
                task.begin();
                moveTo(task.position, onComplete: (bool finished) => {
                    if (finished)task.finish(reason: "destination reached");
                    else task.fail(reason: "movement was interrupted");
                });
                break;
            default:
                Debug.Log("Undefined animal behavior for command: " + task.type);
                break;
        }
    }

    /// Private -- 

    private void animateDeath() {
        GetComponentInChildren<Animator>().SetBool("isHit", true);
        LeanTween.delayedCall(1.5f, () => {
            if (gameObject) {
                onDeath?.Invoke(this);
                Destroy(gameObject);
            }
        });
        LeanTween.delayedCall(1f, () => {
            if (gameObject) {
                dropCoin();
            }
        });
    }

    private void dropCoin() {
        var coinController = FindObjectOfType<CoinController>();
        float direction = lastAttackFrom.x < transform.position.x ? 1 : -1;
        coinController.dropCoin(transform.position, direction, CoinDropType.ByAnimal);
    }
}