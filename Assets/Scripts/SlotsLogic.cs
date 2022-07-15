using System;

namespace SlotsGame
{
    public class SlotsLogic
    {
        public static int REEL_LINE_OFFSET = 5;
        public static int PRECISION = 1000;

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
                slotsData.ReelOffset[reelIdx] = 0;
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
            slotsData.ReelOffset[reelIdx] = GetReelOffset(slotsData, slotsBalance, reelIdx, currentTime - slotsData.ReelStartTime[reelIdx][slotsData.CurrentRun]);
        }

        public static void Tick(SlotsData slotsData, SlotsBalance slotsBalance)
        {
            long currentTime = GetCurrentTime();

            for (int reelIdx = 0; reelIdx < Constants.NUM_REELS; reelIdx++)
            {
                if (slotsData.ReelState[reelIdx] == REEL_STATE.RUNNING)
                {
                    long runningTime = currentTime - slotsData.ReelStartTime[reelIdx][slotsData.CurrentRun];
                    slotsData.ReelOffset[reelIdx] = GetReelOffset(slotsData, slotsBalance, reelIdx, runningTime);
                }
            }
        }

        public static long GetReelOffset(SlotsData slotsData, SlotsBalance slotsBalance, int reelIdx, long runningTime)
        {
            int reelSpeed = slotsBalance.ReelSpeed[reelIdx];

            // apply starting curve if we are below startCurveTime
            long index = (int)((runningTime * slotsBalance.StartCurve[reelIdx].Length) / slotsBalance.StartCurveTime[reelIdx]);
            if (runningTime < slotsBalance.StartCurveTime[reelIdx])
                reelSpeed = (reelSpeed * slotsBalance.StartCurve[reelIdx][index]) / PRECISION;

            // calculate reel offset based on running time
            long reelOffset = slotsData.ReelStartOffset[reelIdx][slotsData.CurrentRun] + ((reelSpeed * runningTime) / PRECISION);

            // wrap around
            if (reelOffset > slotsBalance.NumSymbols * PRECISION)
                reelOffset -= slotsBalance.NumSymbols * PRECISION;
            return reelOffset;
        }

        public static int GetSymbolIndexForReel(SlotsBalance slotsBalance, long offset)
        {
            // get the symbol index for this offset
            int symbolIdx = (int)((offset + REEL_LINE_OFFSET * PRECISION + PRECISION / 2) / PRECISION);
            symbolIdx %= slotsBalance.NumSymbols;
            return symbolIdx;
        }

        public static void AddScore(SlotsData slotsData, SlotsBalance slotsBalance)
        {
            for (int reelIdx = 0; reelIdx < Constants.NUM_REELS; reelIdx++)
            {
                int sIdx = GetSymbolIndexForReel(slotsBalance, slotsData.ReelOffset[reelIdx]);
                slotsData.Score += (int)slotsBalance.ReelSymbols[reelIdx][sIdx];   // I don't know how slot machines calculate scores  ¯\_(ツ)_/¯
            }
        }
    }
}