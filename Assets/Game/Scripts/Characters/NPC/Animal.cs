using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : NPC {

    private int hp = 2;
    public bool isAlive { get { return hp > 0; }}

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


    public void hit() {
        hp -= 1;
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

    public void animateDeath() {
        GetComponentInChildren<Animator>().SetBool("isHit", true);
        LeanTween.delayedCall(1.5f, () => {
            Destroy(gameObject);
        });
        // GetComponentInChildren<Animator>().Play("hit");
    }
    /// Private -- 

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
}