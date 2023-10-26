using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shahant.MeshDraw
{
    public class MeshDrawer<T> : MeshDrawer
    {
        public T DrawerData { get; private set; }
        public virtual void Setup(T data)
        {
            base.Setup(data);
            this.DrawerData = data;
        }
    }

    public class Drawer : MonoBehaviour, IDataView<object>
    {
        public object Data { get; private set; }

        public virtual void Setup(object data)
        {
            Data = data;

            if(Data != null)
                OnSetup();
        }

        public virtual void OnSetup() { }

        public virtual void OnTeardown() { }

        public virtual void Teardown()
        {
            if(Data != null)
                OnTeardown();
        }

        protected virtual void Draw()  {  }
    }

    public class Drawer<T> : Drawer
    {
        public T DrawerData { get; private set; }
        public virtual void Setup(T data)
        {
            base.Setup(data);
            this.DrawerData = data;
        }
    }

    public class MeshDrawer : Drawer
    {
        [SerializeField] Mesh _mesh;
        [SerializeField] Material _material;

        public virtual bool IsInitialized { get; private set; }
        public Mesh Mesh => _mesh;
        public Material Material => _material;

        private Matrix4x4[] _matrices;

        public virtual void Setup(List<Vector3> positions, List<Quaternion> rotations, List<Vector3> scales)
        {
            IsInitialized = false;
            if (positions.Count != rotations.Count || positions.Count != scales.Count) return;
            _matrices = new Matrix4x4[positions.Count];
            for(int i = 0; i < positions.Count; ++i)
            {
                _matrices[i] = Matrix4x4.TRS(positions[i], rotations[i], scales[i]);
            }

            IsInitialized = true;
        }

        protected virtual void Update()
        {
            Draw();
        }

        protected override void Draw()
        {
            if (!IsInitialized)
            {
                return;
            }

            for (int i = 0; i < _matrices.Length; ++i)
            {
                Graphics.DrawMeshInstanced(Mesh, 0, Material, _matrices);
            }
        }
    }
}
