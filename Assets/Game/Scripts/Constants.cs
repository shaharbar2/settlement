using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerControlStyle {
    WASD,
    POINTCLICK,
    POINTCLICK_FREECAMERA
}

public class Constants : MonoBehaviour {
    [Header("Player Settings")]
    [SerializeField] public PlayerControlStyle PLAYER_CONTROL_STYLE;
    [SerializeField] public float PLAYER_SPEED;

    [Header("Build Menu")]
    [SerializeField] public KeyCode BM_KEY_CODE;
    [SerializeField] public float BM_KEY_HOLD;
    [SerializeField] public float BM_TIME_SHOW;
    [SerializeField] public float BM_TIME_HIDE;
    [SerializeField] public float BM_TIME_CHILD_SHOW;
    [SerializeField] public float BM_TIME_CHILD_HIDE;
    [SerializeField] public float BM_TIME_HIGHLIGHT;
    [SerializeField] public LeanTweenType BM_SHOW_EASE;
    [SerializeField] public LeanTweenType BM_HIDE_EASE;
    [SerializeField] public float BM_TIME_CLICKED_CHILD_HIDE;
    [SerializeField] public LeanTweenType BM_CLICKED_CHILD_HIDE_EASE;
    [SerializeField] public Color BM_BASE_COLOR;
    [SerializeField] public Color BM_SELECTED_COLOR;

    public static Constants instance;
    
    void Awake() {
        instance = this;
    }
}
