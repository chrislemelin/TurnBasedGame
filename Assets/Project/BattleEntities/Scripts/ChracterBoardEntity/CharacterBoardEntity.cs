﻿using Placeholdernamespace.Battle.Env;
using Placeholdernamespace.Battle.Interaction;
using Placeholdernamespace.Battle.Managers;
using Placeholdernamespace.Battle.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Placeholdernamespace.Battle.Entities.AttributeStats;
using Placeholdernamespace.Battle.Calculator;
using Placeholdernamespace.Battle.Entities.Skills;
using Placeholdernamespace.Battle.Entities.Passives;
using Placeholdernamespace.Battle.Entities.AI;
using Placeholdernamespace.Common.Animator;
using Placeholdernamespace.Battle.Entities.Instances;
using Placeholdernamespace.Common.UI;
using Placeholdernamespace.Battle.Entities.Kas;

namespace Placeholdernamespace.Battle.Entities
{
    public class CharacterBoardEntity : BoardEntity
    {
        [SerializeField]
        protected CharacterAnimation characterAnimation;
        [SerializeField]
        protected GameObject charactersprite;

        [SerializeField]
        protected CharContainer charContainer;
        
        [SerializeField]
        protected GameObject charKaAura;

        [SerializeField]
        protected CharacterType characterType;
        public CharacterType CharcaterType
        {
            get { return characterType; }
        }
        public void SetCharacterType(CharacterType type)
        {
            characterType = type;
        }

        private bool dying = false;
        public bool Dying
        {
            get { return dying; }
        }

        protected Ka ka;
        public Ka Ka
        {
            get { return ka; }
        }
       

        private static List<CharacterBoardEntity> allCharacterBoardEntities = new List<CharacterBoardEntity>();
        public static List<CharacterBoardEntity> AllCharacterBoardEntities
        {
            get { return new List<CharacterBoardEntity>(allCharacterBoardEntities); }
        }

        [SerializeField]
        private float speed = 5;

        private FloatingTextGenerator floatingTextGenerator;
        public FloatingTextGenerator FloatingTextGenerator
        {
            get { return floatingTextGenerator; }
        }

        protected int? range = Skill.RANGE_ADJACENT;
        public void setRange(int? range)
        {
            this.range = range;
        }
        public int? Range
        {
            get { return range; }
        }

        protected List<Passive> passives = new List<Passive>();
        public List<Passive> Passives
        {
            get {
                List<Passive> returnPassives = new List<Passive>(passives);
                if(ka != null)
                    returnPassives.AddRange(ka.Passives);
                return returnPassives;
            }
        }

        protected List<Skill> skills = new List<Skill>();
        public List<Skill> Skills
        {
            get
            {
                List<Skill> returnSkills = new List<Skill>(skills);
                if (ka != null)
                    returnSkills.AddRange(ka.Skills);
                return returnSkills;
            }
        }

        protected List<Talent> talents = new List<Talent>();
        public List<Talent> Talents
        {
            get { return talents; }
        }

        protected List<TalentTrigger> talentTriggers = new List<TalentTrigger>();
        public List<TalentTrigger> TalentTriggers
        {
            get { return talentTriggers; }
        }

        private BasicAttack basicAttack;
        public BasicAttack BasicAttack
        {
            get { return basicAttack; }
        }

        [SerializeField]
        private EnemyAIBasic enemyAIBasic1;

        //private SkillSelector skillSelector;
        private Tile target = null;
        
        private Dictionary<Tile, Move> cachedMoves = new Dictionary<Tile, Move>();
        private Action<bool> moveDoneCallback;
        private bool initalized = false;
        public bool Initalized
        {
            get { return initalized; }
        }

        public void PartialInit()
        {
            stats.Start(this);
        }

        public override void Init(Position startingPosition, TurnManager turnManager, TileManager tileManager, 
            BoardEntitySelector boardEntitySelector, BattleCalculator battleCalculator, Ka ka = null)
        {
            base.Init(startingPosition, turnManager, tileManager, boardEntitySelector, battleCalculator);

            if (charactersprite != null)
            {
                charactersprite.transform.SetParent(FindObjectOfType<CharacterManagerMarker>().transform);
            }

            allCharacterBoardEntities.Add(this);
            if(enemyAIBasic1 != null)
            {
                enemyAIBasic1.Init(tileManager, this);
            }

            basicAttack = new BasicAttack();
            AddSkill(basicAttack);
            if(charContainer != null)
            {
                charContainer.Init(this);
            }
            floatingTextGenerator = GetComponent<FloatingTextGenerator>();
            this.ka = ka;
            if(ka != null)
            {
                ka.Init(this);
                if(charKaAura != null)
                {
                    //charKaAura.SetActive(true);
                    //Color newColor = new Color(ka.KaColor.r, ka.KaColor.g, ka.KaColor.b, charKaAura.GetComponent<Image>().color.a);
                    //charKaAura.GetComponent<Image>().color = newColor;

                }
            }
            initalized = true;
            foreach (Skill skill in skills)
            {
                InitSkill(skill);
            }
            foreach (Passive passive in passives)
            {
                InitPassive(passive);
            }
            foreach (Passive p in Passives)
            {
                p.StartBattle();
            }
           


        }

