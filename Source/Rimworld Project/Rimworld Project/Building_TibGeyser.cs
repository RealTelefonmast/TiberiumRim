using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;

namespace TiberiumRim
{
    public class Building_TibGeyser : Building
    {
        private IntermittentSteamSprayer steamSprayer;

        public Building harvester;

        private Sustainer spraySustainer;

        private int radius = 12;

        private int progressTicks;

        private int spraySustainerStartTick = -999;

        private int maxCracks = 24;
        private int currentCracks = 0;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.steamSprayer = new IntermittentSteamSprayer(this);
            this.steamSprayer.startSprayCallback = new Action(this.StartSpray);
            this.steamSprayer.endSprayCallback = new Action(this.EndSpray);
        }

        private void StartSpray()
        {
            SnowUtility.AddSnowRadial(this.OccupiedRect().RandomCell, base.Map, 4f, -0.06f);
            this.spraySustainer = SoundDefOf.GeyserSpray.TrySpawnSustainer(new TargetInfo(base.Position, base.Map, false));
            this.spraySustainerStartTick = Find.TickManager.TicksGame;
        }

        private void EndSpray()
        {
            if (this.spraySustainer != null)
            {
                this.spraySustainer.End();
                this.spraySustainer = null;
            }
        }

        public override void TickLong()
        {
            if (this.harvester == null)
            {
                this.steamSprayer.SteamSprayerTick();
                this.progressTicks += 1;
                if (maxCracks > currentCracks)
                {
                    int num = GenRadial.NumCellsInRadius(this.radius);

                    for (int i = 0; i < num; i++)
                    {
                        IntVec3 positionToCheck = this.Position + GenRadial.RadialPattern[i];
                        this.GasAttack(positionToCheck);
                    }
                }
            }
            if (this.spraySustainer != null && Find.TickManager.TicksGame > this.spraySustainerStartTick + 1000)
            {
                this.spraySustainer.End();
                this.spraySustainer = null;
            }
        }

        public void GasAttack(IntVec3 c)
        {
            if (c.InBounds(Map))
            {
                List<Thing> thinglist = c.GetThingList(this.Map);
                for (int i = 0; i < thinglist.Count; i++)
                {
                    if (thinglist[i] is Pawn)
                    {
                        ThingDef crack = DefDatabase<ThingDef>.GetNamed("TiberiumCrack");
                        if (c.GetFirstThing(Map, crack) == null)
                        {
                            GenSpawn.Spawn(crack, c, Map);
                            currentCracks++;
                        }
                    }
                }
            }
        }
    }

    public class PlaceWorker_OnTiberiumGeyser : PlaceWorker
    {
        ThingDef geyser = DefDatabase<ThingDef>.GetNamed("TiberiumGeyser");
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Thing thingToIgnore = null)
        {
            Thing thing = base.Map.thingGrid.ThingAt(loc, geyser);
            if (thing == null || thing.Position != loc)
            {
                return "MustPlaceOnTiberiumGeyser".Translate();
            }
            return true;
        }

        public override bool ForceAllowPlaceOver(BuildableDef otherDef)
        {
            return otherDef == geyser;
        }
    }

}

