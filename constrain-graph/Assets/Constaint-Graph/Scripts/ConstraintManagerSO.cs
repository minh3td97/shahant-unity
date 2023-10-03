using System.Collections.Generic;
using UnityEngine;

namespace Shahant.Constraint
{
    [CreateAssetMenu(menuName = "Shahant/Constraint/Manager")]
    public class ConstraintManagerSO : ScriptableObject
    {
        [SerializeField] List<NodeSO> _nodes;
        public IEnumerable<NodeSO> Nodes => _nodes;

        public void WakeUp()
        {
            for(int i = 0; i < _nodes.Count; ++i)
            {
                _nodes[i].WakeUp();
            }
            
        }

        public void SetConstraint()
        {
            for (int i = 0; i < _nodes.Count; ++i)
            {
                _nodes[i].SetConstraint();
            }
        }
    }

}
