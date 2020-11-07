using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Settlement.UI.RadialMenu {
    public class BuildMenuSegmentData : RadialMenuSegmentData {
        [SerializeField] public Texture2D iconTexture;
        [SerializeField] public BuildingType buildingType;
    }
}