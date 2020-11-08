﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeasantTitle : MonoBehaviour {
    [SerializeField] private Sprite peasantSprite;
    [SerializeField] private Sprite vagabondSprite;
    [SerializeField] private Sprite workerSprite;
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
            case NPCType.Vagabond:
                spriteRenderer.sprite = vagabondSprite;
                break;
            case NPCType.Hunter:
                spriteRenderer.sprite = hunterSprite;
                break;
            case NPCType.Worker:
                spriteRenderer.sprite = workerSprite;
                break;
        }
    }
}