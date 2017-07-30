using System;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;

namespace TiberiumRim
{
    [StaticConstructorOnStartup]
    public class TiberiumCrystal : Thing
    {
        public const float BaseGrowthPercent = 0.05f;
        private const float GridPosRandomnessFactor = 0.3f;
        protected float growthInt = 0.05f;
        protected int ageInt;
        protected int unlitTicks;
        protected int madeLeaflessTick = -99999;
        private string cachedLabelMouseover;

        public virtual float Growth
        {
            get
            {
                return this.growthInt;
            }
            set
            {
                this.growthInt = value;
                this.cachedLabelMouseover = null;
            }
        }

        public virtual int Age
        {
            get
            {
                return this.ageInt;
            }
            set
            {
                this.ageInt = value;
                this.cachedLabelMouseover = null;
            }
        }

        public virtual float GrowthRate
        {
            get
            {
                return this.GrowthRateFactor_Fertility * this.GrowthRateFactor_Temperature * this.GrowthRateFactor_Light;
            }
        }



    }
}
