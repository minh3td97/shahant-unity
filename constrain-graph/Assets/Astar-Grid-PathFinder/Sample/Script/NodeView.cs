using UnityEngine;

namespace Shahant.PathFinding
{
    public class NodeView : MonoBehaviour
    {

        public event System.Action<NodeView> OnButtonClicked;

        public void OnClicked()
        {
            OnButtonClicked?.Invoke(this);
        }
    }
}
