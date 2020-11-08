using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPrefab : MonoBehaviour {
    [SerializeField] public BuildingType type;

    [SerializeField] private GameObject collisionZone;
    [SerializeField] private GameObject dustVFXPrefab;
    [SerializeField] private GameObject bowPrefab;
    [SerializeField] private Transform coinsAnchor;
    [SerializeField] private Transform weaponAnchor;

    [SerializeField] private SpriteRenderer groundSpriteRenderer;
    [SerializeField] private SpriteRenderer buildingSpriteRenderer;
    [SerializeField] private SpriteRenderer shadowSpriteRenderer;

    [HideInInspector] public bool buildOnStart;
    [HideInInspector] public bool instantBuild;

    void Awake() {
        buildingSpriteRenderer.gameObject.SetActive(false);
        shadowSpriteRenderer.gameObject.SetActive(false);
        collisionZone.SetActive(false);
    }

    void Start() {
        if (buildOnStart) { 
            build();
        }
    }

    /// Public --

    public void build() {
        if (instantBuild) {
            completeConstruction();
        } else {
            LeanTween.delayedCall(1.5f, completeConstruction);
            animateDust(amount: 15, duration: 1.5f);
        }
    }

    public Vector3 getCoinsAnchor() {
        return coinsAnchor.transform.position;
    }

    public Vector3 getWeaponAnchor() {
        return weaponAnchor.transform.position;
    }

    /// Private --

    private void buildInstantly() {
        completeConstruction();
    }

    private void completeConstruction() {
        buildingSpriteRenderer.gameObject.SetActive(true);
        shadowSpriteRenderer.gameObject.SetActive(true);
        collisionZone.SetActive(true);
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