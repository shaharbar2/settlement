using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour {
    [SerializeField] public KeyCode BUILD_MENU_KEY_CODE;
    [SerializeField] public float BUILD_MENU_KEY_HOLD;
    [SerializeField] public float BUILD_MENU_SHOW_DURATION;
    [SerializeField] public float BUILD_MENU_HIDE_DURATION;
    [SerializeField] public LeanTweenType BUILD_MENU_SHOW_EASE;
    [SerializeField] public LeanTweenType BUILD_MENU_HIDE_EASE;

    public static Constants instance;
    
    void Awake() {
        instance = this;
    }
}