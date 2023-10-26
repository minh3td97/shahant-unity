using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shahant.MeshDraw;

namespace GameLineup.Theme
{
    public class GameplayThemeConfig : ScriptableObject
    {
        [SerializeField] Drawer[] _elements;
        public Drawer[] Elements => _elements;
    }
}
