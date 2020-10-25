using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMenuSegmentData : MonoBehaviour {
    [SerializeField] public Texture2D iconTexture;
    [SerializeField] public RadialMenuSegmentData[] children;
    // todo: remove from here
    [SerializeField] public TilemapAreaType areaType;
}