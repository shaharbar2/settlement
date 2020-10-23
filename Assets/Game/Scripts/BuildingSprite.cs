using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSprite : MonoBehaviour {

    [SerializeField] Texture2D textureSite;
    [SerializeField] Texture2D textureBulding;
    [SerializeField] GameObject dustVFXPrefab;

    [HideInInspector] public bool isBuilt;

    private SpriteRenderer spriteRenderer;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// Public --

    public void build() {
        BabyUtils.ExecuteAfterTime(this, 1.5f, () => {
            setTexture(textureBulding);
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
        isBuilt = true;
    }

    /// Private --

    private void setTexture(Texture2D tex) {
        spriteRenderer.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
    }
}