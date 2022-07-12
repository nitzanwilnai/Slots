using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SlotsGame
{
    public class SlotsBalance
    {
        public int ID;
        public int NumSymbols;
        public int[] ReelSpeed;
        public int[][] StartCurve;
        public int[] StartCurveTime;
        public SYMBOLS[][] ReelSymbols;

        public void LoadBalance(int ID)
        {
            TextAsset asset = Resources.Load("balance" + ID) as TextAsset;
            LoadBalance(asset.bytes);
        }

        public void LoadBalance(byte[] array)
        {
            Stream s = new MemoryStream(array);
            using (BinaryReader br = new BinaryReader(s))
            {
                int version = br.ReadInt32();

                ID = br.ReadInt32();

                ReelSpeed = new int[Constants.NUM_REELS];
                for (int i = 0; i < Constants.NUM_REELS; i++)
                    ReelSpeed[i] = br.ReadInt32();

                int numCurveSteps = br.ReadInt16();
                StartCurve = new int[Constants.NUM_REELS][];
                for(int reelIdx = 0; reelIdx < Constants.NUM_REELS; reelIdx++)
                {
                    StartCurve[reelIdx] = new int[numCurveSteps];
                    for (int i = 0; i < numCurveSteps; i++)
                        StartCurve[reelIdx][i] = br.ReadInt16();
                }

                StartCurveTime = new int[Constants.NUM_REELS];
                for (int reelIdx = 0; reelIdx < Constants.NUM_REELS; reelIdx++)
                    StartCurveTime[reelIdx] = br.ReadInt32();

                NumSymbols = br.ReadByte();
                ReelSymbols = new SYMBOLS[Constants.NUM_REELS][];
                for (int i = 0; i < Constants.NUM_REELS; i++)
                    ReelSymbols[i] = new SYMBOLS[NumSymbols];
                for (int i = 0; i < Constants.NUM_REELS; i++)
                    for (int j = 0; j < NumSymbols; j++)
                        ReelSymbols[i][j] = (SYMBOLS)br.ReadByte();
            }
        }
    }
}