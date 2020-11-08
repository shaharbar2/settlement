using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameWorld : MonoBehaviour {
    [SerializeField] public BuildingPrefab tent;

    void Start() {
        tent.onDestroyed += onTentDestroyed;
    }

    void Update() {

    }

    private void onTentDestroyed(BuildingPrefab building) {
        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(scene.name);
    }
}