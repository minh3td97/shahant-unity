using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shahant.PathFinding
{
    public class PathFinder 
    {
        public const int HORIZONTAL_COST = 1, VERTICAL_COST = 1;

        public Grid Grid { get; }


        public PathFinder(Grid grid)
        {
            Grid = grid;
        }

        public List<Node> Find(int startX, int startY, int endX, int endY)
            => Find(new Vector2Int(startX, startY), new Vector2Int(endX, endY));

        public List<Node> Find(Vector2Int start, Vector2Int end)
        {
            List<PathNode> openList = new List<PathNode>();
            List<PathNode> closedList = new List<PathNode>();
            Dictionary<Vector2Int, PathNode > createdPathNodeDict = new Dictionary<Vector2Int, PathNode>();
            HashSet<Vector2Int> closedCoords = new HashSet<Vector2Int>();

            var sNode = Grid.GetNode(start);
            var eNode = Grid.GetNode(end);

            var sPathNode = new PathNode(sNode);
            openList.Add(sPathNode);
            createdPathNodeDict.Add(sNode.Coord, sPathNode);

            sPathNode.gCost = 0;
            sPathNode.hCost = CaculateDistanceCost(start, end);

            while(openList.Count > 0)
            {
                var currentNode = GetLowestFCostPathNode(openList);
                if(currentNode.Node == eNode)
                {
                    return CalculatePath(currentNode);
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);
                closedCoords.Add(currentNode.Node.Coord);

                foreach(var neighbor in currentNode.Node.Neighbors)
                {
                    if (closedCoords.Contains(neighbor.Coord)) continue;
                    if(currentNode.Node.Approachable(neighbor) && neighbor.Approachable(currentNode.Node))
                    {
                        if (!createdPathNodeDict.ContainsKey(neighbor.Coord))
                        {
                            createdPathNodeDict.Add(neighbor.Coord, new PathNode(neighbor));
                        }
                        var neighborPathNode = createdPathNodeDict[neighbor.Coord];
                        float tentativeGCost = currentNode.gCost + CaculateDistanceCost(currentNode.Node.Coord, neighbor.Coord);
                        if(tentativeGCost < neighborPathNode.gCost)
                        {
                            neighborPathNode.PrevNode = currentNode;
                            neighborPathNode.gCost = tentativeGCost;
                            neighborPathNode.hCost = CaculateDistanceCost(neighbor.Coord, eNode.Coord);
                            neighborPathNode.CalculateFCost();
                        }
                        if (!openList.Contains(neighborPathNode))
                        {
                            openList.Add(neighborPathNode);
                        }
                    }
                }
            }

            return null;
        }

        private List<Node> CalculatePath(PathNode endPathNode)
        {
            List<Node> path = new List<Node>();
            path.Add(endPathNode.Node);
            PathNode currentNode = endPathNode;
            while(currentNode.PrevNode != null)
            {
                path.Add(currentNode.PrevNode.Node);
                currentNode = currentNode.PrevNode;
            }
            path.Reverse();
            return path;
        }

        private PathNode GetLowestFCostPathNode(List<PathNode> nodes)
        {
            var lowestFCostNode = nodes[0];
            for(int i = 0; i < nodes.Count; ++i)
            {
                if (nodes[i].fCost < lowestFCostNode.fCost)
                    lowestFCostNode = nodes[i];
            }
            return lowestFCostNode;
        }

        private float CaculateDistanceCost(Vector2Int start, Vector2Int end)
        {
            var v = end - start;
            return Mathf.Abs(v.x) * HORIZONTAL_COST + Mathf.Abs(v.y) * VERTICAL_COST;

        }
    }

    public class PathNode
    {
        public Node Node { get; }
        
        public float gCost;
        public float hCost;
        public float fCost;

        public PathNode PrevNode;

        public PathNode(Node node)
        {
            Node = node;
            gCost = float.MaxValue;
            CalculateFCost();
            PrevNode = null;
        }

        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }

        public override string ToString() => Node.Coord.ToString();
    }
}


