using System;

namespace SlotsGame
{
    public class SlotsLogic
    {
        public static int REEL_START_OFFSET = 5;
        public static int PRECISION = 1000;

        public static int IncrementSeed(int seed)
        {
            //AI.Log();
            return (214013 * seed + 2531011);
        }

        public static int RandInt(ref int seed)
        {
            int value = (seed >> 16) & 0x7FFF;
            seed = IncrementSeed(seed);
            return value;
        }

        public static int RandRange(int min, int max, ref int seed)
        {
            int precision = 0x7FFF;
            int randomValue = RandInt(ref seed);
            return ((max - min) * (randomValue % precision) / precision) + min;
        }

        public static long GetCurrentTime()
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        public static void AllocateGame(SlotsData slotsData)
        {
            slotsData.ReelOffset = new long[Constants.NUM_REELS];
            slotsData.ReelRunningTime = new long[Constants.NUM_REELS];

            slotsData.ReelStartTime = new long[Constants.NUM_REELS][];
            slotsData.ReelStopTime = new long[Constants.NUM_REELS][];
            slotsData.ReelStartOffset = new long[Constants.NUM_REELS][];
            for (int i = 0; i < Constants.NUM_REELS; i++)
            {
                slotsData.ReelStartTime[i] = new long[Constants.MAX_RUNS];
                slotsData.ReelStopTime[i] = new long[Constants.MAX_RUNS];
                slotsData.ReelStartOffset[i] = new long[Constants.MAX_RUNS];
            }

            slotsData.ReelState = new REEL_STATE[Constants.NUM_REELS];
        }

        public static void StartGame(SlotsData slotsData)
        {
            slotsData.CurrentRun = 0;
            slotsData.Score = 0;
            for (int reelIdx = 0; reelIdx < Constants.NUM_REELS; reelIdx++)
            {
                slotsData.ReelOffset[reelIdx] = REEL_START_OFFSET * PRECISION;
                slotsData.ReelState[reelIdx] = REEL_STATE.STOPPED;
            }
        }

        public static void StartReels(SlotsData slotsData)
        {
            slotsData.CurrentRun++;

            long currentTime = GetCurrentTime();
            for (int reelIdx = 0; reelIdx < Constants.NUM_REELS; reelIdx++)
            {
                slotsData.ReelState[reelIdx] = REEL_STATE.RUNNING;
                slotsData.ReelStartOffset[reelIdx][slotsData.CurrentRun] = slotsData.ReelOffset[reelIdx];
                slotsData.ReelStartTime[reelIdx][slotsData.CurrentRun] = currentTime;
            }
        }

        public static void StopAllReels(SlotsData slotsData, SlotsBalance slotsBalance, long currentTime)
        {
            for (int reelIdx = 0; reelIdx < Constants.NUM_REELS; reelIdx++)
                StopReel(slotsData, slotsBalance, reelIdx, currentTime);
        }

        public static void StopReel(SlotsData slotsData, SlotsBalance slotsBalance, int reelIdx, long currentTime)
        {
            slotsData.ReelState[reelIdx] = REEL_STATE.STOPPED;
            slotsData.ReelStopTime[reelIdx][slotsData.CurrentRun] = currentTime;
            setReelOffset(slotsData, slotsBalance, reelIdx, currentTime - slotsData.ReelStartTime[reelIdx][slotsData.CurrentRun]);
        }

        public static void Tick(SlotsData slotsData, SlotsBalance slotsBalance)
        {
            long currentTime = GetCurrentTime();

            for (int reelIdx = 0; reelIdx < Constants.NUM_REELS; reelIdx++)
            {
                if (slotsData.ReelState[reelIdx] == REEL_STATE.RUNNING)
                {
                    long runningTime = currentTime - slotsData.ReelStartTime[reelIdx][slotsData.CurrentRun];
                    setReelOffset(slotsData, slotsBalance, reelIdx, runningTime);
                }
            }
        }

        private static void setReelOffset(SlotsData slotsData, SlotsBalance slotsBalance, int reelIdx, long runningTime)
        {
            int reelSpeed = slotsBalance.ReelSpeed[reelIdx];

            // apply starting curve if we are below startCurveTime
            long index = (int)((runningTime * slotsBalance.StartCurve[reelIdx].Length) / slotsBalance.StartCurveTime[reelIdx]);
            if (runningTime < slotsBalance.StartCurveTime[reelIdx])
                reelSpeed = (reelSpeed * slotsBalance.StartCurve[reelIdx][index]) / PRECISION;

            // calculate reel offset based on running time
            slotsData.ReelOffset[reelIdx] = slotsData.ReelStartOffset[reelIdx][slotsData.CurrentRun] + ((reelSpeed * runningTime) / PRECISION);

            // wrap around
            if (slotsData.ReelOffset[reelIdx] > (slotsBalance.NumSymbols + REEL_START_OFFSET) * PRECISION)
                slotsData.ReelOffset[reelIdx] -= slotsBalance.NumSymbols * PRECISION;
        }


        public static void AddScore(SlotsData slotsData, SlotsBalance slotsBalance)
        {
            for (int reelIdx = 0; reelIdx < Constants.NUM_REELS; reelIdx++)
            {
                int sIdx = (int)((slotsData.ReelOffset[reelIdx] + PRECISION / 2) / PRECISION);
                sIdx %= slotsBalance.NumSymbols;
                slotsData.Score += (int)slotsBalance.ReelSymbols[reelIdx][sIdx];
            }
        }
    }
}