using Shahant.MeshDraw;
using System.Collections.Generic;
using UnityEngine;
using Shahant.PathFinding;
using Shahant;

using SPGrid = Shahant.PathFinding.Grid;

namespace GameLineup.Theme
{
    public class Test_ThemeConfig : MonoBehaviour
    {
        [SerializeField] GameplayThemeConfig _themeConfig;
        [SerializeField] int width = 5, height = 5;

        private List<Drawer> Drawers { get; set; } = new();

        public void OnEnable()
        {
            SpawnTheme();
        }


        [ContextMenu("Spawn Theme")]
        public void SpawnTheme()
        {
            var grid = GenerateGrid();


            for(int i = 0; i < _themeConfig.Elements.Length; ++i)
            {
                var element = _themeConfig.Elements[i];
                var drawer =  Instantiate(element, transform);
                drawer.Setup(grid);
            }
        }

        private SPGrid GenerateGrid()
        {
            var grid = new SPGrid(width, height);

            return grid;
        }
        
    }
}
