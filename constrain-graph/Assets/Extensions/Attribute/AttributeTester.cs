using UnityEngine;

namespace Shahant
{
    public class AttributeTester : MonoBehaviour
    {
        [SerializeField, TypeSelection(typeof(IDataProvider))] Object selectedObject;
    }
}

