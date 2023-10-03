using UnityEditor;

namespace Shahant.DataBinder
{
    [CustomEditor(typeof(BindingTarget))]
    public class BindingTarget_Editor : Editor 
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

        }
    }
}
