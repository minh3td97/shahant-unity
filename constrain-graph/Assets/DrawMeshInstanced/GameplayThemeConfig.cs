using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shahant.MeshDraw;
using SPG = Shahant.PathFinding.Grid;

namespace GameLineup.Theme
{
    public class GameplayThemeConfig : ScriptableObject
    {
        [SerializeField] Drawer<SPG>[] _elements;
        public Drawer<SPG>[] Elements => _elements;
    }
}
