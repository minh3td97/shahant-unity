using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shahant.DataBinder
{
    public class DataBinder : MonoBehaviour
    {
        public virtual IDataProvider DataProvider { get; }
        public virtual IEnumerable<BindingTarget> Targets { get; }
        
    }
}
