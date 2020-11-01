using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeasantTitle : MonoBehaviour {
    [SerializeField] private Sprite peasantSprite;
    [SerializeField] private Sprite homelessSprite;
    [SerializeField] private Sprite hunterSprite;

    private SpriteRenderer spriteRenderer;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update() {
        float x = Mathf.Abs(transform.localScale.x) * Mathf.Sign(transform.parent.localScale.x);
        transform.localScale = new Vector3(x, transform.localScale.y, 1);
    }
    /// Public -- 
    
    public void updateTitle(NPCType type) {
        switch(type) {
            case NPCType.Peasant:
                spriteRenderer.sprite = peasantSprite;
                break;
            case NPCType.Homeless:
                spriteRenderer.sprite = homelessSprite;
                break;
            case NPCType.Hunter:
                spriteRenderer.sprite = hunterSprite;
                break;
        }
    }
}