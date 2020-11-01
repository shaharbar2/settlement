using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType {
    Bow
}
public class Weapon : MonoBehaviour {
    public WeaponType type; 

    private SpriteRenderer spriteRenderer;
    private Vector3 originalLocalPos;

    void Awake() {
        spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        originalLocalPos = transform.localPosition;
    }

    /// Public -- 
    
    public void drop(Vector3 position) {
        spriteRenderer.sortingOrder = 1;
        spriteRenderer.color = Color.white.setAlpha(0);
        float dropDistance = 0.6f;
        transform.position = position;
        spriteRenderer.transform.localPosition = originalLocalPos + new Vector3(0, dropDistance);
        LeanTween.value(gameObject, 0f, 1f, 1.2f).setOnUpdate((float lerp) => {
            spriteRenderer.color = spriteRenderer.color.setAlpha(lerp);
            Vector3 pos = spriteRenderer.transform.localPosition;
            pos.y = originalLocalPos.y + dropDistance * (1f - lerp);
            spriteRenderer.transform.localPosition = pos;
        }).setOnComplete(() => {
            spriteRenderer.sortingOrder = 0;
        }).setEaseOutBounce();
    }
    
    public void pickup(System.Action onComplete = null) {
        spriteRenderer.sortingOrder = 1;
        spriteRenderer.color = Color.white.setAlpha(1);
        float yDistance = 0.6f;
        Vector3 fromPos = spriteRenderer.transform.localPosition;
        Vector3 toPos = spriteRenderer.transform.localPosition + new Vector3(0, yDistance);
        LeanTween.value(gameObject, 0f, 1f, 1.2f).setOnUpdate((float lerp) => {
            spriteRenderer.color = spriteRenderer.color.setAlpha(1f - lerp);
            spriteRenderer.transform.localPosition = Vector3.Lerp(fromPos, toPos, lerp);
        }).setOnComplete(() => {
            onComplete?.Invoke();            
        }).setEaseOutCubic();
    }
}