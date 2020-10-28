using System.Collections.Generic;
using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathfindController : MonoBehaviour {
    public Tilemap baseTilemap;
    public Tilemap wallsTilemap;

    BoundsInt bounds;

    private Roy_T.AStar.Grids.Grid grid;
    private Roy_T.AStar.Paths.PathFinder pathFinder;
    void Awake() {
        updateBounds();
        updateGrid();
    }

    /// Public -- 

    public List<Vector2> findPathWorld(Vector2 from, Vector2 to) {
        updateBounds();
        updateGrid();

        Vector3Int tilemapFrom = baseTilemap.WorldToCell(from);
        Vector3Int tilemapTo = baseTilemap.WorldToCell(to);
        Path path = pathFinder.FindPath(tilemapToGrid(tilemapFrom), tilemapToGrid(tilemapTo), grid);

        List<Vector2> worldPath = new List<Vector2>();

        for (int i = 0; i < path.Edges.Count; i++) {
            var edge = path.Edges[i];
            worldPath.Add(baseTilemap.GetCellCenterWorld(gridToTilemap(edge.Start.Position)));
            if (i == path.Edges.Count - 1) {
                worldPath.Add(baseTilemap.GetCellCenterWorld(gridToTilemap(edge.End.Position)));
            }
        }
        return worldPath;
    }

    /// Private --

    private void updateBounds() {
        baseTilemap.CompressBounds();
        wallsTilemap.CompressBounds();
        bounds = baseTilemap.cellBounds;
    }

    private void updateGrid() {
        var gridSize = new GridSize(columns: bounds.size.x, rows: bounds.size.y);
        var cellSize = new Size(Distance.FromMeters(1), Distance.FromMeters(1));
        Velocity traversalVelocity = Velocity.FromKilometersPerHour(100);
        grid = Roy_T.AStar.Grids.Grid.CreateGridWithLateralAndDiagonalConnections(gridSize, cellSize, traversalVelocity);
        pathFinder = new PathFinder();

        for (int x = bounds.xMin, i = 0; i < (bounds.size.x); x++, i++) {
            for (int y = bounds.yMin, j = 0; j < (bounds.size.y); y++, j++) {
                var tilemapPos = new Vector3Int(x, y, 0);
                var gridPos = tilemapToGrid(tilemapPos);
                if (wallsTilemap.HasTile(tilemapPos)) {
                    grid.DisconnectNode(gridPos);
                    grid.RemoveDiagonalConnectionsIntersectingWithNode(gridPos);
                } else if (baseTilemap.HasTile(new Vector3Int(x, y, 0))) {
                } else {
                    grid.DisconnectNode(gridPos);
                    grid.RemoveDiagonalConnectionsIntersectingWithNode(gridPos);
                }
            }
        }
    }

    private GridPosition tilemapToGrid(Vector3Int pos) {
        int x = pos.x;
        int y = pos.y;
        if (baseTilemap.cellBounds.xMin < 0) {
            x += Mathf.Abs(baseTilemap.cellBounds.xMin);
        } else if (baseTilemap.cellBounds.xMin > 0) {
            x -= Mathf.Abs(baseTilemap.cellBounds.xMin);
        }
        if (baseTilemap.cellBounds.yMin < 0) {
            y += Mathf.Abs(baseTilemap.cellBounds.yMin);
        } else if (baseTilemap.cellBounds.yMin > 0) {
            y -= Mathf.Abs(baseTilemap.cellBounds.yMin);
        }
        return new GridPosition(x, y);
    }

    private Vector3Int gridToTilemap(Position pos) {
        int x = (int)pos.X;
        int y = (int)pos.Y;
        if (baseTilemap.cellBounds.xMin < 0) {
            x -= Mathf.Abs(baseTilemap.cellBounds.xMin);
        } else if (baseTilemap.cellBounds.xMin > 0) {
            x += Mathf.Abs(baseTilemap.cellBounds.xMin);
        }
        if (baseTilemap.cellBounds.yMin < 0) {
            y -= Mathf.Abs(baseTilemap.cellBounds.yMin);
        } else if (baseTilemap.cellBounds.yMin > 0) {
            y += Mathf.Abs(baseTilemap.cellBounds.yMin);
        }
        return new Vector3Int(x, y, 0);
    }
}