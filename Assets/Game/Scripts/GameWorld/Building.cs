using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingState {
    AwaitingConstruction,
    AwaitingRepairs,
    UnderConstruction,
    Constructed,
    Destroyed
}

public class Building : MonoBehaviour {
    
    /// Public -- 
    
    [SerializeField] public BuildingType type;
    [SerializeField] public BuildingState state;

    [HideInInspector] public bool buildOnStart;
    [HideInInspector] public bool instantBuild;

    public delegate void BuildingEvent(Building building);
    public event BuildingEvent onDestroyed;

    public bool isUsableState {
        get {
            return state == BuildingState.Constructed || state == BuildingState.AwaitingRepairs;
        }
    }
    public GameObject collisionZoneGameObject { get {
        return collisionZone.gameObject;
    }}

    /// Private -- 

    [SerializeField] private GameObject collisionZone;
    [SerializeField] private GameObject dustVFXPrefab;
    [SerializeField] private GameObject bowPrefab;
    [SerializeField] private Transform coinsAnchor;
    [SerializeField] private Transform weaponAnchor;

    [SerializeField] private HitpointsBar hitpointsBar;

    [SerializeField] private SpriteRenderer groundSpriteRenderer;
    [SerializeField] private SpriteRenderer buildingSpriteRenderer;
    [SerializeField] private SpriteRenderer shadowSpriteRenderer;

    private float hp;
    private BuildingData data;

    void Awake() {
        buildingSpriteRenderer.gameObject.SetActive(false);
        shadowSpriteRenderer.gameObject.SetActive(false);
    }

    void Start() {
        data = BuildingConfiguration.instance.buildingDataFor(type);
        hp = (float)data.hitpoints;
        hitpointsBar.gameObject.SetActive(false);
        if (buildOnStart) {
            build();
        }
    }

    void Update() {
        if (state == BuildingState.Constructed && hp < data.hitpoints) {
            state = BuildingState.AwaitingRepairs;
        }
        if (state == BuildingState.AwaitingRepairs && hp >= data.hitpoints) {
            state = BuildingState.Constructed;
        }
    }

    /// Public --

    public void build(System.Action<bool> onComplete = null) {
        if (instantBuild) {
            completeConstruction();
            onComplete?.Invoke(true);
        } else {
            state = BuildingState.UnderConstruction;
            animateDust(amount: 15, duration: 1.5f);
            LeanTween.delayedCall(1.5f, () => {
                completeConstruction();
                onComplete?.Invoke(true);
            });
        }
    }

    public Vector3 getCoinsAnchor() {
        return coinsAnchor.transform.position;
    }

    public Vector3 getWeaponAnchor() {
        return weaponAnchor.transform.position;
    }

    public string getInteractHint() {
        string keyCode = Constants.instance.COIN_KEY_CODE.ToString();
        switch (type) {
            case BuildingType.BowShop:
                return $"Press {keyCode} to buy a {WeaponType.Bow}";
            case BuildingType.HammerShop:
                return $"Press {keyCode} to buy a {WeaponType.Hammer}";
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
        buildingSpriteRenderer.gameObject.SetActive(true);
        shadowSpriteRenderer.gameObject.SetActive(true);
        state = BuildingState.Constructed;
    }

    private void onBuildingDestroyed() {
        state = BuildingState.Destroyed;
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