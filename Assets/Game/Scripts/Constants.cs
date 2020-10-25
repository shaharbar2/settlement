using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour {
    [SerializeField] public KeyCode BUILD_MENU_KEY_CODE;
    [SerializeField] public float BUILD_MENU_KEY_HOLD;
    [SerializeField] public float BUILD_MENU_TIME_SHOW;
    [SerializeField] public float BUILD_MENU_TIME_HIDE;
    [SerializeField] public float BUILD_MENU_TIME_CHILD_SHOW;
    [SerializeField] public float BUILD_MENU_TIME_CHILD_HIDE;
    [SerializeField] public float BUILD_MENU_TIME_HIGHLIGHT;
    [SerializeField] public LeanTweenType BUILD_MENU_SHOW_EASE;
    [SerializeField] public LeanTweenType BUILD_MENU_HIDE_EASE;
    [SerializeField] public Color BUILD_MENU_BASE_COLOR;
    [SerializeField] public Color BUILD_MENU_SELECTED_COLOR;

    public static Constants instance;
    
    void Awake() {
        instance = this;
    }
}