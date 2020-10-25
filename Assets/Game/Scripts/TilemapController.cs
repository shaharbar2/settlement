using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TilemapAreaType {
    S_2x2,
    S_3x3
}

public class TilemapController : MonoBehaviour {

    internal class TilemapAreaData {
        public Vector2 worldPosition;
        public Vector3Int centerTile;
        public Tilemap tilemap;
        public TilemapAreaType type;

        public TilemapAreaData(Tilemap tilemap, Vector2 worldPosition, TilemapAreaType type) {
            this.centerTile = tilemap.WorldToCell(worldPosition);
            this.type = type;
            this.tilemap = tilemap;
            this.worldPosition = worldPosition;
        }

        public RectInt bounds {
            get {
                switch (type) {
                    case TilemapAreaType.S_2x2:
                        return new RectInt(centerTile.x - 1, centerTile.y - 1, 2, 2);
                    case TilemapAreaType.S_3x3:
                        return new RectInt(centerTile.x - 1, centerTile.y - 1, 3, 3);
                }
                throw (new System.Exception("Unknown area type"));
            }
        }
    }

    [SerializeField] private Tilemap baseTilemap;
    [SerializeField] private Tilemap wallsTilemap;
    [SerializeField] private Tilemap buildOverlayTilemap;

    [SerializeField] private TileBase buildHighlightTile;

    private TilemapAreaData highlightedArea;

    /// Public --

    public void highlightForBuild(Vector2 worldPosition, TilemapAreaType type) {
        if (highlightedArea != null) {
            removeHighlightForBuild();
        }
        highlightedArea = new TilemapAreaData(buildOverlayTilemap, worldPosition, type);
        fillAreaWithTile(highlightedArea, buildHighlightTile);
    }

    public void removeHighlightForBuild() {
        if (highlightedArea != null) {
            fillAreaWithTile(highlightedArea, null);
            highlightedArea = null;
        }
    }

    public bool isWalkable(Vector3 worldPosition) {
        return !wallsTilemap.HasTile(wallsTilemap.WorldToCell(worldPosition));
    }

    public void markUnwalkable(Vector3 worldPosition, TilemapAreaType type) {
        TilemapAreaData area = new TilemapAreaData(wallsTilemap, worldPosition, type);
        fillAreaWithTile(area, buildHighlightTile);
    }

    public Vector2Int tileDistance(Vector2 posA, Vector2 posB) {
        Vector3Int tileA = baseTilemap.WorldToCell(posA);
        Vector3Int tileB = baseTilemap.WorldToCell(posB);
        return new Vector2Int(tileA.x - tileB.x, tileA.y - tileB.y);
    }

    /// Private --

    private void fillAreaWithTile(TilemapAreaData areaData, TileBase tile) {
        for (int x = 0; x < areaData.bounds.size.x; x++) {
            for (int y = 0; y < areaData.bounds.size.y; y++) {
                Vector3Int pos = new Vector3Int(areaData.bounds.xMin + x, areaData.bounds.yMin + y, 0);
                areaData.tilemap.SetTile(pos, tile);
            }
        }
    }

}