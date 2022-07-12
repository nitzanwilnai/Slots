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
        public GameData m_gameData;

        public GameObject UIMainMenu;
        public GameObject UIInGame;

        public MENU_STATE MenuState = MENU_STATE.MAIN_MENU;
        public int Seed;

        SlotsBalance m_slotsBalance = new SlotsBalance();


        protected override void Awake()
        {
            base.Awake();

            m_gameData = new GameData();

            SlotsLogic.AllocateGame(m_gameData);
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
                SlotsLogic.Tick(m_gameData, m_slotsBalance);
                SlotsVisual.Tick(m_gameData, m_slotsBalance);
        }

        public void StartGame()
        {
            SlotsLogic.StartGame(m_gameData, m_slotsBalance, ref Seed);
            SlotsVisual.Show(m_gameData, m_slotsBalance, UIInGame.GetComponent<GUIRef>());
            SetMenuState(MENU_STATE.IN_GAME);
        }

        public void ButtonStartReels()
        {
            SlotsLogic.StartReels(m_gameData);
            SlotsVisual.StartReels();
        }

        public void ButtonStopReel(int reelIdx)
        {
            SlotsLogic.StopReel(m_gameData, m_slotsBalance, reelIdx, SlotsLogic.GetCurrentTime());
            SlotsVisual.StopReel(m_gameData, reelIdx);
        }

        public void ButtonStopAllReels()
        {
            SlotsLogic.StopAllReels(m_gameData, m_slotsBalance, SlotsLogic.GetCurrentTime());
            SlotsVisual.StopAllReels();
        }
    }
}