        public override List<Move> MoveSet()
        {
            return tileManager.DFSMoves(GetTile().Position, this, team: team, tauntTiles:GetTauntTiles());
        }
        
        public List<SkillModifier> GetSkillModifier(Skill skill)
        {
            List<SkillModifier> skillModifiers = new List<SkillModifier>();
            foreach(Passive passive in Passives)
            {
                skillModifiers.AddRange(passive.GetSkillModifiers(skill));
            }
            return skillModifiers;
        }

        public List<StatModifier> GetStatModifiers()
        {
            List<StatModifier> statModifiers = new List<StatModifier>();
            foreach(Passive passive in Passives)
            {
                statModifiers.AddRange(passive.GetStatModifiers());
            }
            return statModifiers;
        }

       

        private void OnMouseEnter()
        {
            
        }

        public void SetAplha(float aplha)
        {
            charactersprite.GetComponent<ColorEffectManager>().SetAlpha(aplha);
        }

        public void Die()
        {
            foreach(Passive p in Passives)
            {
                p.Die();
            }
            if(stats.GetMutableStat(StatType.Health).Value == 0 )
            {
                SetAnimation(AnimatorUtils.animationType.death);
                dying = true;
                Core.CallbackDelay(.8f, () => DieHelper());
 
            }

        }

        private void DieHelper()
        {
            if(ka == null)
            {
                turnManager.RemoveBoardEntity(this);
                GetTile().SetBoardEntity(null);
                if(TurnManager.CurrentBoardEntity == this)
                {
                    EndMyTurn();
                }
                Destroy(gameObject);
            }
            else
            {
                stats.SetMutableStat(StatType.Health, 1);
                SetAnimation(AnimatorUtils.animationType.idle);
                ka = null;
                charKaAura.SetActive(false);
                FloatingTextGenerator.AddTextDisplay(new TextDisplay() { text = "Lost Support Character" });
            }
      
        }

        private void OnDestroy()
        {

            Destroy(healthBarInstance);
            Destroy(charactersprite);
        }

        private List<Tile> path = new List<Tile>();
        private int pathCounter = 0;
        private int chargeCounter = 0;
        private bool interupted = false;
        private bool interuptClearMovement = false;
        private bool charging = false;
        private bool pushing = false;
        private Position direction = null;
        private Action<int, CharacterBoardEntity> chargeCallback;

        public void ExecutePush(Tile tile, AnimatorUtils.animationDirection direction)
        {
            interupted = false;
            pushing = true;
            pathCounter = 0;

            SetAnimationDirection(direction);
            SetAnimation(AnimatorUtils.animationType.damage);
            
            path.Add(tile);
            tileManager.MoveBoardEntity(tile.Position, this, false);
            ChangeTarget();
        }

        public void ExecuteCharge(Move move, Position direction, Action<int, CharacterBoardEntity> action = null)
        {

            interupted = false;
            charging = true;
            this.direction = direction;
            pathCounter = 0;
            chargeCounter = 0;
            if (characterAnimation != null)
            {
                SetAnimation(AnimatorUtils.animationType.walking);
            }
            moveDoneCallback = null;
            chargeCallback = action; 
            if (move != null)
            {
                OutlineOnHover.disabled = true;
                PathOnClick.pause = true;

                foreach (Passive p in Passives)
                {
                    p.ExecutedMove(move);
                }
                path = move.path;
            }
            ChangeTarget();
        }

        public void ExecuteMove(Move move, Action<bool> action = null)
        {
            pushing = false;
            charging = false;
            interupted = false;
            pathCounter = 0;
            if (characterAnimation != null)
            {
                SetAnimation(AnimatorUtils.animationType.walking);
            }
            moveDoneCallback = action;
            if (move != null)
            {
                OutlineOnHover.disabled = true;
                PathOnClick.pause = true;

                foreach (Passive p in Passives)
                {
                    p.ExecutedMove(move);
                }
                path = move.path;
            }
            ChangeTarget();
        }

        public void InteruptMovment()
        {
            path.Clear();
            interupted = true;
            interuptClearMovement = true;
          
        }

