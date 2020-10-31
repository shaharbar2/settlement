﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerControlStyle {
    WASD,
    POINTCLICK
}
public enum CameraControlStyle {
    PAN_MOUSE,
    PAN_WASD,
    FOLLOW    
}
public enum BuildingStyle {
    MOUSE_SELECTION,
    AUTOMATIC
}
public class Constants : MonoBehaviour {
    [Header("Player Settings")]
    [SerializeField] public PlayerControlStyle PLAYER_CONTROL_STYLE;
    [SerializeField] public CameraControlStyle CAMERA_CONTROL_STYLE;
    [SerializeField] public BuildingStyle BUILDING_STYLE;
    [SerializeField] public float CAMERA_SPEED;
    [SerializeField] public float PLAYER_SPEED;
    [SerializeField] public float PEASANT_SPEED;
    
    [Header("Keybinds")]
    [SerializeField] public KeyCode BM_KEY_CODE;
    [SerializeField] public KeyCode COIN_KEY_CODE;

    [Header("Real Estate prices (in coins)")]
    [SerializeField] public int COST_BOW_SHOP; 

    [Header("Build Menu")]
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
