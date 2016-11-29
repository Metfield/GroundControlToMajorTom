using UnityEngine;
using System;
using Shared;

namespace GroundControl
{
    [Serializable]
    public class CargoItemProperties
    {
        public ECargoItem item;
        public int cost;
        public Sprite sprite;
        public Color color;
    }
}