using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TreeState {
    Ready,
    MarkedForChop,
    AssignedForChop,
    Chopped
}

public class Tree : MonoBehaviour {
    /// Public -- 

    [SerializeField] public TreeState state;

    public delegate void TreeEvent(Tree tree);
    public event TreeEvent onChoppedDown;
    /// Private -- 
    [SerializeField] private float hp;

    [SerializeField] private GameObject collisionZone;
    [SerializeField] private GameObject dustVFXPrefab;
    [SerializeField] private Transform coinsAnchor;

    [SerializeField] private SpriteRenderer rootSpriteRenderer;
    [SerializeField] private SpriteRenderer trunkSpriteRenderer;
    [SerializeField] private SpriteRenderer shadowSpriteRenderer;

    [SerializeField] private Sprite[] roots;
    [SerializeField] private Sprite[] trunks;
    [SerializeField] private Sprite[] shadows;

    private BuildingData data;

    public GameObject collisionZoneGameObject {
        get {
            return collisionZone.gameObject;
        }
    }

    void Awake() {
        int idx = Random.Range(0, roots.Length);
        rootSpriteRenderer.sprite = roots[idx];
        trunkSpriteRenderer.sprite = trunks[idx];
        shadowSpriteRenderer.sprite = shadows[idx];
    }

    void Start() {

    }

    void Update() {

    }

    /// Public --

    public void chop(System.Action<bool> onComplete = null) {
        hp -= 1;
        bool dead = hp <= 0;

        animateChop();

        animateDust(amount: 2, duration: 0.2f);
        LeanTween.delayedCall(1.5f, () => {
            if (hp <= 0) {
                completeLumering();
                onChoppedDown?.Invoke(this);
            }
            onComplete?.Invoke(dead);
        });
    }

    public Vector3 getCoinsAnchor() {
        return coinsAnchor.transform.position;
    }

    public string getInteractHint() {
        KeyCode keyCode = Constants.instance.COIN_KEY_CODE;
        return $"Press {keyCode} to order lumbering";
    }

    public void mark() {
        state = TreeState.MarkedForChop;
    }

    /// Private --

    private void buildInstantly() {
        completeLumering();
    }

    private void completeLumering() {
        LTDescr tween = LeanTween.rotateZ(trunkSpriteRenderer.gameObject, -60, 3f);
        tween.setEaseOutBounce();
        tween.setOnComplete(() => {
            LTDescr fadeTween = LeanTween.alpha(trunkSpriteRenderer.gameObject, 0, 1f);
        });
        LeanTween.delayedCall(1f, () => {
            animateDust(4, 0.1f, 1.5f);
            dropCoin();
            dropCoin();
        });
        state = TreeState.Chopped;
    }

    private void animateChop() {
        LTDescr tween = LeanTween.rotateZ(trunkSpriteRenderer.gameObject, -2, 0.4f);
        tween.setEaseOutElastic();
        tween.setOnComplete(() => {
            tween = LeanTween.rotateZ(trunkSpriteRenderer.gameObject, 0, 0.4f);
            tween.setEaseOutElastic();
        });
    }

     private void dropCoin() {
        var coinController = FindObjectOfType<CoinController>();
        float direction = BabyUtils.randomBool ? 1 : -1;
        coinController.dropCoin(transform.position, direction, CoinDropType.ByAnimal);
    }

    private void animateDust(int amount, float duration, float rMax = 0f) {
        for (int i = 0; i < amount; i++) {
            float rMin = 0f;
            float t = Random.Range(0f, duration);
            LeanTween.delayedCall(t, () => {
                GameObject vfx = Instantiate(dustVFXPrefab);
                float yOffset = Random.Range(-rMin * 1f, rMin * 1f) + 0.2f;
                float xOffset = rMin + Random.Range(0f, rMax);
                if (BabyUtils.randomBool) {
                    vfx.transform.localScale = new Vector3(-1 * vfx.transform.localScale.x, vfx.transform.localScale.y, 1);
                    vfx.transform.position = new Vector3(transform.position.x + xOffset, transform.position.y + yOffset);
                } else {
                    vfx.transform.position = new Vector3(transform.position.x + xOffset, transform.position.y + yOffset);
                }
                vfx.transform.localScale *= Random.Range(0.5f, 1f);
                LeanTween.delayedCall(1.3f, () => {
                    Destroy(vfx);
                });
            });
        }
    }
}