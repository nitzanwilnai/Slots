using System;
using UnityEngine;

namespace SlotsGame
{
    public enum SYMBOLS { NONE, BAR, BAR_2X, BAR_3X, CHERRY, SEVEN_RED, SEVEN_BLUE, COIN, WILD, BONUS, N100, N200, N500, TWO_X, THREE_X };

    [CreateAssetMenu(fileName = "Data", menuName = "Slots/SlotsBalance", order = 1)]
    public class SlotsBalanceSO : ScriptableObject
    {
        [HideInInspector] public int ID;

        [Header("Orders")]
        [Range(0.1f, 10.0f)] public float Reel1Speed;
        [Range(0.1f, 10.0f)] public float Reel2Speed;
        [Range(0.1f, 10.0f)] public float Reel3Speed;
        public AnimationCurve StartCurve1;
        public AnimationCurve StartCurve2;
        public AnimationCurve StartCurve3;
        public float StartCurve1Time;
        public float StartCurve2Time;
        public float StartCurve3Time;
        public SYMBOLS[] Reel1Symbols;
        public SYMBOLS[] Reel2Symbols;
        public SYMBOLS[] Reel3Symbols;

    }
}