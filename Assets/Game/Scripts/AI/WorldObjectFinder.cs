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

    public Building closestConstructionJob(float radius) {
        return closestBuilding(radius, BuildingState.AwaitingConstruction);
    }

    public Building closestRepairJob(float radius) {
        return closestBuilding(radius, BuildingState.AwaitingRepairs);
    }
    
    public Tree closestLumberingJob(float radius) {
        return closestTree(radius, TreeState.MarkedForChop);
    }

    /// Private -- 
    
    private Building closestBuilding(float radius, BuildingState state) {
        Building[] allBuildings = GameObject.FindObjectsOfType<Building>();
        Building closest = null;
        float closestRange = float.MaxValue;
        foreach (Building building in allBuildings) {
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

    private Tree closestTree(float radius, TreeState state) {
        Tree[] allTrees = GameObject.FindObjectsOfType<Tree>();
        Tree closest = null;
        float closestRange = float.MaxValue;
        foreach (Tree tree in allTrees) {
            if (tree.state != state) continue;
            float r = Vector2.Distance(tree.transform.position, gameObject.transform.position);
            if (r < radius) {
                if (r < closestRange) {
                    closestRange = r;
                    closest = tree;
                }
            }
        }
        return closest;
    }


}