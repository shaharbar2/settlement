using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingConfiguration : MonoBehaviour { 
    [HideInInspector] public BuildingData[] data; 

    static public BuildingConfiguration instance;

    void Awake() {
        instance = this;

        var config = GetComponentsInChildren<BuildingConfigurationData>();
        data = new BuildingData[config.Length];
        for (int i = 0; i < config.Length; i++) {
            data[i] = config[i].data;
        }
    }    
    
    /// Public -- 
    
    public BuildingData buildingDataFor(BuildingType type) {
        foreach(BuildingData buildingData in data) {
            if (buildingData.type == type)
                return buildingData;
        }
        // ToDo: this seems wrong
        if (type == BuildingType.Undefined) {
            return new BuildingData();
        }
        throw new System.Exception($"No building data for type {type.ToString()} found in BuildingConguration.");
    }
}