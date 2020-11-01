using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalPopulationController : MonoBehaviour {
    [SerializeField] private Animal animalPrefab;

    private TilemapController tilemapController;

    private List<Animal> worldAnimals = new List<Animal>();

    private float loopInterval = 0.1f;
    private float loopElapsed = 0;
    private float spawnInterval = 0;

    void Start() {
        tilemapController = FindObjectOfType<TilemapController>();
    }

    void Update() {
        spawnInterval += Time.deltaTime;
        loopElapsed += Time.deltaTime;
        if (loopElapsed >= loopInterval) {
            loopElapsed -= loopInterval;
            controlPopulation();
        }
    }

    /// Private -- 
    
    private void controlPopulation() {
        if (worldAnimals.Count < Constants.instance.MAX_ANIMALS) {
            if (spawnInterval < Constants.instance.ANIMALS_SPAWN_INTERVAL) {
                spawnAnimal();  
            }
        }
    }

    private void spawnAnimal() {
        Animal animal = Instantiate(animalPrefab);
        animal.transform.position = randomAnimalPosition();
        worldAnimals.Add(animal);
        animal.onDeath += onAnimalDeath;
    }

    private void onAnimalDeath(Animal animal) {
        worldAnimals.Remove(animal);
        spawnInterval = 0;
    }

    private Vector3 randomAnimalPosition() {
        Vector3 pos = Vector3.zero;
        float R = 1.5f; // center radius
        bool isOutsideCenter = false;
        int iterations = 0;
        int maxIterations = 100;
        while(!isOutsideCenter) {
            if (iterations++ > maxIterations) {
                return pos;
            }
            pos = tilemapController.randomPosition(walkable: true);
            isOutsideCenter = Mathf.Abs(pos.x) > R && Mathf.Abs(pos.y/2f) > R;
        }
        return pos;
    }
}