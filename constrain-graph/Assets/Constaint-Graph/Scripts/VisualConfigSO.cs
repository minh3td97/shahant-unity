using UnityEngine;

namespace Shahant.Constraint
{
    public class VisualConfigSO : ScriptableObject
    {
        [SerializeField] Sprite _background;
        [SerializeField] Sprite _icon;

        public Sprite Background => _background;
        public Sprite Icon => _icon;
    }
}
