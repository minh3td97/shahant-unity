using UnityEngine;

namespace Shahant.MeshDraw
{
    public abstract class CombineMeshDrawer<T> : Drawer<T>
    { 

        public abstract Material Material { get; }

        protected override void Draw()
        {
            var combineDrawer = new GameObject("Combined");
            combineDrawer.transform.SetParent(transform);

            Mesh mesh = Combine();

            var meshFilter = combineDrawer.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = mesh;

            var meshRender = combineDrawer.AddComponent<MeshRenderer>();
            meshRender.sharedMaterial = Material;
        }

        protected virtual Mesh Combine(MeshFilter[] meshFilters)
        {
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];

            int i = 0;
            while (i < meshFilters.Length)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                meshFilters[i].gameObject.SetActive(false);

                i++;
            }

            Mesh mesh = new Mesh();
            mesh.CombineMeshes(combine);
            return mesh;
        }

        protected Mesh Combine()
        {
            MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
            return Combine(meshFilters);
        }

        protected void Clear()
        {
            while(transform.childCount > 0)
            {
                Destroy(transform.GetChild(0).gameObject);
            }
        }
    }
    
}


