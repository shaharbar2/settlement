using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitpointsBar : MonoBehaviour {

    [SerializeField] private SpriteRenderer back;
    [SerializeField] private SpriteRenderer front;

    void Start() {

    }

    void Update() {

    }

    /// Public -- 
    
    public void update(float current, float total) {
        Vector3 scale = front.transform.localScale;
        scale.x = current/total;
        front.transform.localScale = scale;
    }
}