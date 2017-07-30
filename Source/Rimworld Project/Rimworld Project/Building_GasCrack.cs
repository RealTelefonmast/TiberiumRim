using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;

namespace TiberiumRim
{
    public class Building_GasCrack : Building
    {
        private int progressTicks;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
        }

        private float ProgressDays
        {
            get
            {
                return (float)this.progressTicks / 60000f;
            }
        }

        private float CurrentRadius
        {
            get
            {
                return Mathf.Min(15, this.ProgressDays / 1 * 1);
            }
        }

        private bool ShouldBeDying
        {
            get
            {
                return progressTicks > 60000;
            }
        }

        public override void TickRare()
        {
            this.progressTicks += 2500;
            if(ShouldBeDying)
            {
                this.DeSpawn();
                return;
            }
            int num = GenRadial.NumCellsInRadius(this.CurrentRadius);
            for (int i = 0; i < num; i++)
            {
                IntVec3 c = this.Position + GenRadial.RadialPattern[i];
                if (c.InBounds(this.Map))
                {
                    ThingDef gas = DefDatabase<ThingDef>.GetNamed("Gas_Tiberium");
                    if (c.GetFirstThing(Map, gas) == null)
                    {
                        GenSpawn.Spawn(gas, c, Map);
                    }
                }
            }
        }
    }
}
