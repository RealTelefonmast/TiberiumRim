﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
using RimWorld;

namespace TiberiumRim
{
    public class JobGiver_SearchAndHarvestTiberium : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            Harvester harvester = pawn as Harvester;
            if (harvester.ShouldHarvest && !harvester.Harvesting && !harvester.Unloading)
            {
                TraverseParms traverseParms = TraverseParms.For(harvester, Danger.Some, TraverseMode.PassDoors, false);
                TiberiumCrystal tiberiumTarget = TiberiumUtility.ClosestPreferableReachableAndReservableTiberiumForHarvester(harvester, harvester.Position, harvester.Map, harvester.TiberiumDefToPrefer, traverseParms, PathEndMode.Touch);
                if (tiberiumTarget != null)
                {
                    if (harvester.CanReach(tiberiumTarget, PathEndMode.Touch, Danger.Some, false))
                    {
                        JobDef job = DefDatabase<JobDef>.GetNamed("SearchAndHarvestTiberium");
                        return new Job(job, tiberiumTarget);
                    }
                }
            }
            return null;
        }
    }

    public class JobDriver_SearchAndHarvestTiberium : JobDriver
    {
        protected TiberiumCrystal Tiberium
        {
            get
            {
                return (TiberiumCrystal)this.job.targetA.Thing;
            }
        }

        protected Harvester Harvester
        {
            get
            {
                return this.pawn as Harvester;
            }
        }

        private int ticksPassed;

        private int newTicks;
        private float growth;

        public override bool TryMakePreToilReservations()
        {
            if (pawn.CanReserve(this.TargetA))
            {
                return this.pawn.Reserve(this.TargetA, this.job);
            }
            return false;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            Toil harvest = new Toil();
            harvest.initAction = delegate
            {
                growth += Tiberium.growthInt;
                newTicks += Tiberium.harvestTicks;
            };
            harvest.tickAction = delegate
            {
                Harvester actor = (Harvester)harvest.actor;
                if (!actor.Container.CapacityFull)
                {
                    if (ticksPassed < Tiberium.harvestTicks)
                    {
                        actor.Container.AddCrystal(Tiberium.def.TibType, (growth / newTicks) * Tiberium.def.tiberium.maxHarvestValue , out float flt);
                        Tiberium.growthInt -= (growth / newTicks) - (flt / Tiberium.def.tiberium.maxHarvestValue); 
                    }
                    else
                    {
                        if (Tiberium != null)
                        {
                            Tiberium.Destroy();                          
                        }                     
                        this.EndJobWith(JobCondition.InterruptOptional);
                        return;                       
                    }
                }
                else
                {
                    this.EndJobWith(JobCondition.InterruptForced);
                    return;
                }
                ticksPassed++;
            };
            harvest.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            harvest.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            harvest.defaultCompleteMode = ToilCompleteMode.Never;
            yield return harvest;
        }
    }
}
