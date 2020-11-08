using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingConfigurationData : MonoBehaviour {
    [SerializeField] private BuildingType type;
    [SerializeField] private TilemapAreaType areaType;
    [SerializeField] private int costToConstruct;
    [SerializeField] private int costToUse;
    [SerializeField] private int hitpoints;
    [SerializeField] private BuildingPrefab prefab;

    public BuildingData data { get{
        var data = new BuildingData();
        data.type = type;
        data.areaType = areaType;
        data.costToConstruct = costToConstruct;
        data.costToUse = costToUse;
        data.prefab = prefab;
        data.name = name;
        data.hitpoints = hitpoints;
        return data;
    }}
}