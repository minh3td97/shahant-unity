using UnityEngine;
using UnityEngine.UI;

namespace Shahant.Constraint
{
    public class NodeSOView : MonoBehaviour
    {
        [SerializeField] Image _background;
        [SerializeField] Image _icon;

        public NodeSO Data { get; private set; }

        public void Setup(NodeSO data)
        {
            Data = data;
            OnSetup();
        }

        private void OnSetup()
        {
            _background.sprite = Data.Background;
            _icon.sprite = Data.Icon;
        } 
    }
}
