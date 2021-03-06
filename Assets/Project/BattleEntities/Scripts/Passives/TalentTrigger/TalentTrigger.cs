﻿
using Placeholdernamespace.Battle.Calculator;
using Placeholdernamespace.Battle.Env;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Placeholdernamespace.Battle.Entities.Passives
{
    public abstract class TalentTrigger : Passive
    {
        public TalentTrigger() :base()
        {
            type = PassiveType.TalentTrigger;
        }

        public override string GetTitle()
        {
            return title;
        }

        public override string GetDescription()
        {
            return "<color=red>Talent Trigger</color> : "+description ;
        }



        public void Trigger()
        {
            boardEntity.FloatingTextGenerator.AddTextDisplay(new Common.UI.TextDisplay() { target = boardEntity, text = "talent trigger" });
            boardEntity.TriggerTalents();
        }

    }
}
