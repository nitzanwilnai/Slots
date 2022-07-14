using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SlotsGame
{
    public enum MENU_STATE {  MAIN_MENU, IN_GAME };

    public class SlotsGame : Singleton<SlotsGame>
    {
        public SlotsVisual SlotsVisual;
        public SlotsData m_slotsData;

        public GameObject UIMainMenu;
        public GameObject UIInGame;

        public MENU_STATE MenuState = MENU_STATE.MAIN_MENU;
        public int Seed;

        SlotsBalance m_slotsBalance = new SlotsBalance();


        protected override void Awake()
        {
            base.Awake();

            m_slotsData = new SlotsData();

            SlotsLogic.AllocateGame(m_slotsData);
            m_slotsBalance.LoadBalance(1);
        }

        // Start is called before the first frame update
        void Start()
        {
            SlotsVisual.Allocate();
            SetMenuState(MENU_STATE.MAIN_MENU);
        }

        public void SetMenuState(MENU_STATE newMenuState)
        {
            MenuState = newMenuState;
            UIMainMenu.SetActive(MenuState == MENU_STATE.MAIN_MENU);
            UIInGame.SetActive(MenuState == MENU_STATE.IN_GAME);
        }

        // Update is called once per frame
        void Update()
        {
            if (MenuState == MENU_STATE.IN_GAME)
                SlotsLogic.Tick(m_slotsData, m_slotsBalance);
                SlotsVisual.Tick(m_slotsData, m_slotsBalance);
        }

        public void StartGame()
        {
            SlotsLogic.StartGame(m_slotsData);
            SlotsVisual.Show(m_slotsData, m_slotsBalance, UIInGame.GetComponent<GUIRef>());
            SetMenuState(MENU_STATE.IN_GAME);
        }

        public void ButtonStartReels()
        {
            SlotsLogic.StartReels(m_slotsData);
            SlotsVisual.StartReels(m_slotsData);
        }

        public void ButtonStopReel(int reelIdx)
        {
            SlotsLogic.StopReel(m_slotsData, m_slotsBalance, reelIdx, SlotsLogic.GetCurrentTime());
            bool allReelsStopped;
            SlotsVisual.StopReel(m_slotsData, reelIdx, out allReelsStopped);

            if (allReelsStopped)
            {
                SlotsLogic.AddScore(m_slotsData, m_slotsBalance);
                SlotsVisual.UpdateScore(m_slotsData);
                Validate();
            }
        }

        public void ButtonStopAllReels()
        {
            SlotsLogic.StopAllReels(m_slotsData, m_slotsBalance, SlotsLogic.GetCurrentTime());
            SlotsVisual.StopAllReels();

            SlotsLogic.AddScore(m_slotsData, m_slotsBalance);
            SlotsVisual.UpdateScore(m_slotsData);

            Validate();
        }

        private void Validate()
        {
            bool validation = true;
            string result = "Result: ";
            for (int reelIdx = 0; reelIdx < Constants.NUM_REELS; reelIdx++)
            {
                long offset = m_slotsData.ReelOffset[reelIdx];
                int sIdx = SlotsLogic.GetSymbolIndexForReel(m_slotsBalance, offset);
                result += string.Format("{1} ", reelIdx, m_slotsBalance.ReelSymbols[reelIdx][sIdx].ToString());

                // validate offset with running time
                long runningTime = m_slotsData.ReelStopTime[reelIdx][m_slotsData.CurrentRun] - m_slotsData.ReelStartTime[reelIdx][m_slotsData.CurrentRun];
                long offset2 = SlotsLogic.GetReelOffset(m_slotsData, m_slotsBalance, reelIdx, runningTime);
                if (offset != offset2)
                    validation = false;
            }

            result += "Validation: " + validation.ToString();

            Debug.Log(result);
        }
    }
}