using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObjectFinder {
    private GameObject gameObject;

    private WeaponController weaponController;
    private CoinController coinController;

    // Caching 
    private Player cachedPlayer;

    /// Initialization -- 

    public WorldObjectFinder(GameObject gameObject) {
        this.gameObject = gameObject;
    }

    public void inject(CoinController coinController, WeaponController weaponController) {
        this.coinController = coinController;
        this.weaponController = weaponController;
    }

    /// Public -- 

    public Player player() {
        if (cachedPlayer == null) {
            cachedPlayer = GameObject.FindObjectOfType<Player>();
        }
        return cachedPlayer;
    }

    public Animal closestAliveAnimal(float radius) {
        Animal[] allAnimals = GameObject.FindObjectsOfType<Animal>();
        Animal closest = null;
        float closestRange = float.MaxValue;
        foreach (Animal animal in allAnimals) {
            if (!animal.isAlive)continue;
            float r = Vector2.Distance(animal.transform.position, gameObject.transform.position);
            if (r < radius) {
                if (r < closestRange) {
                    closestRange = r;
                    closest = animal;
                }
            }
        }
        return closest;
    }

    public BuildingPrefab closestConstructionJob(float radius) {
        return closestBuilding(radius, BuildingState.AwaitingConstruction);
    }

    public BuildingPrefab closestRepairJob(float radius) {
        return closestBuilding(radius, BuildingState.AwaitingRepairs);
    }

    private BuildingPrefab closestBuilding(float radius, BuildingState state) {
        BuildingPrefab[] allBuildings = GameObject.FindObjectsOfType<BuildingPrefab>();
        BuildingPrefab closest = null;
        float closestRange = float.MaxValue;
        foreach (BuildingPrefab building in allBuildings) {
            if (building.state != state) continue;
            float r = Vector2.Distance(building.transform.position, gameObject.transform.position);
            if (r < radius) {
                if (r < closestRange) {
                    closestRange = r;
                    closest = building;
                }
            }
        }
        return closest;
    }

}