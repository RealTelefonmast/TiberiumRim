using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace TiberiumRim
{
    public class RefineryDef : GraphicSwitchableDef
    {
        public PawnKindDef harvester;

        public float refineTime = 3;
    }

    public class Building_Refinery : Building_GraphicSwitchable
    {       
        public new RefineryDef def;

        public List<Harvester> harvesterList = new List<Harvester>();

        public Comp_TNW NetworkComp;

        public int refineTicks;

        public override bool IsFilled
        {
            get
            {
                return false;
            }
        }

        public override bool IsActivated
        {
            get
            {
                return false;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look<RefineryDef>(ref def, "def");
            Scribe_Collections.Look<Harvester>(ref harvesterList, "harvesterList", LookMode.Reference);
            Scribe_Values.Look<int>(ref refineTicks, "refineTicks");
        }

        public override void Tick()
        {
            base.Tick();
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            def = (RefineryDef)base.def;
            NetworkComp = this.GetComp<Comp_TNW>();
            if (!respawningAfterLoad)
            {
                Harvester harvester = SpawnHarvester();
                harvester.UpdateRefineriesOrAddNewMain();

                UpdateOrAddMissingHarvesters();
            }
            this.refineTicks = GenTicks.SecondsToTicks(this.def.refineTime);
        }

        public void UpdateOrAddMissingHarvesters()
        {
            foreach(Harvester harvester in this.Map.listerThings.AllThings.Where((Thing x) => x.def.thingClass == typeof(Harvester)))
            {
                if(harvester != null)
                {
                    if (!this.harvesterList.Contains(harvester))
                    {
                        harvesterList.Add(harvester);
                        harvester.UpdateRefineriesOrAddNewMain();
                    }
                }
            }
        }

        public void SortHarvesterQueue()
        {
            for(int i = 1; i < harvesterList.Count(); i++)
            {
                int j = i;
                while ((j > 0) && (harvesterList[j].CurrentStorage > harvesterList[j-1].CurrentStorage))
                {
                    int k = j - 1;
                    Harvester harvester = harvesterList[k];
                    harvesterList[k] = harvesterList[j];
                    harvesterList[j] = harvester;
                    j--;
                }
            }
        }

        public override void DeSpawn()
        {
            List<Harvester> list = new List<Harvester>();
            list.AddRange(this.harvesterList);            
            foreach(Harvester harvester in list)
            {
                if (harvester.mainRefinery == this)
                {
                    harvester.mainRefinery = null;
                    Messages.Message("RefineryLost".Translate(), this, MessageTypeDefOf.NegativeEvent);
                }
                harvester.availableRefineries.Remove(this);
            }
            base.DeSpawn();
        }

        private Harvester SpawnHarvester()
        {
            Harvester harvester = (Harvester)PawnGenerator.GeneratePawn(this.def.harvester, this.Faction);
            harvester.ageTracker.AgeBiologicalTicks = 0;
            harvester.ageTracker.AgeChronologicalTicks = 0;
            harvester.Rotation = this.Rotation;
            harvester.mainRefinery = this;

            IntVec3 spawnLoc = this.InteractionCell;

            return (Harvester)GenSpawn.Spawn(harvester, spawnLoc, this.Map);
        }

        public bool CanBeRefinedAt
        {
            get
            {
                if (this.GetComp<CompPowerTrader>().PowerOn && !this.IsBrokenDown())
                {
                    if (!NetworkComp.CapacityFull)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
