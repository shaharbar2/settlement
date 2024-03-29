﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSettingsMenu : MonoBehaviour {
    [SerializeField] public Dropdown dropdownMovement;
    [SerializeField] public Dropdown dropdownCamera;
    [SerializeField] public Dropdown dropdownBuild;
    [SerializeField] public Button closeButton;
    [SerializeField] public TMP_Text versionText;

    public delegate void PlayerSettingsMenuEvent();
    public event PlayerSettingsMenuEvent onClose;

    void Start() {
        dropdownMovement.onValueChanged.AddListener(onDropdownMovementValueChanged);
        dropdownMovement.value = (int)Constants.instance.PLAYER_CONTROL_STYLE;
        dropdownCamera.onValueChanged.AddListener(onDropdownCameraValueChanged);
        dropdownCamera.value = (int)Constants.instance.CAMERA_CONTROL_STYLE;
        dropdownBuild.onValueChanged.AddListener(onDropdownBuildValueChanged);
        dropdownBuild.value = (int)Constants.instance.BUILDING_STYLE;
        closeButton.onClick.AddListener(onCloseButtonClick);
        versionText.text = "build " + Application.version;
    }

    // Update is called once per frame
    void Update() {

    }

    /// Private -- 
    
    private void onDropdownMovementValueChanged(int value) {
        Constants.instance.PLAYER_CONTROL_STYLE = (PlayerControlStyle)value;
    }

    private void onDropdownCameraValueChanged(int value) {
        Constants.instance.CAMERA_CONTROL_STYLE = (CameraControlStyle)value;
    }

    private void onDropdownBuildValueChanged(int value) {
        Constants.instance.BUILDING_STYLE = (BuildingStyle)value;
    }

    private void onCloseButtonClick() {
        this.gameObject.SetActive(false);
        onClose?.Invoke();
    }
}