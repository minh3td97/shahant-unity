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

            for (int x = 0; x < width; ++x)
            {
                for(int y = 0; y < height; ++y)
                {
                    Node node = new Node(x, y);
                    Nodes.Add(node);
                    nodeDict.Add(node.Coord, node);

                    // Caculate Neigbors
                    foreach(Direction dir in directions)
                    {
                        if (dir == Direction.None) continue;
                        var coord = node.Coord + dir.ToVector();
                        
                        if(nodeDict.TryGetValue(coord, out var nNode))
                        {
                            nNode.AddNeighbor(node);
                            node.AddNeighbor(nNode);
                        }
                    }
                    
                }
            }
        }
    }

    
}


