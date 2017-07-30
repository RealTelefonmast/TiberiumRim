﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

namespace TiberiumRim
{
    public class TiberiumVein : TiberiumCrystal
    {
        private const float minGrowthTemperature = 5;
        private int bledTimes = 0;

        //Veins shouldn't infect pawns directly
        public override void infect(Pawn p)
        {
            return;
        }

        //For aesthetics and the fact that Veins are a living being, let's make it bleed
        public void Bleed(Map map)
        {
            //Only bleed if health is lowered - Only allowing it to bleed a few times to not lag the game
            if (this.HitPoints < this.MaxHitPoints && this.bledTimes < 5)
            {
                IntVec3 pos = this.PositionHeld;
                FilthMaker.MakeFilth(pos, map, ThingDefOf.FilthBlood, 1);
                bledTimes = bledTimes + 1;
            }
        }

        //Veins also corrupt walls
        public override void corruptWall(Building p)
        {
            if (p != null)
            {
                ThingDef wall = DefDatabase<ThingDef>.GetNamed("VeinTiberiumRock_TBNS", true);
                IntVec3 loc = p.Position;

                if (p.def.mineable && !p.def.defName.Contains("TBNS"))
                {
                    GenSpawn.Spawn(wall, loc, Map);
                    return;
                }
            }
            return;
        }

        //Veins don't mutate creatures
        public override void spawnFiendOrVisceroid(IntVec3 pos, Pawn p)
        {
            return;
        }

        //Overriding ticklong for the bleeding
        public override void TickLong()
        {
            base.TickLong();
            Bleed(Map);
        }


        //Not quite sure what to use this for. So far it would only kill Veins that are not connected to an adult Vein. This rarely happens and basically causes unnecessary checks
        /*   public void Networkcheck(Map map)
           {
               if (GenAdjFast.AdjacentCells8Way(this.Position).Any(i => i.GetFirstThing(map, ThingDef.Named("Veinhole_TBNS")) != null))
               {
                   return;
               }
                   if (GenAdjFast.AdjacentCells8Way(this.Position).Any(i =>
                   {
                       var c = i.GetFirstThing(map, this.def);
                       if (c != null)
                       {
                           TiberiumVein plant = c as TiberiumVein;
                           if (plant != null && !plant.HarvestableNow)
                           {
                               return false;
                           }
                           if (plant != null && plant.HarvestableNow)
                           {
                               return true;
                           }
                       }
                       return false;
                   }))
                   {
                       return;
                   }
                   this.Destroy(DestroyMode.Vanish);
           } */
    }
}