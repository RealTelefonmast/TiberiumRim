using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace TiberiumRim
{
    public class TiberiumContainer : IExposable
    {
        public readonly float maxStorage;

        private Dictionary<TiberiumType, float> ContainedTiberium = new Dictionary<TiberiumType, float>();

        public TiberiumContainer(float maxStorage)
        {
            this.maxStorage = maxStorage;
        }

        public void ExposeData()
        {
            Scribe_Collections.Look<TiberiumType, float>(ref ContainedTiberium, "ContainedTinerium");
            Log.Message("Scribing Container");
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
            TiberiumControlDef def = MainTCD.MainTiberiumControlDef;
            if(type == TiberiumType.Green)
            {
                color = def.GreenColor;
            }
            if (type == TiberiumType.Blue)
            {
                color = def.BlueColor;
            }
            if (type == TiberiumType.Red)
            {
                color = def.RedColor;
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
