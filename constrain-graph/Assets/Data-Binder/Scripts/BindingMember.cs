using System;
using UnityEngine;

namespace Shahant.DataBinder
{
    [System.Serializable]
    public partial class BindingMember
    {
        public UnityEngine.Object target;
        public string member;

        protected Wrapper wrapper;

        public virtual object Target => target;
        public virtual Type TargetType => Target?.GetType();
        public virtual bool IsValid => TargetType != null && wrapper.IsValid;

        public virtual void Setup(UnityEngine.Object context = null)
        {
            FindMemberInfor(context);
        }

        protected void FindMemberInfor(UnityEngine.Object context = null)
        {
            wrapper = new Wrapper();
            wrapper.Init(TargetType, member, context);
        }


        public object GetValue()
        {
            return wrapper.GetValue(Target);
        }

        public object GetValue(object target)
        {
            return wrapper.GetValue(target);
        }

        public object EditorGetValue()
        {
            Setup();
            return wrapper.GetValue(Target);
        }

        public void SetValue(object value)
        {
            wrapper.SetValue(Target, value);
        }
    }

}
