using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        public static void AllocateGame(GameData gameData)
        {
            gameData.ReelOffset = new long[Constants.NUM_REELS];
            gameData.ReelRunningTime = new long[Constants.NUM_REELS];

            gameData.ReelStartTime = new long[Constants.NUM_REELS][];
            gameData.ReelStopTime = new long[Constants.NUM_REELS][];
            gameData.ReelStartOffset = new long[Constants.NUM_REELS][];
            for (int i = 0; i < Constants.NUM_REELS; i++)
            {
                gameData.ReelStartTime[i] = new long[Constants.MAX_RUNS];
                gameData.ReelStopTime[i] = new long[Constants.MAX_RUNS];
                gameData.ReelStartOffset[i] = new long[Constants.MAX_RUNS];
            }

            gameData.ReelState = new REEL_STATE[Constants.NUM_REELS];
        }

        public static void StartGame(GameData gameData, SlotsBalance slotsBalance, ref int seed)
        {
            gameData.CurrentRun = 0;
            for (int reelIdx = 0; reelIdx < Constants.NUM_REELS; reelIdx++)
            {
                gameData.ReelOffset[reelIdx] = REEL_START_OFFSET * PRECISION;
                gameData.ReelState[reelIdx] = REEL_STATE.STOPPED;
            }
        }

        public static void StartReels(GameData gameData)
        {
            gameData.CurrentRun++;

            long currentTime = GetCurrentTime();
            for (int reelIdx = 0; reelIdx < Constants.NUM_REELS; reelIdx++)
            {
                gameData.ReelState[reelIdx] = REEL_STATE.RUNNING;
                gameData.ReelStartOffset[reelIdx][gameData.CurrentRun] = gameData.ReelOffset[reelIdx];
                gameData.ReelStartTime[reelIdx][gameData.CurrentRun] = currentTime;
            }
        }

        public static void StopAllReels(GameData gameData, SlotsBalance slotsBalance, long currentTime)
        {
            for (int reelIdx = 0; reelIdx < Constants.NUM_REELS; reelIdx++)
                StopReel(gameData, slotsBalance, reelIdx, currentTime);
        }

        public static void StopReel(GameData gameData, SlotsBalance slotsBalance, int reelIdx, long currentTime)
        {
            gameData.ReelState[reelIdx] = REEL_STATE.STOPPED;
            gameData.ReelStopTime[reelIdx][gameData.CurrentRun] = currentTime;
            SetReelOffset(gameData, slotsBalance, reelIdx, currentTime - gameData.ReelStartTime[reelIdx][gameData.CurrentRun]);
        }

        public static void Tick(GameData gameData, SlotsBalance slotsBalance)
        {
            long currentTime = GetCurrentTime();

            for (int reelIdx = 0; reelIdx < Constants.NUM_REELS; reelIdx++)
            {
                if (gameData.ReelState[reelIdx] == REEL_STATE.RUNNING)
                {
                    long runningTime = currentTime - gameData.ReelStartTime[reelIdx][gameData.CurrentRun];
                    SetReelOffset(gameData, slotsBalance, reelIdx, runningTime);
                }
            }
        }

        private static void SetReelOffset(GameData gameData, SlotsBalance slotsBalance, int reelIdx, long runningTime)
        {
            int reelSpeed = slotsBalance.ReelSpeed[reelIdx];
            long index = (int)((runningTime * slotsBalance.StartCurve[reelIdx].Length) / slotsBalance.StartCurveTime[reelIdx]);
            if (runningTime < slotsBalance.StartCurveTime[reelIdx])
            {
                reelSpeed = (reelSpeed * slotsBalance.StartCurve[reelIdx][index]) / SlotsLogic.PRECISION;
                if (reelIdx == 0)
                    Debug.LogFormat("runningTime {0} slotsBalance.ReelSpeed[{1}] {2} slotsBalance.StartCurve[{1}][{3}] {4} reelSpeed {5}", runningTime, reelIdx, slotsBalance.ReelSpeed[reelIdx], index, slotsBalance.StartCurve[reelIdx][index], reelSpeed);
            }

            gameData.ReelOffset[reelIdx] = gameData.ReelStartOffset[reelIdx][gameData.CurrentRun] + ((reelSpeed * runningTime) / PRECISION);

            if (gameData.ReelOffset[reelIdx] > (slotsBalance.NumSymbols + REEL_START_OFFSET) * PRECISION)
                gameData.ReelOffset[reelIdx] -= slotsBalance.NumSymbols * PRECISION;
        }
    }
}