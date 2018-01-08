﻿using Placeholdernamespace.Battle.Entities;
using Placeholdernamespace.Battle.Env;
using Placeholdernamespace.Battle.Interaction;
using Placeholdernamespace.Battle.Managers;
using Placeholdernamespace.Battle.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Placeholdernamespace.Battle.Interaction
{
    public class BoardEntitySelector :MonoBehaviour{

        [SerializeField]
        private SkillSelector skillSelector;
        public SkillSelector SkillSelector
        {
            get { return SkillSelector; }
        }

        [SerializeField]
        private TileSelectionManager tileSelectionManager;
        public TileSelectionManager TileSelectionManager
        {
            get { return tileSelectionManager; }
        }

        [SerializeField]
        private Profile profile;

        [SerializeField]
        private List<Color> ApCostColors;

        [SerializeField]
        private Color hoverColor;

        private BoardEntity selectedBoardEntity;
        public BoardEntity SelectedBoardEntity
        {
            get { return selectedBoardEntity; }
        }
        
        public void Init()
        {
            tileSelectionManager.Init(profile);
            skillSelector.Init(tileSelectionManager, buildMoveOptions, ()=> setSelectedBoardEntity(null));
        }

        public void setSelectedBoardEntity(BoardEntity boardEntity)
        {
            tileSelectionManager.CancelSelection();
            profile.UpdateProfile(boardEntity);
     
            selectedBoardEntity = boardEntity;
            buildMoveOptions();
            if(boardEntity == null)
            {
                skillSelector.Hide();
            }
        }

        private void buildMoveOptions()
        {
            if (selectedBoardEntity is CharacterBoardEntity)
            {
                if (TurnManager.CurrentBoardEntity == selectedBoardEntity)
                {
                    List<Move> moveSet = selectedBoardEntity.MoveSet();
                    HashSet<Move> usedMoves = new HashSet<Move>();
                    List<TileSelectOption> options = new List<TileSelectOption>();
                    foreach (Move m in moveSet)
                    {
                        Color col = ApCostColors[0];
                        if(m.apCost < ApCostColors.Count)
                        {
                            col = ApCostColors[m.apCost];
                        }
                        options.Add(new TileSelectOption()
                        {
                            Selection = m.destination,
                            OnHover = m.path,
                            HighlightColor = col,
                            HoverColor = hoverColor,
                            ReturnObject = m,
                            OnHoverAction = (() => profile.PreviewMove(selectedBoardEntity, m))
                        });
                    }
                    
                    options.Add(new TileSelectOption()
                    {
                        Selection = selectedBoardEntity.GetTile(),
                        OnHoverAction = (() => profile.UpdateProfile(selectedBoardEntity))
                    });
                    
                    tileSelectionManager.SelectTile(selectedBoardEntity, options, sendMoveToBoardEntity);               
                    skillSelector.SetBoardEntity((CharacterBoardEntity)selectedBoardEntity);
                    skillSelector.SetSkills(selectedBoardEntity.Skills);
                }
                else
                {
                    tileSelectionManager.SelectTile(selectedBoardEntity, new List<Move>() , sendMoveToBoardEntity, null, null);
                }
            }
        }

        private void sendMoveToBoardEntity(TileSelectOption tileOption)
        {
            if (selectedBoardEntity is CharacterBoardEntity)
            {
                if(tileOption != null)
                {
                    ((CharacterBoardEntity)selectedBoardEntity).ExecuteMove((Move)tileOption.ReturnObject);
                }
                else
                {
                    ((CharacterBoardEntity)selectedBoardEntity).ExecuteMove(null);
              
                }
                if(skillSelector.SelectedSkill == null)
                {
                    setSelectedBoardEntity(null);
                    skillSelector.Hide();
                }
            }
        }
    }
}
