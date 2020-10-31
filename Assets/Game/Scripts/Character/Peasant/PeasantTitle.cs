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
    
    public void setTitle(string type) {
        if (type.ToLower() == "peasant") {
            spriteRenderer.sprite = peasantSprite;
        } else if (type.ToLower() == "hunter") {
            spriteRenderer.sprite = hunterSprite;
        } else if (type.ToLower() == "homeless") {
            spriteRenderer.sprite = homelessSprite;
        }
    }
}