        private void checkAtTarget()
        {
            if (transform.position == target.transform.position)
            {
                Tile leavingTile = GetTile();
                tileManager.MoveBoardEntity(target.Position, this);
                Tile tempTarget = target;
                target = null;
                tempTarget.ExecuteEnterActions(this, leavingTile, ChangeTarget);
            }
        }

        private void doMovement()
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);
        }  

        private void ChangeTarget()
        {

            // we gotta check to see if we just walked into a taunt, we will have to do this for stun as well
            /*if (GetTauntTiles().Count != 0)
            {
                HashSet<Tile> tauntTiles = GetTauntTiles();
                for(int a = 0; a < path.Count; a++)
                {
                    if (tauntTiles.Contains(path[a]))
                    {
                        path.RemoveRange(a, path.Count - a);
                        interupted = true;
                    }
                }
            }
            */

            if(path.Count > 0)
            {
                AnimatorUtils.animationDirection dir = AnimatorUtils.GetAttackDirectionCode(GetTile().Position, path[0].Position);
                if(!pushing)
                {
                    SetAnimationDirection(dir);
                }
                target = path[0];
                path.RemoveAt(0);
                pathCounter++;
                if(charging)
                {
                    Position targetTile = position + direction;
                    Tile t = tileManager.GetTile(targetTile);
                    if(t != null && t.BoardEntity != null && t.BoardEntity.Team != Team)
                    {
                        chargeCounter++;
                        Tile newTile = tileManager.GetTile(targetTile + direction);                        
                        AnimatorUtils.animationDirection pushDir = AnimatorUtils.GetAttackDirectionCode(newTile.Position, position);
                        ((CharacterBoardEntity)t.BoardEntity).ExecutePush(newTile, pushDir);

                    }
                }
            }
            else
            {
                bool display = team == Team.Player;
                if(!charging)
                    stats.SubtractMovementPoints(pathCounter, display);
                if(interuptClearMovement)
                {
                    stats.SetMutableStat(AttributeStats.StatType.Movement, 0);
                    stats.SetMutableStat(AttributeStats.StatType.AP, 0);
                    interuptClearMovement = false;
                }
                // all done moving
                target = null;
                PathOnClick.pause = false;
                OutlineOnHover.disabled = false;
                if (characterAnimation != null)
                {
                    SetAnimation(AnimatorUtils.animationType.idle);
                }
                if (moveDoneCallback != null)
                {
                    Action<bool> tempMoveDoneCallback = moveDoneCallback;
                    moveDoneCallback = null; 
                    tempMoveDoneCallback(interupted);
                }
                
                if (charging)
                {
                    Position targetTile = position + direction;
                    Tile t = tileManager.GetTile(targetTile);
                    if (t != null && t.BoardEntity != null && t.BoardEntity.Team != Team)
                    {
                        chargeCallback(chargeCounter, Core.Instance.convert(t.BoardEntity));
                    }
                    else
                    {
                        chargeCallback(chargeCounter, null);
                    }
                    charging = false;
                }
            }
        }

        void Update()
        {
            if (target != null)
            {
                doMovement();
                checkAtTarget();
            }
        }

        public void SetUpMyTurn()
        {
            stats.NewTurn();
            foreach (Skill skill in Skills)
            {
                skill.StartTurn();
            }
            foreach (Passive passive in Passives)
            {
                passive.StartTurn();

            }
        }

        public override void StartMyTurn()
        {
            bool skipTurn = false;
            foreach(Passive passive in Passives)
            {
                skipTurn = passive.SkipTurn(skipTurn);

            }
            if (skipTurn)
            {
                EndMyTurn();
            }
            else
            {               
                if (team == Team.Enemy)
                {
                    BoardEntity boardEntity = GetRagedBy();
                    enemyAIBasic1.ExecuteTurn(this, EndMyTurn, ragedBy:boardEntity);
                }
            }           
        } 
        
        public void EndMyTurn()
        {
            foreach(Passive p in Passives)
            {
                p.EndTurn();
            }
            foreach (Skill skill in Skills)
            {
                skill.EndTurn();
            }
            turnManager.NextTurn();
        }

        public void ReduceCooldowns()
        {
            foreach(Skill skill in Skills)
            {
                skill.ReduceCooldowns();
            }
        }

        // Passives

        /// <summary>
        /// please use this when adding any type of passive
        /// </summary>
        /// <param name="passive"></param>
        public override void AddPassive(Passive passive)
        {
            if (initalized)
            {
                InitPassive(passive);
            }
            else
            {
                passive.PartialInit(this);
            }
            bool add = true;
            if (passive is Buff)
            {
                add = AddBuff((Buff)passive);
            }
            if (passive is TalentTrigger)
            {
                TalentTriggers.Add((TalentTrigger)passive);
            }
            if (passive is Talent)
            {
                Talents.Add((Talent)passive);
            }
            if(add)
                passives.Add(passive);            
        }

        public void InitPassive(Passive passive)
        {
            passive.Init(battleCalculator, this, tileManager);
        }
        

        public void RemoveBuff<buffClass>()
        {
            foreach(Passive p in Passives)
            {
                if(p is Buff)
                {
                    if(p is buffClass)
                    {
                        ((Buff)p).PopAll();
                    }
                }
            }
        }
        
        public List<Passive> GetTalents()
        {
            List<Passive> talents = new List<Passive>();
            foreach(Passive passive in passives)
            {
                if(passive is Talent)
                {
                    talents.Add(passive);
                }
            }
            return talents;
        }

        public void RemovePassive(Passive passive)
        {
            foreach(Passive p in Passives)
            {
                if(p == passive)
                {
                    passives.Remove(passive);
                }
            }
        }

        public void TriggerTalents()
        {
            foreach(Passive passive in Passives)
            {
                if(passive is Talent)
                {
                    ((Talent)passive).Activate();
                }
            }
        }

        public bool HasPassiveType(PassiveType type)
        {
            foreach (Passive p in Passives)
            {
                if(p.Type == type)
                {
                    return true;
                }
            }
            return false;
        }

        public override void AddSkill(Skill skill)
        {
            if(initalized)
            {
                InitSkill(skill);
            }
            else
            {
                skill.PartialInit(this);
            }
            skills.Add(skill);
        }

        public void InitSkill(Skill skill)
        {
            skill.Init(tileManager, this, battleCalculator, turnManager);
        }

        protected bool AddBuff(Buff buff)
        {
            buff.Init(passives.Remove);
            buff.Added();
            foreach(Passive p in Passives)
            {
                if(buff.GetType() == p.GetType())
                {
                    ((Buff)p).AddSameBuff(buff);
                    return false;
                }
            }
            return true;
        }

        public CharacterBoardEntity GetRagedBy()
        {
            CharacterBoardEntity returnEntity = null;
            foreach(Passive passive in Passives)
            {
                returnEntity = passive.GetRagedBy(returnEntity);
            }
            return returnEntity;
        }

        public HashSet<Tile> GetTauntTiles()
        {
            HashSet<Tile> returnTauntTiles = new HashSet<Tile>();
            foreach (Passive passive in Passives)
            {
                foreach(Tile tile in passive.GetTauntTiles())
                {
                    returnTauntTiles.Add(tile);
                }
            }
            return returnTauntTiles;
        }

        public bool IsStealthed()
        {
            bool stealthed = false;
            foreach(Passive p in Passives)
            {
                stealthed = p.IsStealthed(stealthed);
            }
            return stealthed;
        }

        // ANIMATIONS

        public void SetAnimation(AnimatorUtils.animationType type)
        {
            if (characterAnimation != null && !dying)
            {
                characterAnimation.OnButtonClick((int)type);
            }
        }

        private AnimatorUtils.animationDirection? lastDirection = AnimatorUtils.animationDirection.right;



        public bool ChangeDirection(AnimatorUtils.animationDirection direction)
        {
            HashSet<AnimatorUtils.animationDirection> noRotate = new HashSet<AnimatorUtils.animationDirection>
            {
                AnimatorUtils.animationDirection.right, AnimatorUtils.animationDirection.down
            };
            if(lastDirection == direction)
            {
                return true;
            }
            if (lastDirection != null)
            {
                if (noRotate.Contains((AnimatorUtils.animationDirection)lastDirection) && noRotate.Contains(direction))
                    return true;
            }
            
            return false;
        }

        public void SetAnimationDirection(AnimatorUtils.animationDirection direction)
        {
            if (characterAnimation != null && !dying)
            {
                //bool changeDirection = changeDirection(direction);
                if (direction == AnimatorUtils.animationDirection.left)
                {
                    characterAnimation.On_Front_Back(true);
                    characterAnimation.On_Left_Right(true);
                }
                else if (direction == AnimatorUtils.animationDirection.right)
                {
                    characterAnimation.On_Front_Back(false);
                    characterAnimation.On_Left_Right(false);
                    
                }
                else if (direction == AnimatorUtils.animationDirection.up)
                {
                    characterAnimation.On_Front_Back(false);
                    characterAnimation.On_Left_Right(true);
                }
                else if (direction == AnimatorUtils.animationDirection.down)
                {
                    characterAnimation.On_Front_Back(true);
                    characterAnimation.On_Left_Right(false);
                }
                lastDirection = direction;
            }
        }

      
    }


 
  
    
}
