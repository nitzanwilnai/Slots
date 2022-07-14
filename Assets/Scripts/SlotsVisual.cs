using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlotsGame
{
    public struct BottomButtonsInfo
    {
        public GameObject StartButton;
        public GameObject StopAllButton;
        public Button[] StopReelButton;
    }

    public class SlotsVisual : MonoBehaviour
    {
        public Transform[] ReelTransform;

        Symbol[][] m_reelSymbols;

        public Symbol SymbolPrefab;
        public GameObject Bar;

        TextMeshProUGUI m_scoreText;
        TextMeshProUGUI m_runsText;

        BottomButtonsInfo m_bottomButtonsInfo = new BottomButtonsInfo();

        private void Awake()
        {
            SymbolPrefab.gameObject.SetActive(false);
            Bar.SetActive(false);
        }

        // Start is called before the first frame update
        public void Allocate()
        {
            m_bottomButtonsInfo.StopReelButton = new Button[Constants.NUM_REELS];
            m_reelSymbols = new Symbol[Constants.NUM_REELS][];
            for (int reelIdx = 0; reelIdx < Constants.NUM_REELS; reelIdx++)
            {
                m_reelSymbols[reelIdx] = new Symbol[Constants.MAX_SYMBOLS];
                Vector3 pos = Vector3.zero;
                for (int tileIdx = 0; tileIdx < Constants.MAX_SYMBOLS; tileIdx++)
                {
                    Symbol tile = Instantiate(SymbolPrefab, ReelTransform[reelIdx]);
                    tile.transform.localScale = Vector3.one;
                    pos.y = tileIdx;
                    tile.transform.localPosition = pos;
                    m_reelSymbols[reelIdx][tileIdx] = tile;
                    tile.gameObject.SetActive(false);
                }
            }
        }

        public void Show(SlotsData slotsData, SlotsBalance slotsBalance, GUIRef guiRef)
        {
            for (int reelIdx = 0; reelIdx < Constants.NUM_REELS; reelIdx++)
            {
                Vector3 pos = Vector3.zero;
                int numSymbols = slotsBalance.NumSymbols;
                for (int sIdx = 0; sIdx < numSymbols; sIdx++)
                {
                    m_reelSymbols[reelIdx][sIdx].gameObject.SetActive(true);
                    m_reelSymbols[reelIdx][sIdx].Text.text = slotsBalance.ReelSymbols[reelIdx][sIdx].ToString();
                    ShowReel(slotsData, slotsBalance, reelIdx);
                }
                for (int tileIdx = numSymbols; tileIdx < Constants.MAX_SYMBOLS; tileIdx++)
                    m_reelSymbols[reelIdx][tileIdx].gameObject.SetActive(false);
            }
            Bar.SetActive(true);

            m_bottomButtonsInfo.StartButton = guiRef.GetButton("Start").gameObject;
            m_bottomButtonsInfo.StopAllButton = guiRef.GetButton("StopAll").gameObject;
            m_bottomButtonsInfo.StopReelButton[0] = guiRef.GetButton("Stop1");
            m_bottomButtonsInfo.StopReelButton[1] = guiRef.GetButton("Stop2");
            m_bottomButtonsInfo.StopReelButton[2] = guiRef.GetButton("Stop3");

            guiRef.GetButton("Start").onClick.AddListener(SlotsGame.Instance.ButtonStartReels);
            guiRef.GetButton("StopAll").onClick.AddListener(SlotsGame.Instance.ButtonStopAllReels);
            guiRef.GetButton("Stop1").onClick.AddListener(() => { SlotsGame.Instance.ButtonStopReel(0); });
            guiRef.GetButton("Stop2").onClick.AddListener(() => { SlotsGame.Instance.ButtonStopReel(1); });
            guiRef.GetButton("Stop3").onClick.AddListener(() => { SlotsGame.Instance.ButtonStopReel(2); });

            m_bottomButtonsInfo.StartButton.SetActive(true);
            m_bottomButtonsInfo.StopAllButton.SetActive(false);
            for (int reelIdx = 0; reelIdx < Constants.NUM_REELS; reelIdx++)
                m_bottomButtonsInfo.StopReelButton[reelIdx].interactable = false;

            m_scoreText = guiRef.GetText("Score");
            m_runsText = guiRef.GetText("Runs");
            m_scoreText.text = "Score:";
            m_runsText.text = "Runs:";
        }

        void ShowReel(SlotsData slotsData, SlotsBalance slotsBalance, int reelIdx)
        {
            float reelOffset = slotsData.ReelOffset[reelIdx] / (float)SlotsLogic.PRECISION;
            Vector3 pos = new Vector3(0.0f, reelOffset, 0.0f);
            int numSymbols = slotsBalance.NumSymbols;
            for (int sIdx = 0; sIdx < numSymbols; sIdx++)
            {
                pos.y = -reelOffset + sIdx;
                while (pos.y < -0.5f)
                    pos.y += numSymbols;
                m_reelSymbols[reelIdx][sIdx].transform.localPosition = pos;
            }
        }

        public void Hide()
        {
            for (int reelIdx = 0; reelIdx < Constants.NUM_REELS; reelIdx++)
                for (int tileIdx = 0; tileIdx < Constants.MAX_SYMBOLS; tileIdx++)
                    m_reelSymbols[reelIdx][tileIdx].gameObject.SetActive(false);
            Bar.SetActive(false);
        }

        // Update is called once per frame
        public void Tick(SlotsData slotsData, SlotsBalance slotsBalance)
        {
            for (int reelIdx = 0; reelIdx < Constants.NUM_REELS; reelIdx++)
                ShowReel(slotsData, slotsBalance, reelIdx);
        }

        public void StartReels(SlotsData slotsData)
        {
            m_runsText.text = "Runs: " + slotsData.CurrentRun;

            m_bottomButtonsInfo.StartButton.SetActive(false);
            m_bottomButtonsInfo.StopAllButton.SetActive(true);
            for (int reelIdx = 0; reelIdx < Constants.NUM_REELS; reelIdx++)
                m_bottomButtonsInfo.StopReelButton[reelIdx].interactable = true;
        }

        public void StopAllReels()
        {
            m_bottomButtonsInfo.StartButton.SetActive(true);
            m_bottomButtonsInfo.StopAllButton.SetActive(false);
            for (int reelIdx = 0; reelIdx < Constants.NUM_REELS; reelIdx++)
                m_bottomButtonsInfo.StopReelButton[reelIdx].interactable = false;
        }

        public void StopReel(SlotsData slotsData, int reelIdx, out bool allReelsStopped)
        {
            m_bottomButtonsInfo.StopReelButton[reelIdx].interactable = false;

            allReelsStopped = true;
            for (int i = 0; i < Constants.NUM_REELS; i++)
                if (slotsData.ReelState[i] == REEL_STATE.RUNNING)
                    allReelsStopped = false;

            m_bottomButtonsInfo.StartButton.SetActive(allReelsStopped);
            m_bottomButtonsInfo.StopAllButton.SetActive(!allReelsStopped);
        }

        public void UpdateScore(SlotsData slotsData)
        {
            m_scoreText.text = "Score: " + slotsData.Score;
        }
    }
}