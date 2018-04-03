﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace TiberiumRim
{
    public class TiberiumContainer
    {
        public readonly float maxStorage;

        private Dictionary<TiberiumType, float> ContainedTiberium = new Dictionary<TiberiumType, float>();

        public TiberiumContainer(float maxStorage)
        {
            this.maxStorage = maxStorage;
        }

        public Color Color
        {
            get
            {
                Color newColor = new Color();
                foreach(TiberiumType type in ContainedTiberium.Keys)
                {
                    newColor += TypeColor(type) * (ContainedTiberium[type] / maxStorage);
                }
                return newColor;
            }
        }

        public Color TypeColor (TiberiumType type)
        {
            Color color = new Color();
            if(type == TiberiumType.Green)
            {
                color = new Color(100f/255f, 200f/255f, 0f);
            }
            if (type == TiberiumType.Blue)
            {
                color = new Color(0f, 100f/255f, 200f/255f);
            }
            if (type == TiberiumType.Red)
            {
                color = new Color(200f/255f, 0f, 100f/255f);
            }
            return color;
        }

        public TiberiumType MainType
        {
            get
            {
                float maxFlt = 0f;
                TiberiumType tibType = TiberiumType.None;
                foreach(TiberiumType type in ContainedTiberium.Keys)
                {
                    if (maxFlt < ContainedTiberium[type])
                    {
                        maxFlt = ContainedTiberium[type];
                        tibType = type;
                    }
                }
                return tibType;
            }
        }

        public List<TiberiumType> GetTypes
        {
            get
            {
                return ContainedTiberium.Keys.ToList();
            }
        }

        public float ValueForType(TiberiumType type)
        {
            if (ContainedTiberium.ContainsKey(type))
            {
                return ContainedTiberium[type];
            }
            return 0f;
        }

        public float GetTotalStorage
        {
            get
            {
                float flt = 0f;
                foreach(TiberiumType type in ContainedTiberium.Keys)
                {
                    
                    flt += ContainedTiberium[type];
                }
                return flt;
            }
        }

        public bool CapacityFull
        {
            get
            {
                return GetTotalStorage >= maxStorage;
            }
        }

        public void AddCrystal(TiberiumType type, float value, out float leftOver)
        {
            leftOver = 0f;
            if (CapacityFull)
            {
                leftOver = value;
                return;
            }
            if (!ContainedTiberium.ContainsKey(type))
            {
                ContainedTiberium.Add(type, value);
                return;
            }
            if(ContainedTiberium[type] + value > maxStorage)
            {
                value = (ContainedTiberium[type] + value) - ContainedTiberium[type];
            }
            ContainedTiberium[type] += value;
        }

        public void RemoveCrystal(TiberiumType type, float value)
        {
            if (ContainedTiberium.ContainsKey(type))
            {
                ContainedTiberium[type] -= value;
                if(ContainedTiberium[type] < 0)
                {
                    ContainedTiberium.Remove(type);
                }
            }
        }
    }
}