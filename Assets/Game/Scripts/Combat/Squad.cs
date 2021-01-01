using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SquadUpdateType {
    LeaderUpdate,
    ModeUpdate,
    UnitAdded,
    UnitRemoved
}

public enum SquadMode {
    Forming,
    Enroute,
    InCombat,
    Idle,
    Regroup
}

public class SquadUpdate {
    public Squad squad { get; private set; }
    public SquadUpdateType type { get; private set; }
    public NPC unit { get; private set; }

    private SquadUpdate() { }

    public static SquadUpdate modeUpdate(Squad squad) {
        return new SquadUpdate() { squad = squad, type = SquadUpdateType.ModeUpdate };
    }
    public static SquadUpdate leaderUpdate(Squad squad) {
        return new SquadUpdate() { squad = squad, type = SquadUpdateType.LeaderUpdate };
    }
    public static SquadUpdate unitAdded(Squad squad, NPC unit) {
        return new SquadUpdate() { squad = squad, type = SquadUpdateType.UnitAdded, unit = unit };
    }
    public static SquadUpdate unitRemoved(Squad squad, NPC unit) {
        return new SquadUpdate() { squad = squad, type = SquadUpdateType.UnitRemoved, unit = unit };
    }

    override public string ToString() {
        string s = $"SquadUpdate {type} Type: {type}, mode: {squad.mode}";
        return s;
    }
}

public class Squad {
    public SquadMode mode { get; private set; }
    public MonoBehaviour leader { get; private set; } // should we have leader interface or sth?

    private List<NPC> units;

    private Squad() {
        units = new List<NPC>();
    }

    public static Squad create(MonoBehaviour leader) {
        return new Squad() {leader = leader};
    }

    /// Public -- 

    public void update() {

    }

    public void addUnit(NPC unit) {
        units.Add(unit);
        updateAllUnits(SquadUpdate.unitAdded(this, unit));
    }

    public void transferLeadership(MonoBehaviour newLeader) {
        this.leader = newLeader;
        updateAllUnits(SquadUpdate.leaderUpdate(this));
    }

    public void setMode(SquadMode mode) {
        if (this.mode != mode) {
            this.mode = mode;
            updateAllUnits(SquadUpdate.modeUpdate(this));
        }
    }

    /// Private -- 

    private void updateAllUnits(SquadUpdate update) {
        foreach (NPC unit in units) {
            PeasantAI ai = unit.ai as PeasantAI;
            ai.onSquadUpdate(update);
        }
    }
}