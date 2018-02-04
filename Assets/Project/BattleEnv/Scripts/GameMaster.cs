﻿using Placeholdernamespace.Battle;
using Placeholdernamespace.Battle.Calculator;
using Placeholdernamespace.Battle.Entities;
using Placeholdernamespace.Battle.Env;
using Placeholdernamespace.Battle.Interaction;
using Placeholdernamespace.Battle.Managers;
using Placeholdernamespace.Battle.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Placeholdernamespace.Battle
{
    public class GameMaster : MonoBehaviour
    {

        private static GameMaster instance;
        public static GameMaster Instance
        {
            get { return instance; }
        }

        public TurnManager turnManager;
        public TileManager tileManager;
        public Profile profile;

        public BattleCalculator battleCalulator;
        public BoardEntitySelector boardEntitySelector;
        public TileSelectionManager tileSelectionManager;
        public GameObject Jaz;
        public GameObject Bongani;
        public GameObject Lesidi;
        public GameObject Dadi;
        public GameObject Player1;
        public GameObject Player2;
        public GameObject Enemy1;

        // Use this for initialization
        void Start()
        {
            instance = this;

            tileManager.Init(turnManager, profile);
            GameObject BE;

            BE = Instantiate(Lesidi);
            BE.GetComponent<CharacterBoardEntity>().Init(new Position(2, 2), turnManager, tileManager, boardEntitySelector, battleCalulator);

            BE = Instantiate(Dadi);
            BE.GetComponent<CharacterBoardEntity>().Init(new Position(3, 3), turnManager, tileManager, boardEntitySelector, battleCalulator);

            //BE = Instantiate(Bongani);
            //BE.GetComponent<CharacterBoardEntity>().Init(new Position(1, 1), turnManager, tileManager, boardEntitySelector, battleCalulator);

            //BE = Instantiate(Jaz);
            //BE.GetComponent<CharacterBoardEntity>().Init(new Position(1, 0), turnManager, tileManager, boardEntitySelector, battleCalulator);

            BE = Instantiate(Enemy1);
            BE.GetComponent<CharacterBoardEntity>().Init(new Position(4, 4), turnManager, tileManager, boardEntitySelector, battleCalulator);

            BE = Instantiate(Enemy1);
            BE.GetComponent<CharacterBoardEntity>().Init(new Position(3, 4), turnManager, tileManager, boardEntitySelector, battleCalulator);

            BE = Instantiate(Enemy1);
            BE.GetComponent<CharacterBoardEntity>().Init(new Position(4, 3), turnManager, tileManager, boardEntitySelector, battleCalulator);

            turnManager.init(boardEntitySelector, tileSelectionManager);
            turnManager.ReCalcQueue();
            turnManager.startGame();

        }
    }
}
