using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace Shahant.PathFinding
{
    public enum Mode
    {
        Start,
        End,
        Blocked,
        Wall
    }

    public class Test : MonoBehaviour
    {
        [SerializeField] NodeView _nodePrefab;
        [SerializeField] Transform _container;
        [SerializeField] int _width = 10, _height = 10;
        [SerializeField] float size = 60, spacing = 5;
        [SerializeField] Color _defaultColor = Color.white, _startColor = Color.white, _endColor = Color.white, _blockColor = Color.black, _pathColor = Color.white;


        [Space(10)]
        [SerializeField] NodeView _selectedNodeView;
        [SerializeField] NodeView _startView;
        [SerializeField] NodeView _endView;
        [SerializeField] Mode _mode;

        Dictionary<Vector2Int, NodeView> _nodes = new Dictionary<Vector2Int, NodeView>();
        List<NodeView> _pathNodeView = new List<NodeView>();
        private PathFinder _pathFinder;

        public void OnEnable()
        {
            GenerateGrid();
        }

        [ContextMenu("Generate Grid")]
        public void GenerateGrid()
        {
            Clear();
            
            Grid grid = new Grid(_width, _height);
            _pathFinder = new PathFinder(grid);

            for(int i = 0; i < grid.Nodes.Count; ++i)
            {
                var node = grid.Nodes[i];
               
                NodeView nodeView = Instantiate(_nodePrefab, _container);
                nodeView.gameObject.SetActive(true);
                (nodeView.transform as RectTransform).anchoredPosition = new Vector3(node.Coord.x, node.Coord.y) * (size + spacing);
                nodeView.SetColor(_defaultColor);
                nodeView.Setup(node);

                nodeView.OnButtonClicked -= Node_OnSelected;
                nodeView.OnButtonClicked += Node_OnSelected;

                _nodes.Add(node.Coord, nodeView);
            }
        }


        public void Node_OnSelected(NodeView nodeView)
        {
            _selectedNodeView = nodeView;
            if(_mode == Mode.Start)
            {
                if (nodeView.Data.Blocked) return;
                _startView?.SetColor(_defaultColor);
                _startView = nodeView;
                _startView.SetColor( _startColor);
                if (_endView == nodeView) _endView = null;
            }
            else if(_mode == Mode.End)
            {
                if (nodeView.Data.Blocked) return;
                _endView?.SetColor(_defaultColor);
                _endView = nodeView;
                _endView.SetColor(_endColor);
                if (_startView == nodeView) _startView = null;
                GeneratePath();
            }
            else if(_mode == Mode.Blocked)
            {
                nodeView.Data.Blocked = !nodeView.Data.Blocked;
                nodeView.SetColor(nodeView.Data.Blocked ? _blockColor : _defaultColor);
            }
        }

        public void GeneratePath()
        {
            foreach (var node in _pathNodeView) 
            {
                if (node == _startView || node == _endView || node.Data.Blocked) continue;
                node.SetColor(_defaultColor); 
            }
            _pathNodeView.Clear();
            if(_startView && _endView)
            {

                List<Node> path = _pathFinder.Find(_startView.Data.Coord, _endView.Data.Coord);
                if (path == null) return;
                foreach(var node in path)
                {
                    if(node.Coord != _startView.Data.Coord && node.Coord != _endView.Data.Coord)
                    {
                        _nodes[node.Coord].SetColor( _pathColor);
                        _pathNodeView.Add(_nodes[node.Coord]);
                    }
                }
            }
        }

        private void Clear()
        {
            while(_container.childCount > 0)
            {
                DestroyImmediate(_container.GetChild(0).gameObject);
            }
            _nodes.Clear();
        }

        
        [CustomEditor(typeof(Test))]
        public class _Editor : Editor
        {
            Test tool;
            private string[] _modes = new string[4] { "Start", "End", "Blocked", "Wall" };
            int mode = 0;

            public void OnEnable()
            {
                tool = (Test)target;
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                GUILayout.Space(10);
                mode = GUILayout.Toolbar(mode, _modes);
                tool._mode = (Mode)mode;
                EditorUtility.SetDirty(tool);
                if(tool._mode == Mode.Wall)
                {
                    GUILayout.Space(10);
                    GUILayout.BeginHorizontal();
                    if(GUILayout.Button("Up"))
                    {
                        AddWall(Direction.Up);
                    }
                    if (GUILayout.Button("Right"))
                    {
                        AddWall(Direction.Right);
                    }
                    if (GUILayout.Button("Down"))
                    {
                        AddWall(Direction.Down);
                    }
                    if (GUILayout.Button("Left"))
                    {
                        AddWall(Direction.Left);
                    }
                    GUILayout.EndHorizontal();
                }
            }

            private void AddWall(Direction direction)
            {
                if (!tool._selectedNodeView.Data.Walls.Contains(direction))
                {
                    tool._selectedNodeView.Data.AddWall(direction);
                }
                else
                {
                    tool._selectedNodeView.Data.RemoveWall(direction);
                }
                tool._selectedNodeView.UpdateWalls();
            }
        }
    }
}
