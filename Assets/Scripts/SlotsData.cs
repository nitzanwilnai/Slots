using System;
using System.Collections;
using System.Collections.Generic;

namespace SlotsGame
{
    public enum REEL_STATE { STOPPED, RUNNING };

    // all the data we need to know about player actions in the slot game
    [Serializable]
    public class SlotsData
    {
        public long[] ReelRunningTime;

        public long[][] ReelStartTime;
        public long[][] ReelStartOffset;
        public long[][] ReelStopTime;

        public int CurrentRun;

        public long[] ReelOffset;
        public REEL_STATE[] ReelState;

        public int Score;
    }
}