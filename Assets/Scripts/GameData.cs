using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotsGame
{
    public enum REEL_STATE { STOPPED, RUNNING };

    [Serializable]
    public class GameData
    {
        public long[] ReelRunningTime;

        public long[][] ReelStartTime;
        public long[][] ReelStartOffset;
        public long[][] ReelStopTime;

        public int CurrentRun;

        public int PrevFrameTime;

        public long[] ReelOffset;
        public REEL_STATE[] ReelState;
    }
}