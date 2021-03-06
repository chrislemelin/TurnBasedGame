﻿using Placeholdernamespace.Battle.Calculator;
using Placeholdernamespace.Battle.Env;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Placeholdernamespace.Battle.Entities.Passives
{
    public abstract class Buff : Passive
    {
        
        public const int STACKS_HIDDEN = int.MaxValue;
        protected int stacks = STACKS_HIDDEN;

        protected enum AddBuff {add, refresh};
        protected AddBuff addBuffHandle = AddBuff.add;
        protected bool popOneAtTurnEnd = true;


        public int Stacks
        {
            get { return stacks; }
        }
        private Func<Passive, bool> remove;

        public Buff(int stacks = STACKS_HIDDEN) : base()
        {
            this.stacks = stacks;
            type = PassiveType.Buff;
        }

        public virtual void Added()
        {

        }

        public override void StartTurn()
        {
        }
        public override void EndTurn()
        {
            if (popOneAtTurnEnd)
                PopStack();
        }

        public void Init(Func<Passive, bool> remove)
        {
            this.remove = remove;
        }

        protected void Remove()
        {
            if(remove != null)
            {
                RemoveHelper();
                remove(this);
            }
        }

        protected virtual void RemoveHelper(){}

        public void PopStack(int value = 1)
        {
            if (stacks != STACKS_HIDDEN)
            {
                stacks -= value;
                if (stacks <= 0)
                    Remove();
            }
        }

        public virtual void AddSameBuff(Buff b)
        {
            if(GetType() == b.GetType())
            {
                if (addBuffHandle == AddBuff.add)
                    AddStack(b.stacks);
                else
                    SetStacks(b.stacks);
            }
        }

        public void AddStack(int value  = 1)
        {
            if(stacks != STACKS_HIDDEN)
                stacks = stacks + value;
        }

        private void SetStacks(int value)
        {
            if (stacks != STACKS_HIDDEN)
                stacks = value;
        }

        public void PopAll()
        {
            PopStack(stacks);
        }

        public override string GetDescription()
        {
            string descriptionReturn = GetDescriptionHelper();
            string descriptionExtra = GetDescriptionExtra();
            if (descriptionExtra != "")
            {
                descriptionReturn += "\n" + descriptionExtra;
            }
            return descriptionReturn;
        }

        /// <summary>
        /// override for function based description
        /// </summary>
        /// <returns></returns>
        protected virtual string GetDescriptionHelper()
        {
            return description;
        }

        private string GetDescriptionExtra()
        {
            string returnString = "";
            if(stacks != STACKS_HIDDEN)
            {
                returnString += "STACKS: " + stacks;
            }
            return returnString;
        }


    }
}
