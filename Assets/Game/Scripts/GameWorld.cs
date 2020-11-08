using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameWorld : MonoBehaviour {
    [SerializeField] public Building tent;

    void Start() {
        tent.onDestroyed += onTentDestroyed;
    }

    void Update() {

    }

    private void onTentDestroyed(Building building) {
        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(scene.name);
    }
}