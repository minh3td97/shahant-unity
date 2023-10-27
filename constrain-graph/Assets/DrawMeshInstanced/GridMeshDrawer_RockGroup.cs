using System.Collections.Generic;
using UnityEngine;
using SPG = Shahant.PathFinding.Grid;

namespace Shahant.MeshDraw
{
    public class GridMeshDrawer_RockGroup : CombineMeshDrawer<SPG>
    {
        
        [SerializeField] RockMesh _cornerMesh;
        [SerializeField] RockMesh[] _meshes;
        [SerializeField] Padding _padding;
        [SerializeField] Material _material;

        public override Material Material => _material;

        SPG Grid => Data;

        public override void OnSetup()
        {
            base.OnSetup();
            float widthWithPadding = Grid.Width + _padding.left + _padding.right;
            float heightWithPadding = Grid.Height + _padding.top + _padding.bottom;
            float fWidth = Mathf.CeilToInt(2 * widthWithPadding) * 0.5f;
            float fHeight = Mathf.CeilToInt(2* heightWithPadding) * 0.5f;

            float widthDiff = fWidth - widthWithPadding;
            float heightDiff = fHeight - heightWithPadding;

            float xMin = -_padding.left - widthDiff * 0.5f;
            float xMax = Grid.Width + _padding.right + widthDiff * 0.5f;
            float yMin = -_padding.bottom - heightDiff * 0.5f;
            float yMax = Grid.Height + _padding.top + heightDiff * 0.5f;

            var rockPath = new List<Vector3>();
            rockPath.Add(new Vector3(xMin, 0, yMin));
            rockPath.Add(new Vector3(xMin, 0, yMax));
            rockPath.Add(new Vector3(xMax, 0, yMax));
            rockPath.Add(new Vector3(xMax, 0, yMin));

            GenerateRocks(rockPath);
            Draw();
        }

        private void GenerateRocks(List<Vector3> rockPath)
        {
            bool isSpawnedAtStart = false;
            for(int i = 0; i < rockPath.Count - 1; ++i)
            {
                var start = rockPath[i];
                var end = rockPath[i + 1];
                GenerateRockLine(start, end, isSpawnedAtStart);
                isSpawnedAtStart = true;
            }
        }

        private void GenerateRockLine(Vector3 start, Vector3 end, bool isSpawnedStart)
        {
            var len = (end - start).magnitude;
            var directionVector = CalculateDirectionVector(start, end);
            Quaternion quanternion = CalculateRotation(start, end);
            Vector3 nextStart = start;

            if (!isSpawnedStart)
            {
                GenerateMeshFilter("rock_corner", _cornerMesh.mesh, start, quanternion, Vector3.one - 0.1f * Vector3.one);
                len -= _cornerMesh.size.z * 0.5f;
                nextStart = nextStart + directionVector * _cornerMesh.size.z * 0.5f;
            }
            else
            {
                len -= _cornerMesh.size.x * 0.5f;
                nextStart = nextStart + directionVector * _cornerMesh.size.x * 0.5f;
            }

            // corner at end 
            GenerateMeshFilter("rock_corner", _cornerMesh.mesh, end, quanternion, Vector3.one - 0.1f * Vector3.one);
            len -= _cornerMesh.size.y / 2;

            // Rock Between
            while(len > 0)
            {
                var selectedRock = SelectRockMesh(len);
                if (selectedRock == null) break;

                GenerateMeshFilter("rock", 
                    selectedRock.mesh, 
                    nextStart + directionVector * selectedRock.size.z * 0.5f,
                    quanternion,
                    selectedRock.size - 0.1f * Vector3.one);

                len -= selectedRock.size.z;
                nextStart = nextStart + directionVector * selectedRock.size.z;
            }

        }

        private Quaternion CalculateRotation(Vector3 start, Vector3 end)
        {
            var angle = Vector3.Angle(Vector3.forward, end - start);
            return Quaternion.Euler(0, angle, 0);
        }

        private Vector3 CalculateDirectionVector(Vector3 start, Vector3 end)
        {
            var directVector = end - start;
            directVector.x = directVector.x == 0 ? 0 : (directVector.x > 0 ? 1 : -1);
            directVector.y = directVector.y == 0 ? 0 : (directVector.y > 0 ? 1 : -1);
            directVector.z = directVector.z == 0 ? 0 : (directVector.z > 0 ? 1 : -1);
            return directVector;
        }

        private int selectedIndex = 0;
        private RockMesh SelectRockMesh(float len)
        {
            var nextSelectedIndex  = (selectedIndex + 1) % _meshes.Length;

            while(len < _meshes[nextSelectedIndex].size.z)
            {
                if (nextSelectedIndex == selectedIndex) break;
                nextSelectedIndex = (nextSelectedIndex + 1) % _meshes.Length;
            }

            if (nextSelectedIndex == selectedIndex) return null;
            selectedIndex = nextSelectedIndex;
            return _meshes[selectedIndex];
        }

        private void GenerateMeshFilter(string name, Mesh mesh, Vector3 pos, Quaternion rot, Vector3 scale)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform);
            go.transform.localPosition = pos;
            go.transform.localRotation = rot;
            go.transform.localScale = scale;
            
            var meshFilter = go.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = mesh;
        }

        public override void OnTeardown()
        {
            base.OnTeardown();
        }

        [System.Serializable]
        private class RockMesh
        {
            public Mesh mesh;
            public Vector3 size = Vector3.one;
        }

        [System.Serializable]
        private class Padding
        {
            public float left;
            public float right;
            public float top;
            public float bottom;
        }
    }
}

