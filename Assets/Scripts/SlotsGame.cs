using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SlotsGame
{
    public enum MENU_STATE { MAIN_MENU, IN_GAME };

    // START HERE
    public class SlotsGame : Singleton<SlotsGame>
    {
        public SlotsVisual SlotsVisual; // Assigned through Unity Editor
        public SlotsData m_slotsData; // Player actions in the current game - send to server for validation

        public GameObject UIMainMenu; // TODO - Should come from AssetBundle
        public GameObject UIInGame; // TODO - Should come from AssetBundle 

        public MENU_STATE MenuState = MENU_STATE.MAIN_MENU;

        SlotsBalance m_slotsBalance = new SlotsBalance(); // Balance/Config of our current slot game - should come from Server
        /*
         * NOTE
         * SlotsBalance is binary data that should come from the server, right now it loads it from a file
         * 1. Designer sets the data in a scriptable object, SlotsBalanceSO
         * 2. SlotsBalanceParser converts it to binary data, and should run validation to make sure designers didn't screw anything up
         * 3. To parse the balance, in the top menu go to Slots -> Balance -> Parse Local
         * 4. SlotsBalance loads the file (although in reality it should come from the server, allowing designers to update it live
         */


        protected override void Awake()
        {
            base.Awake();

            m_slotsData = new SlotsData();

            SlotsLogic.AllocateGame(m_slotsData); // allocate all the data we need to run the logic of the game once, at startup
            m_slotsBalance.LoadBalance(1); // load our slots game balance/config - should come from server
        }

        void Start()
        {
            SlotsVisual.Allocate(); // allocate the visual part of our slots game, can be kept in memory or loaded/unloaded as player enters and exits the game
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
                SlotsLogic.Tick(m_slotsData, m_slotsBalance); // does stuff to the data based on logic and balance
            SlotsVisual.Tick(m_slotsData, m_slotsBalance); // shows stuff to the player based on data and balance
        }

        public void StartGame()
        {
            SlotsLogic.StartGame(m_slotsData); // start the game in logic
            SlotsVisual.Show(m_slotsData, m_slotsBalance, UIInGame.GetComponent<GUIRef>()); // show the game visually
            SetMenuState(MENU_STATE.IN_GAME);
        }

        public void ButtonStartReels()
        {
            SlotsLogic.StartReels(m_slotsData); // start all the reels in logic
            SlotsVisual.StartReels(m_slotsData); // start all the reels visually
        }

        public void ButtonStopReel(int reelIdx)
        {
            SlotsLogic.StopReel(m_slotsData, m_slotsBalance, reelIdx, SlotsLogic.GetCurrentTime()); // stop this reel in logic
            bool allReelsStopped;
            SlotsVisual.StopReel(m_slotsData, reelIdx, out allReelsStopped); // stop this reel visually

            if (allReelsStopped)
            {
                SlotsLogic.AddScore(m_slotsData, m_slotsBalance);
                SlotsVisual.UpdateScore(m_slotsData);
                Validate(); // validate our result - this should happen in server
            }
        }

        public void ButtonStopAllReels()
        {
            SlotsLogic.StopAllReels(m_slotsData, m_slotsBalance, SlotsLogic.GetCurrentTime()); // stop all reels in logic
            SlotsVisual.StopAllReels(); // stop all reels visually

            SlotsLogic.AddScore(m_slotsData, m_slotsBalance);
            SlotsVisual.UpdateScore(m_slotsData);

            Validate(); // validate our result - this should happen in server
        }

        private void Validate()
        {
            bool validation = true;
            string result = "Result: ";
            for (int reelIdx = 0; reelIdx < Constants.NUM_REELS; reelIdx++)
            {
                // get the symbol for the current reel offset
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