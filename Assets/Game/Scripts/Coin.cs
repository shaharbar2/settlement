using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {
    [SerializeField] private Animator coin;
    [SerializeField] private SpriteRenderer shadow;
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Coin Settings")]
    [SerializeField] private AnimationCurve dropCurveY;
    [SerializeField] private AnimationCurve dropCurve;
    [SerializeField] private float dropTime;
    [SerializeField] private float pickupTime;
    [SerializeField] private float bounceHeight;
    [SerializeField] private float bounceTime;

    private LTDescr bounceTween;
    private float bounceLerp;

    private LTDescr dropTween;
    private float dropLerp;

    private LTDescr fadeTween;
    private float fadeLerp;

    private Vector3 dropStartPos;

    private SpriteRenderer coinRenderer;
    private ContactFilter2D contactFilter = new ContactFilter2D();
    private Collider2D[] coinCollisions = new Collider2D[15];

    void Awake() {
        coinRenderer = coin.GetComponent<SpriteRenderer>();
    }

    void Update() {
        contactFilter.useTriggers = true;
        float count = boxCollider.OverlapCollider(contactFilter, coinCollisions);
        if (count > 0) {
            shadow.color = shadow.color.setAlpha(0.32f / ((count + 1) * 0.4f));
        }
    }

    /// Public -- 

    public void bounce() {
        stopBounce();
        bounceTween = LeanTween.value(gameObject, 0f, 1f, bounceTime);
        bounceTween.setEase(LeanTweenType.easeOutCirc);
        bounceTween.setLoopCount(-1);
        bounceTween.setLoopPingPong();
        bounceTween.setOnUpdate(updateBounceLerp);
        bounceTween.setOnComplete(() => bounceTween = null);
    }

    public void drop(float distance, float height = 0.5f, System.Action onComplete = null) {
        if (dropTween == null) {
            stopBounce();
            dropStartPos = new Vector3(-distance, height);

            coin.transform.localScale = Vector3.zero;
            shadow.transform.localScale = Vector3.zero;
            coin.transform.localPosition = dropStartPos;
            shadow.transform.localPosition = new Vector3(dropStartPos.x, 0);

            dropTween = LeanTween.value(gameObject, 0f, 1f, dropTime);
            dropTween.setEase(dropCurve);
            dropTween.setOnUpdate(updateDropLerp);
            dropTween.setOnComplete(() => {
                dropTween = null;
                bounce();
                onComplete?.Invoke();
            });
        }
    }

    public void spend(float distance, float height = 0.5f, System.Action onComplete = null) {
        if (dropTween == null) {
            shadow.gameObject.SetActive(false);
            boxCollider.gameObject.SetActive(false);
            coinRenderer.sortingOrder = 1;
            stopBounce();
            dropStartPos = new Vector3(-distance, height);

            coin.transform.localScale = Vector3.zero;
            shadow.transform.localScale = Vector3.zero;
            coin.transform.localPosition = dropStartPos;
            shadow.transform.localPosition = new Vector3(dropStartPos.x, 0);

            dropTween = LeanTween.value(gameObject, 0f, 1f, dropTime);
            dropTween.setEase(dropCurve);
            dropTween.setOnUpdate(updateDropLerp);
            dropTween.setOnComplete(() => {
                LeanTween.delayedCall(0.7f, () => {
                    fadeTween = LeanTween.value(gameObject, 0f, 1f, 0.4f);
                    fadeTween.setEaseInExpo();
                    fadeTween.setOnUpdate(updateFadeLerp);
                    dropTween.setOnComplete(() => {
                        onComplete?.Invoke();
                        Destroy(gameObject);
                    });
                });
            });
        }
    }

    public void pickup(Vector3 worldPosition, System.Action onComplete = null) {
        stopBounce();
        dropStartPos = worldPosition - transform.position;
        if (dropTween == null) {
            dropTween = LeanTween.value(gameObject, 1f, 0f, pickupTime);
            dropTween.setEase(LeanTweenType.animationCurve);
            dropTween.setOnUpdate(updateDropLerp);
            dropTween.setOnComplete(() => {
                dropTween = null;
                onComplete?.Invoke();
            });
            coinRenderer.sortingOrder = 1;
        }
    }

    /// Private -- 

    private void updateDropLerp(float dropLerp) {
        this.dropLerp = dropLerp;
        shadow.transform.localScale = Vector3.one * dropLerp;
        coin.transform.localScale = Vector3.one * dropLerp;
        shadow.transform.localPosition = Vector3.Lerp(new Vector3(dropStartPos.x, 0), Vector3.zero, dropLerp);
        coin.transform.localPosition = Vector3.Lerp(dropStartPos, Vector3.zero, dropLerp);
        float y = coin.transform.localPosition.y;
        y += dropCurveY.Evaluate(dropLerp) * 0.4f;
        coin.transform.localPosition = new Vector3(coin.transform.localPosition.x, y);
    }

    private void updateBounceLerp(float bounceLerp) {
        this.bounceLerp = bounceLerp;
        Vector3 animatorPos = coin.transform.localPosition;
        animatorPos.y = bounceLerp * bounceHeight;
        coin.transform.localPosition = animatorPos;
        shadow.transform.localScale = Vector3.one * (1f - bounceLerp * bounceHeight * 2);
    }

    private void updateFadeLerp(float fadeLerp) {
        this.fadeLerp = fadeLerp;
        Vector3 animatorPos = coin.transform.localPosition;
        animatorPos.y = fadeLerp * 2f;
        coin.transform.localPosition = animatorPos;
        coinRenderer.color = coinRenderer.color.setAlpha(1f - bounceLerp);
    }

    private void stopBounce() {
        if (bounceTween != null) {
            LeanTween.cancel(bounceTween.id);
            bounceTween = null;
        }
        bounceLerp = 0;
    }
}