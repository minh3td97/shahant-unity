using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

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
        

        [Space(10)]
        [SerializeField] NodeView _selectedNodeView;
        [SerializeField] Mode _mode;

        [ContextMenu("Generate Grid")]
        public void GenerateGrid()
        {
            Grid grid = new Grid(_width, _height);
            for(int i = 0; i < grid.Nodes.Count; ++i)
            {
                
            }
        }


        
        [CustomEditor(typeof(Test))]
        public class _Editor : Editor
        {
            private string[] _modes = new string[4] { "Start", "End", "Blocked", "Wall" };
            int mode = 0;
            
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                GUILayout.Space(10);
                mode = GUILayout.Toolbar(mode, _modes);
            }
        }
    }
}
