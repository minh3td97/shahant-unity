#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace Shahant.DataBinder
{
    public class DataBinder_MBProvider : DataBinder
    {
        [SerializeField] View _provider;
        [SerializeField] BindingTarget[] _targets;
        
        public override IDataProvider DataProvider => _provider;
        public override IEnumerable<BindingTarget> Targets => _targets;

#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(DataBinder_MBProvider))]
        public class _Editor : DataBinder_Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
            }
        }
#endif
    }

    
}
#endif