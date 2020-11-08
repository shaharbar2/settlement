using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeasantPopulationController : MonoBehaviour {
    [SerializeField] private Peasant peasantPrefab;

    private TilemapController tilemapController;

    private List<Peasant> worldPeasants = new List<Peasant>();

    private float loopInterval = 0.1f;
    private float loopElapsed = 0;
    private float spawnInterval = float.MaxValue;

    void Start() {
        tilemapController = FindObjectOfType<TilemapController>();

        // spawnPeasantAt(new Vector3(2.54f, 0.01f, 0f));
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
        if (worldPeasants.Count < Constants.instance.MAX_PEASANTS) {
            if (spawnInterval > Constants.instance.PEASANT_SPAWN_INTERVAL) {
                spawnPeasant();  
                spawnInterval = 0;
            }
        }
    }

    private void spawnPeasant() {
        Peasant peasant = Instantiate(peasantPrefab);
        peasant.transform.position = randomPeasantPosition();
        worldPeasants.Add(peasant);
    }

    private Vector3 randomPeasantPosition() {
        Vector3 pos = Vector3.zero;
        float R = 2.5f; // center radius
        bool isOutsideCenter = false;
        int iterations = 0;
        int maxIterations = 100;
        while(!isOutsideCenter) {
            if (iterations++ > maxIterations) {
                return pos;
            }
            pos = tilemapController.randomPosition(walkable: true);
            isOutsideCenter = Mathf.Abs(pos.x) < R && Mathf.Abs(pos.y) < R;
        }
        return pos;
    }
}