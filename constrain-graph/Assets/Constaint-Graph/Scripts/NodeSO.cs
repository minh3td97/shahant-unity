using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shahant.Constraint
{
    [CreateAssetMenu(menuName = "Shahant/Constraint/Node")]
    public class NodeSO : ScriptableObject
    {
        [SerializeField] List<NodeSO> _children;
        [SerializeField] Vector2 _coord;
        [SerializeField] Sprite _background;
        [SerializeField] Sprite _icon;

        List<NodeSO> _parents = new();

        public Vector2 Coord => _coord;
        public Sprite Background => _background;
        public Sprite Icon => _icon;
        public IEnumerable<NodeSO> Children => _children;
        public IEnumerable<NodeSO> Parent => _parents;

        public void WakeUp()
        {
            _parents.Clear();
        }

        public void SetConstraint()
        {
            foreach (var node in Children)
                node.AddParent(this);
        }

        public void AddParent(NodeSO parent)
        {
            if(!_parents.Contains(parent))
                _parents.Add(parent);
        }

        public void ShutDown()
        {
            
        }

    } 
}
