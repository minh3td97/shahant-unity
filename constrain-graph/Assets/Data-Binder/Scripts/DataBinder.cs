using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Shahant.DataBinder
{
    public class DataBinder : MonoBehaviour
    {
        [SerializeField] private int flag = 0x01;

        public virtual IDataProvider DataProvider { get; }
        public virtual IEnumerable<BindingTarget> Targets { get; }
        public Type ProviderDataType => (DataProvider as IDataProvider)?.GetDataType();

        public int Flag
        {
            get => flag;
            set => flag = value;
        }

        private bool _initialized = false;

        public void OnEnable()
        {
            Init();
            Bind();
        }

        public void OnDisable()
        {
            if (DataProvider != null) DataProvider.DataChanged -= DataChanged;
        }

        private void Init()
        {
            if (!_initialized)
            {
                foreach(var _ in Targets)
                {
                    _.Init(ProviderDataType, this);
                }
                _initialized = true;
            }

            if (DataProvider == null) return;

            DataProvider.DataChanged -= DataChanged;
            DataProvider.DataChanged += DataChanged;
        }

        private void DataChanged(IDataProvider dataProvider, int flags, object data)
        {
            if (dataProvider != null)
            {
                Bind(flags, dataProvider.GetData());
            }
        }

        public void Bind()
        {
            if (DataProvider != null)
            {
                Bind(flag, DataProvider.GetData());
            }
        }

        private void Bind(object data)
        {
            Bind(flag, data);
        }

        private void Bind(int flags, object data)
        {
            if ((flag & flags) == 0) return;

            foreach(var _ in Targets)
            {
                _.Bind(data);
            }
        }
    }

    public class Wrapper
    {
        private PropertyInfo propertyInfo;
        private FieldInfo fieldInfo;
        private MethodInfo methodInfo;
        private string member;

        public bool IsStatic { get; private set; }
        public bool IsValid { get; private set; }
        public Type ReturnType { get; private set; }


        public void Init(Type t, string member, UnityEngine.Object context)
        {
            this.member = member;
            fieldInfo = null;
            propertyInfo = null;
            methodInfo = null;
            IsValid = false;
            ReturnType = null;

            if (t == null || string.IsNullOrEmpty(member)) return;

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            fieldInfo = t.GetField(member, flags);
            if (fieldInfo == null)
            {
                propertyInfo = t.GetProperty(member, flags);
            }
            if (fieldInfo == null && propertyInfo == null)
            {
                try
                {
                    methodInfo = t.GetMethod(member, flags);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"{context.ToString()}: {t} / {member} / {ex}");
                }

            }

            IsStatic = fieldInfo?.IsStatic ?? propertyInfo?.GetMethod?.IsStatic ?? methodInfo?.IsStatic ?? false;
            IsValid = fieldInfo != null || propertyInfo != null || methodInfo != null;
            ReturnType = fieldInfo?.FieldType ?? propertyInfo?.PropertyType ?? methodInfo?.ReturnType;
        }

        static readonly object[] emptyArray = new object[0];
        static readonly object[] oneArray = new object[1];

        public object GetValue(object @object)
        {
            object obj = (@object == null && !IsStatic) ? null : (fieldInfo?.GetValue(@object) ?? propertyInfo?.GetValue(@object) ?? methodInfo?.Invoke(@object, null));
            return obj;
        }

        public void SetValue(object @object, object value)
        {
            fieldInfo?.SetValue(@object, value);
            propertyInfo?.SetValue(@object, value);
            if (methodInfo != null)
            {
                oneArray[0] = value;
                methodInfo?.Invoke(@object, oneArray);
            }
        }

        public void Invoke(object @object)
        {
            methodInfo?.Invoke(@object, null);
        }

        public override string ToString()
        {
            return string.Format("Member Name: {0}\n Member: {1} ({2}) ({3})",
                member,
                fieldInfo?.Name ?? propertyInfo?.Name ?? methodInfo?.Name,
                fieldInfo?.FieldType ?? propertyInfo?.PropertyType ?? methodInfo?.ReturnType,
                fieldInfo?.GetType() ?? propertyInfo?.GetType() ?? methodInfo?.GetType());
        }
    }

}
