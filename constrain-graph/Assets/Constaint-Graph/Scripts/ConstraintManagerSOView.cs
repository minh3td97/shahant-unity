using System.Collections.Generic;
using UnityEngine;

namespace Shahant.Constraint
{
    [RequireComponent(typeof(ConstraintGraphLayout))]
    [RequireComponent(typeof(LineRenderer))]
    public class ConstraintManagerSOView : MonoBehaviour
    {
        ConstraintGraphLayout _layout;
        LineRenderer _lineRender;

        [SerializeField] ConstraintManagerSO _data;
        [SerializeField] NodeSOView _nodePrefab;
        [SerializeField] Transform _container;

        private List<NodeSOView> _nodes = new();

        [ContextMenu("Generate Graph")]
        public void GenerateGraph()
        {
            if (_layout == null) _layout = GetComponent<ConstraintGraphLayout>();
            if (_lineRender == null) _lineRender = GetComponent<LineRenderer>();
            Clear();
            
            Vector2 nodeSize = (_nodePrefab.transform as RectTransform).sizeDelta;
            Vector2 nodeCenter = new Vector2(0.5f, -0.5f) * nodeSize;
            _layout.SetGraphCoordRect(GetGraphCoordRect());

            Vector2 startPos = _layout.GetStartPosition(_container as RectTransform, nodeSize);
            
            foreach(var node in _data.Nodes)
            {
                var view = Instantiate(_nodePrefab, _container, false);
                view.Setup(node);
                _nodes.Add(view);
                var rect = view.transform as RectTransform;
                rect.anchorMin = rect.anchorMax = Vector2.up;
                rect.anchoredPosition = _layout.CalculatePos(node.Coord, nodeSize, startPos);
                Vector2 start = rect.anchoredPosition;
                foreach(var child in node.Children)
                {
                    Vector2 end = _layout.CalculatePos(child.Coord, nodeSize, startPos);
                    _lineRender.GenerateLine(start, end);
                }
            }
        }

        private Rect GetGraphCoordRect() 
        {
            Rect graphCoordRect = new Rect();
            graphCoordRect.xMin = graphCoordRect.yMin = float.MaxValue;
            graphCoordRect.xMax = graphCoordRect.yMax = float.MinValue;
            foreach (var node in _data.Nodes)
            {
                if (graphCoordRect.xMin > node.Coord.x) graphCoordRect.xMin = node.Coord.x;
                if (graphCoordRect.xMax < node.Coord.x) graphCoordRect.xMax = node.Coord.x;
                if (graphCoordRect.yMin > node.Coord.y) graphCoordRect.yMin = node.Coord.y;
                if (graphCoordRect.yMax < node.Coord.y) graphCoordRect.yMax = node.Coord.y;
            }
            
            return graphCoordRect;
        }

        [ContextMenu("Clear")]
        private void Clear()
        {
            for(; _container.childCount > 0; )
            {
                DestroyImmediate(_container.GetChild(0).gameObject);
            }
            _nodes.Clear();

            if (_lineRender == null) _lineRender = GetComponent<LineRenderer>();
            _lineRender.Clear();
        }

    }
}