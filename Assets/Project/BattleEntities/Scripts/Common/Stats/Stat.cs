﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Placeholdernamespace.Battle.Entities.AttributeStats.Internal;

/// <summary>
/// for EXTERNAL use of the stat module, why are there 2? because the value need to be an int for the final stat determination and needs to be a float
/// when calculating the modifiers and I dont trust myself to always remember to cast it when using it
/// </summary>
namespace Placeholdernamespace.Battle.Entities.AttributeStats
{

    [System.Serializable]
    public class Stat
    {
        [SerializeField]
        private StatType type;
        public StatType Type
        {
            get { return type; }
        }

        [SerializeField]
        private int value;
        public int Value
        {
            get { return value; }
        }

        private bool display;
        public bool Display
        {
            get { return display; }
        }

        public Stat(StatInternal statInternal)
        {
            this.type = statInternal.Type;
            display = statInternal.Display;
            // here to do rounding logic, right now rounding down is happening, might have some wierd float mult errors tho
            value = (int)statInternal.Value;
        }

        public Stat(Stat stat, int newValue)
        {
            type = stat.type;
            display = stat.display;
            value = newValue;
        }      

        public Stat(Stat stat)
        {
            type = stat.type;
            value = stat.value;
            display = stat.display;
        }
    }
}