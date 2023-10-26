using System.Collections.Generic;
using UnityEngine;
using SPG = Shahant.PathFinding.Grid;

namespace Shahant.MeshDraw
{
    public class GridMeshDrawer_RockGroup : Drawer<Shahant.PathFinding.Grid>
    {
        [SerializeField] GridMeshDrawer_Rock[] _drawers;
        [SerializeField] Vector2 _padding;

        SPG Grid => DrawerData;

        public override void OnSetup()
        {
            base.OnSetup();

            foreach (var drawer in _drawers) drawer.Setup(Grid);
            
            List<Vector3> pos = new List<Vector3>();
            List<Quaternion> rotation = new List<Quaternion>();
            List<Vector3> scale = new List<Vector3>();

            float xMin = 0;
            float xMax = Grid.Width;
            float yMin = 0; 
            float yMax = Grid.Height;

            float distance = 0;
            float[] lineLengths = new float[3] { yMax - yMin, xMax - xMin, yMax - yMin };


            foreach(var len in lineLengths)
            {
                distance = 0;
                while (distance < len)
                {
                    
                }
            }

            for(int i = 0; i < _drawers.Length; ++i)
            {
                var drawer = _drawers[i];
                drawer.Setup(pos, rotation, scale);
            }
            
        }

        public override void OnTeardown()
        {
            base.OnTeardown();
        }
    }
}

