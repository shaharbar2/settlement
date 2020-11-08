using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TreeState {
    Ready,
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
        animateDust(amount: 3, duration: 0.2f);
        LeanTween.delayedCall(1.5f, () => {
            if (hp <= 0) {
                completeLumering();
                onChoppedDown?.Invoke(this);
            }
            onComplete?.Invoke(true);
        });
    }

    public Vector3 getCoinsAnchor() {
        return coinsAnchor.transform.position;
    }


    public string getInteractHint() {
        KeyCode keyCode = Constants.instance.COIN_KEY_CODE;
         return $"Press {keyCode} to order lumbering";
    }


    /// Private --

    private void buildInstantly() {
        completeLumering();
    }

    private void completeLumering() {
        LTDescr tween = LeanTween.rotateZ(trunkSpriteRenderer.gameObject, -90, 1f);
        tween.setEaseOutElastic();
        tween.setOnComplete(() => {
            LTDescr fadeTween = LeanTween.alpha(trunkSpriteRenderer.gameObject, 0, 1f);
        });
        state = TreeState.Chopped;
    }

    private void animateDust(int amount, float duration) {
        for (int i = 0; i < amount; i++) {
            float rMin = 0.1f;
            float rMax = 0.3f;
            float t = Random.Range(0f, duration);
            LeanTween.delayedCall(t, () => {
                GameObject vfx = Instantiate(dustVFXPrefab);
                float yOffset = Random.Range(-rMin * 4f, rMin * 2f);
                float xOffset = rMin + Random.Range(0f, rMax);
                if (BabyUtils.randomBool) {
                    vfx.transform.localScale = new Vector3(-1 * vfx.transform.localScale.x, vfx.transform.localScale.y, 1);
                    vfx.transform.position = new Vector3(transform.position.x - xOffset, transform.position.y + yOffset);
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