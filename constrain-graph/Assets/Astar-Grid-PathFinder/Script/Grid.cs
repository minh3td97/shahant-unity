using System.Collections.Generic;
using UnityEngine;

namespace Shahant.PathFinding
{

    public class Grid
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public List<Node> Nodes { get; } = new();

        public Grid(int width, int height)
        {
            Width = width;
            Height = height;

            Dictionary<Vector2Int, Node> nodeDict = new Dictionary<Vector2Int, Node>();

            var directions = System.Enum.GetValues(typeof(Direction));

            for(int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    Node node = new Node(x, y);
                    Nodes.Add(node);
                    nodeDict.Add(node.Coord, node);
                }
            }

            for(int i = 0; i < Nodes.Count; ++i)
            {
                Nodes[i].AddNeighbor(GetNode(Nodes[i].Coord + Direction.Up.ToVector()));
                Nodes[i].AddNeighbor(GetNode(Nodes[i].Coord + Direction.Down.ToVector()));
                Nodes[i].AddNeighbor(GetNode(Nodes[i].Coord + Direction.Right.ToVector()));
                Nodes[i].AddNeighbor(GetNode(Nodes[i].Coord + Direction.Left.ToVector()));
            }

        }
        
        public Node GetNode(Vector2Int coord)
        {
            if(coord.x >= 0 && coord.y >= 0 && coord.x < Width && coord.y < Height)
                return Nodes[coord.y * Width + coord.x];
            return null;
        }
    }

    
}


