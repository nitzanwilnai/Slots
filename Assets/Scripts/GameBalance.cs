using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotsGame
{
    [Serializable]
    public class GameBalance
    {
        public int MinReelSpeed;
        public int MaxReelSpeed;
        public int[] StartCurve;
        public int[] EndCurve;
        public int MinTiles;
        public int MaxTiles;
    }
}