using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Astar {
    public Spot[, ] Spots;

    public Astar(Vector3Int[, ] grid, int columns, int rows) {
        Spots = new Spot[columns, rows];
    }

    private bool isValidPath(Vector3Int[, ] grid, Spot start, Spot end) {
        if (end == null)
            return false;
        if (start == null)
            return false;
        if (end.height >= 1)
            return false;
        return true;
    }

    public List<Spot> createPath(Vector3Int[, ] grid, Vector2Int start, Vector2Int end, int length) {
        //if (!IsValidPath(grid, start, end))
        //     return null;

        Spot End = null;
        Spot Start = null;
        var columns = Spots.GetUpperBound(0) + 1;
        var rows = Spots.GetUpperBound(1) + 1;
        Spots = new Spot[columns, rows];

        for (int i = 0; i < columns; i++) {
            for (int j = 0; j < rows; j++) {
                Spots[i, j] = new Spot(grid[i, j].x, grid[i, j].y, grid[i, j].z);
            }
        }

        for (int i = 0; i < columns; i++) {
            for (int j = 0; j < rows; j++) {
                Spots[i, j].addNeighboors(Spots, i, j);
                if (Spots[i, j].x == start.x && Spots[i, j].y == start.y)
                    Start = Spots[i, j];
                else if (Spots[i, j].x == end.x && Spots[i, j].y == end.y)
                    End = Spots[i, j];
            }
        }

        if (!isValidPath(grid, Start, End))
            return null;

        List<Spot> OpenSet = new List<Spot>();
        List<Spot> ClosedSet = new List<Spot>();

        OpenSet.Add(Start);

        while (OpenSet.Count > 0) {
            //Find shortest step distance in the direction of your goal within the open set
            int winner = 0;
            for (int i = 0; i < OpenSet.Count; i++)
                if (OpenSet[i].f < OpenSet[winner].f)
                    winner = i;
                else if (OpenSet[i].f == OpenSet[winner].f) //tie breaking for faster routing
                if (OpenSet[i].h < OpenSet[winner].h)
                    winner = i;

            var current = OpenSet[winner];

            //Found the path, creates and returns the path
            if (End != null && OpenSet[winner] == End) {
                List<Spot> Path = new List<Spot>();
                var temp = current;
                Path.Add(temp);
                while (temp.previous != null) {
                    Path.Add(temp.previous);
                    temp = temp.previous;
                }
                if (length - (Path.Count - 1) < 0) {
                    Path.RemoveRange(0, (Path.Count - 1) - length);
                }
                return Path;
            }

            OpenSet.Remove(current);
            ClosedSet.Add(current);

            //Finds the next closest step on the grid
            var neighboors = current.neighboors;
            for (int i = 0; i < neighboors.Count; i++) //look threw our current spots neighboors (current spot is the shortest F distance in openSet
            {
                var n = neighboors[i];
                if (!ClosedSet.Contains(n) && n.height < 1) //Checks to make sure the neighboor of our current tile is not within closed set, and has a height of less than 1
                {
                    var tempG = current.g + 1; //gets a temp comparison integer for seeing if a route is shorter than our current path

                    bool newPath = false;
                    if (OpenSet.Contains(n)) //Checks if the neighboor we are checking is within the openset
                    {
                        if (tempG < n.g) //The distance to the end goal from this neighboor is shorter so we need a new path
                        {
                            n.g = tempG;
                            newPath = true;
                        }
                    } else //if its not in openSet or closed set, then it IS a new path and we should add it too openset
                    {
                        n.g = tempG;
                        newPath = true;
                        OpenSet.Add(n);
                    }
                    if (newPath) //if it is a newPath caclulate the H and F and set current to the neighboors previous
                    {
                        n.h = Heuristic(n, End);
                        n.f = n.g + n.h;
                        n.previous = current;
                    }
                }
            }
        }
        return null;
    }

    private int Heuristic(Spot a, Spot b) {
        //manhattan
        // var dx = Math.Abs(a.x - b.x);
        // var dy = Math.Abs(a.y - b.y);
        // return 1 * (dx + dy);

        #region diagonal
        //diagonal
        // Chebyshev distance
        var D = 1;
        var D2 = 1;
        //octile distance
        // var D = 1;
        // var D2 = 1;
        var dx = Math.Abs(a.x - b.x);
        var dy = Math.Abs(a.y - b.y);
        var result = (int)(1 * (dx + dy) + (D2 - 2 * D));
        // return result;// *= (1 + (1 / 1000));
        return (int)Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
        #endregion
    }
}
public class Spot {
    public int x;
    public int y;
    public int f;
    public int g;
    public int h;
    public int height = 0;
    public List<Spot> neighboors;
    public Spot previous = null;

    public Spot(int x, int y, int height) {
        this.x = x;
        this.y = y;
        f = 0;
        g = 0;
        h = 0;
        neighboors = new List<Spot>();
        this.height = height;
    }

    public void addNeighboors(Spot[, ] grid, int x, int y) {
        if (x < grid.GetUpperBound(0))
            neighboors.Add(grid[x + 1, y]);
        if (x > 0)
            neighboors.Add(grid[x - 1, y]);
        if (y < grid.GetUpperBound(1))
            neighboors.Add(grid[x, y + 1]);
        if (y > 0)
            neighboors.Add(grid[x, y - 1]);
        #region diagonal
        // if (x > 0 && y > 0)
        //    neighboors.Add(grid[x - 1,y - 1]);
        // if (x < grid.GetUpperBound(0) && y > 0)
        //    neighboors.Add(grid[x + 1, y - 1]);
        // if (x > 0 && y < grid.GetUpperBound(1))
        //    neighboors.Add(grid[x - 1, y + 1]);
        // if (x < grid.GetUpperBound(0) && y < grid.GetUpperBound(1))
        //    neighboors.Add(grid[x + 1, y + 1]);
        #endregion
    }
}