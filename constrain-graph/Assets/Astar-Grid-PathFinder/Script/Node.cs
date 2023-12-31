﻿using System.Collections.Generic;
using UnityEngine;

namespace Shahant.PathFinding
{
    [System.Serializable]
    public class Node
    {
        public Vector2Int Coord;
        public List<Node> Neighbors { get; } = new();
        public List<Direction> Walls { get; } = new();
        public bool Blocked { get; set; }

        public event System.Action<Node> OnDataChanged;
        
        public Node(int x, int y) 
        {
            Coord.x = x;
            Coord.y = y;
        }

        public Node(Vector2Int Coord)
        {
            this.Coord = Coord;
        }

        public void AddNeighbor(Node node)
        {
            if (node == null) return;
            Neighbors.Add(node);
        }

        public void AddWall(Direction wall) => Walls.Add(wall);
        public void RemoveWall(Direction wall) 
        { 
            if (Walls.Contains(wall)) 
                Walls.Remove(wall); 
        }
        
        public bool Approachable(Direction dir) => !Blocked && !Walls.Contains(dir);
        public bool Approachable(Node neighbor) => Approachable((neighbor.Coord - Coord).ToDirection());
    }
}


