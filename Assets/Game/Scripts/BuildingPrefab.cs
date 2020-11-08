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

    [SerializeField] private HitpointsBar hitpointsBar;

    [SerializeField] private SpriteRenderer groundSpriteRenderer;
    [SerializeField] private SpriteRenderer buildingSpriteRenderer;
    [SerializeField] private SpriteRenderer shadowSpriteRenderer;

    [HideInInspector] public bool buildOnStart;
    [HideInInspector] public bool instantBuild;

    public delegate void BuildingEvent(BuildingPrefab building);
    public event BuildingEvent onDestroyed;

    private float hp;
    private BuildingData data;

    void Awake() {
        buildingSpriteRenderer.gameObject.SetActive(false);
        shadowSpriteRenderer.gameObject.SetActive(false);
        collisionZone.SetActive(false);
    }

    void Start() {
        data = BuildingConfiguration.instance.buildingDataFor(type);
        hp = (float)data.hitpoints;
        hitpointsBar.gameObject.SetActive(false);
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

    public string getInteractHint() {
        switch(type) {
            case BuildingType.BowShop: return $"Press {Constants.instance.COIN_KEY_CODE} to buy a {WeaponType.Bow}";
            case BuildingType.HammerShop: return $"Press {Constants.instance.COIN_KEY_CODE} to buy a {WeaponType.Hammer}";
        }
        return null;
    }
    public void hit(float damage) {
        hp -= damage;
        hitpointsBar.update(hp, data.hitpoints);
        hitpointsBar.gameObject.SetActive(hp < data.hitpoints);

        if (hp <= 0) {
            hp = 0;
            onBuildingDestroyed();
        }
    }

    /// Private --

    private void buildInstantly() {
        completeConstruction();
    }

    private void completeConstruction() {
        Debug.Log("constuction completed");
        buildingSpriteRenderer.gameObject.SetActive(true);
        shadowSpriteRenderer.gameObject.SetActive(true);
        collisionZone.SetActive(true);
    }

    private void onBuildingDestroyed() {
        onDestroyed?.Invoke(this);
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