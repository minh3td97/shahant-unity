using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shahant
{
    public class View : MonoBehaviour, IDataProvider
    {
        private static readonly string[] bindingFilters = { "static", "dynamic"};

        public event Action<IDataProvider, int, object> DataChanged;
        public Type GetDataType() => GetType();
        public object GetData() => this;
        public virtual string[] BindingFilters => bindingFilters;
        public void TriggerChanged(int mask) => DataChanged?.Invoke(this, mask, GetData());
        public void TriggerChanged(string filters) => TriggerChanged(this.GetFilters(filters));
        [ContextMenu("Trigger change all")]
        public void TriggerChangedAll() => TriggerChanged(~0);
    }

    public class View<T> : View
    {
        public T Data { get; private set; }

        public void Setup(T data)
        {
            Data = data;
        }

    }
}

