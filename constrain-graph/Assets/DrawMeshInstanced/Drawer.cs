using UnityEngine;

namespace Shahant.MeshDraw
{
    public class Drawer : MonoBehaviour
    {
        protected virtual void Draw()  {  }
    }

    public class Drawer<T> : Drawer, IDataView<T>
    {
        public T Data { get; private set; }
        public void Setup(T data)
        {
            Data = data;
            OnSetup();
        }

        public virtual void OnSetup() { }

        public virtual void OnTeardown() { }

        public void Teardown()
        {
            OnTeardown();
        }
    }
}

