using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingType {
    Undefined,
    Tent,
    BowShop,
    HammerShop,

    // Fortifications
    Fence,
    GuardTower
}

public class BuildingData {
    public BuildingType type;
    public string name;
    
    public TilemapAreaType areaType;
    
    public int costToConstruct;
    public int costToUse;

    public int hitpoints;

    public Building prefab;
}