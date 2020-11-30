using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banner : MonoBehaviour {
    private Squad squad;

    void Start() {
        squad = Squad.create(leader: this);
        Peasant[] peasants = FindObjectsOfType<Peasant>();
        foreach (Peasant peasant in peasants) {
            squad.addUnit(peasant);
        }
        squad.setMode(SquadMode.Forming);
    }

    // Update is called once per frame
    void Update() {

    }

    public Squad takeOverSquad() {
        return squad;
    }
}