using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathfindController : MonoBehaviour {
    public Tilemap baseTilemap;
    public Tilemap wallsTilemap;
    
    public Vector3Int[, ] spots;

    Astar astar;
    List<Spot> roadPath = new List<Spot>();
    BoundsInt bounds;

    void Awake() {
        updateBounds();
        astar = new Astar(spots, bounds.size.x, bounds.size.y);
    }


    /// Public -- 

    public List<Vector2> findPathWorld(Vector2 from, Vector2 to) {
        updateBounds();
        updateGrid();

        Vector3Int gridFrom = baseTilemap.WorldToCell(from);
        Vector3Int gridTo = baseTilemap.WorldToCell(to);
        List<Spot> spotsPath = astar.createPath(spots, (Vector2Int)gridFrom, (Vector2Int)gridTo, 1000);
        if (spotsPath != null) {
            List<Vector2> path = new List<Vector2>();
            foreach(Spot spot in spotsPath) {
                path.Add(baseTilemap.GetCellCenterWorld(new Vector3Int(spot.x, spot.y, 0)));
            }
            path.Reverse();
            return path;
        } else {
            return null;
        }
    }

    /// Private --

    private void updateBounds() {
        baseTilemap.CompressBounds();
        wallsTilemap.CompressBounds();
        bounds = baseTilemap.cellBounds;
    }

    private void updateGrid() {
        spots = new Vector3Int[bounds.size.x, bounds.size.y];
        for (int x = bounds.xMin, i = 0; i < (bounds.size.x); x++, i++) {
            for (int y = bounds.yMin, j = 0; j < (bounds.size.y); y++, j++) {
                if (wallsTilemap.HasTile(new Vector3Int(x, y, 0))) {
                    spots[i, j] = new Vector3Int(x, y, 1);    
                } else if (baseTilemap.HasTile(new Vector3Int(x, y, 0))) {
                    spots[i, j] = new Vector3Int(x, y, 0);
                } else {
                    spots[i, j] = new Vector3Int(x, y, 1);
                }
            }
        }
    }
}