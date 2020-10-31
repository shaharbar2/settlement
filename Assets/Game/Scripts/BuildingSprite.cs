using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSprite : MonoBehaviour {
    [SerializeField] private Sprite spriteSite;
    [SerializeField] private Sprite spriteBuilding;
    [SerializeField] private Sprite spriteShadow;
    [SerializeField] private GameObject collisionZone;
    [SerializeField] private GameObject dustVFXPrefab;
    [SerializeField] private GameObject bowPrefab;
    [SerializeField] private Transform coinsAnchor;
    [SerializeField] private Transform dropAnchor;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer shadowSpriteRenderer;

    void Awake() {
        shadowSpriteRenderer.sprite = null;
        collisionZone.SetActive(false);
    }

    /// Public --

    public void build() {
        BabyUtils.ExecuteAfterTime(this, 1.5f, () => {
            spriteRenderer.sprite = spriteBuilding;
            shadowSpriteRenderer.sprite = spriteShadow;
            collisionZone.SetActive(true);
        });
        for (int i = 0; i < 15; i++) {
            float rMin = 0.1f;
            float rMax = 0.3f;
            float t = Random.Range(0f, 1.5f);
            BabyUtils.ExecuteAfterTime(this, t, () => {
                GameObject vfx = Instantiate(dustVFXPrefab);

                float yOffset = Random.Range(-rMin, rMin);
                float xOffset = rMin + Random.Range(0f, rMax);
                if (BabyUtils.randomBool()) {
                    vfx.transform.localScale = new Vector3(-1 * vfx.transform.localScale.x, vfx.transform.localScale.y, 1);
                    vfx.transform.position = new Vector3(transform.position.x - xOffset, transform.position.y + yOffset);
                } else {
                    vfx.transform.position = new Vector3(transform.position.x + xOffset, transform.position.y + yOffset);
                }
                vfx.transform.localScale *= Random.Range(0.5f, 1f);
                BabyUtils.ExecuteAfterTime(this, 0.9f, () => {
                    Destroy(vfx);
                });
            });
        }
        spriteRenderer.sortingLayerName = "Objects";
    }

    public Vector3 getCoinsAnchor() {
        return coinsAnchor.transform.position;
    }

    public void dropBow() {
        GameObject bow = Instantiate(bowPrefab);
        SpriteRenderer spriteRenderer = bow.GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sortingOrder = 1;
        spriteRenderer.color = Color.white.setAlpha(0);
        float dropDistance = 0.6f;
        bow.transform.position = dropAnchor.transform.position + new Vector3(0, dropDistance);
        LeanTween.value(bow, 0f, 1f, 1.2f).setOnUpdate((float lerp) => {
            spriteRenderer.color = spriteRenderer.color.setAlpha(lerp);
            Vector3 pos = bow.transform.position;
            pos.y = dropAnchor.transform.position.y + dropDistance * (1f -lerp);
            bow.transform.position = pos;
        }).setOnComplete(() => {
            spriteRenderer.sortingOrder = 0;
        }
        ).setEaseOutBounce();
    }
    /// Private --

}