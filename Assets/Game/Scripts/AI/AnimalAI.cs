using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalAI : AIBase {

    /// Protected -- 

    protected override void updateStateMachine() {
        idleRoamUpdate();
    }

}