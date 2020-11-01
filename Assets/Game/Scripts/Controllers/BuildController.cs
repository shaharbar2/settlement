﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildController : MonoBehaviour {
    internal enum BuildControllerState {
        IDLE,
        SELECTING_BUILDING,
        SELECTING_SPOT
    }

    [SerializeField] private BuildingSprite buildingPrefab;

    private GameUI ui;

    private TilemapController tilemapController;
    private Player player;

    private float radialMenuHoldElapsed = 0f;
    private BuildControllerState state = BuildControllerState.IDLE;

    private RadialMenuSegmentData highlightedBuildingData;
    private Vector3 selectedSpot;

    void Start() {
        ui = FindObjectOfType<GameUI>();
        player = FindObjectOfType<Player>();
        tilemapController = FindObjectOfType<TilemapController>();
        ui.radialMenu.onClicked += onBuildingSelected;
        ui.radialMenu.onHighlighted += onBuildingHighlighted;
    }

    // Update is called once per frame
    void Update() {
        handleRadialMenuInput();
        handleBuildingHighlight();

        if (Input.GetMouseButtonDown(0)) {
            if (state == BuildControllerState.SELECTING_SPOT) {
                buildAtSelectedSpot();
                unblockPlayerInput();
            }
        }
    }

    /// Private -- 
    private void blockPlayerInput() {
        ui.blockPlayerMouseMovement = true;
    }

    private void unblockPlayerInput() {
        LeanTween.delayedCall(0.5f, () => {
            ui.blockPlayerMouseMovement = false;
        });
    }

    private void handleRadialMenuInput() {
        KeyCode keyCode = Constants.instance.BM_KEY_CODE;
        if (Input.GetKeyDown(keyCode)) {
            radialMenuHoldElapsed = 0;
        }

        if (Input.GetKey(keyCode)) {
            radialMenuHoldElapsed += Time.deltaTime;
            if (radialMenuHoldElapsed >= Constants.instance.BM_KEY_HOLD) {
                blockPlayerInput();
                ui.showRadialMenu();
            }
        }

        if (Input.GetKeyUp(keyCode)) {
            unblockPlayerInput();
            ui.hideRadialMenu();
        }
    }

    private void handleBuildingHighlight() {
        if (highlightedBuildingData != null) {
            if (state == BuildControllerState.SELECTING_BUILDING) {
                selectedSpot = player.transform.position;
            } else if (state == BuildControllerState.SELECTING_SPOT) {
                selectedSpot = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                selectedSpot.z = 0;
            }
            tilemapController.highlightForBuild(selectedSpot, highlightedBuildingData.areaType);
        } else {
            tilemapController.removeHighlightForBuild();
        }
    }

    private void buildAtSelectedSpot() {
        BuildingSprite building = Instantiate(buildingPrefab);
        building.transform.position = tilemapController.snap(selectedSpot);
        building.build();
        tilemapController.markUnwalkable(building.transform.position, highlightedBuildingData.areaType);
        tilemapController.removeHighlightForBuild();

        foreach (var movement in FindObjectsOfType<CharacterMovement>()) {
            movement.leaveBuildArea();
        }

        state = BuildControllerState.IDLE;
        highlightedBuildingData = null;
    }

    private void onBuildingHighlighted(RadialMenuSegmentData data) {
        highlightedBuildingData = data;
        state = BuildControllerState.SELECTING_BUILDING;
    }

    private void onBuildingSelected(RadialMenuSegmentData data) {
        highlightedBuildingData = data;
        if (data != null) {
            if (Constants.instance.BUILDING_STYLE == BuildingStyle.AUTOMATIC) {
                buildAtSelectedSpot();
                state = BuildControllerState.IDLE;
            } else {
                LeanTween.delayedCall(0.1f, () => {
                    state = BuildControllerState.SELECTING_SPOT;
                });
            }
        } else {
            state = BuildControllerState.IDLE;
        }
    }
}