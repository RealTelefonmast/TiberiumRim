using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld;

namespace TiberiumRim
{
    public class JobGiver_ReturnToRefineryToUnload : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            Harvester harvester = pawn as Harvester;
            if (harvester.AvailableRefineryToUnload != null)
            {
                if (harvester.ShouldUnload && !harvester.Unloading && harvester.CanReserve(harvester.AvailableRefineryToUnload))
                {
                    JobDef job = DefDatabase<JobDef>.GetNamed("ReturnToRefineryToUnload");
                    return new Job(job, harvester.AvailableRefineryToUnload);
                }
            }
            return null;
        }
    }

    public class JobDriver_ReturnToRefineryToUnload : JobDriver
    {
        private Building_Refinery Refinery
        {
            get
            {
                return (Building_Refinery)this.job.targetA.Thing;
            }
        }

        private Harvester Harvester
        {
            get
            {
                return (Harvester)this.pawn;
            }
        }

        private float CurrentStorage;

        private int ticksPassed;

        public override bool TryMakePreToilReservations()
        {
            return this.pawn.Reserve(this.TargetA, this.job);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoCell(Refinery.InteractionCell, PathEndMode.OnCell);
            Toil unload = new Toil();
            unload.initAction = delegate
            {
                Harvester harvester = unload.actor as Harvester;      
                
                CurrentStorage += harvester.Container.GetTotalStorage;
                harvester.pather.StopDead();
            };
            unload.tickAction = delegate
            {
                Harvester actor = unload.actor as Harvester;
                if (!Refinery.NetworkComp.Container.CapacityFull)
                {
                    if(ticksPassed < Refinery.refineTicks)
                    {
                        TiberiumType type = actor.Container.MainType;

                        Refinery.NetworkComp.Container.AddCrystal(type,(CurrentStorage / Refinery.refineTicks), out float flt);
                        actor.Container.RemoveCrystal(type, ((CurrentStorage / Refinery.refineTicks) - flt));
                    }
                    else
                    {
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
            unload.FailOnDespawnedOrNull(TargetIndex.A);
            unload.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            unload.defaultCompleteMode = ToilCompleteMode.Never;
            yield return unload;
        }
    }
}
