using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld;

namespace TiberiumRim
{
    public class JobGiver_RapairAnyMechWithinRange : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            RepairDrone drone = pawn as RepairDrone;
            return null;
        }
    }

    public class JobDriver_RapairAnyMechWithinRange : JobDriver
    {
        private RepairDrone Drone
        {
            get
            {
                return this.pawn as RepairDrone;
            }
        }

        private Pawn Target
        {
            get
            {
                return this.TargetA.Thing as Pawn;
            }
        }

        public override bool TryMakePreToilReservations()
        {
            return this.pawn.Reserve(this.TargetA, this.job);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            Toil repair = new Toil();
            repair.tickAction = delegate
            {
                if(GenTicks.TicksGame % GenTicks.TickLongInterval == 0)
                {
                    foreach (Hediff hediff in Target.health.hediffSet.hediffs)
                    {
                        hediff.Severity -= Drone.def.repairAmount;
                    }
                }
            };
            repair.FailOnDespawnedOrNull(TargetIndex.A);
            repair.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            repair.defaultCompleteMode = ToilCompleteMode.Never;
            yield return repair;
        }
    }
}
