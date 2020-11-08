using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSceneLoad : MonoBehaviour {

    private TilemapController tilemapController;
    private Building[] defaultBuildings;

    void Awake() {
        tilemapController = FindObjectOfType<TilemapController>();

        manageDefaultBuildingsAwake();
    }

    void Start() {
        manageDefaultBuildingsStart();
    }

    /// Private -- 
    
    private void manageDefaultBuildingsAwake() {
        defaultBuildings = FindObjectsOfType<Building>();
        foreach(var building in defaultBuildings) {
            building.buildOnStart = true;
            building.instantBuild = true;
            building.transform.position = tilemapController.snap(building.transform.position);
        }
    }

    private void manageDefaultBuildingsStart() {
        foreach(var building in defaultBuildings) {
            BuildingData buildingData = BuildingConfiguration.instance.buildingDataFor(building.type);
            tilemapController.markUnwalkable(building.transform.position, buildingData.areaType);
        }
    }
}