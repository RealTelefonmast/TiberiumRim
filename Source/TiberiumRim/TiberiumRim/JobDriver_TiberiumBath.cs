using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
using RimWorld;

namespace TiberiumRim
{
    public class JobGiver_Tiberium : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            TraverseParms traverseParms = TraverseParms.For(pawn, Danger.Some, TraverseMode.PassDoors, false);
            Thing targetA = TiberiumUtility.ClosestPreferableReachableAndReservableTiberiumForHarvester(pawn, pawn.Position, pawn.Map, null, true, traverseParms, PathEndMode.OnCell);
            JobDef job = DefDatabase<JobDef>.GetNamed("TiberiumBath");
            return new Job(job, targetA);
        }
    }

    public class JobDriver_TiberiumBath : JobDriver
    {
        private Toil bath;

        public override PawnPosture Posture
        {
            get
            {
                return (base.CurToil != this.bath) ? PawnPosture.Standing : PawnPosture.LayingAny;
            }
        }

        public override bool TryMakePreToilReservations()
        {
            if (pawn.CanReserve(this.TargetA))
            {
                return this.pawn.Reserve(this.TargetA, this.job);
            }
            return false;
        }

        [DebuggerHidden]
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
            this.bath = new Toil
            {
                tickAction = delegate
                {
                    if (this.pawn.needs.joy != null)
                    {
                        JoyUtility.JoyTickCheckEnd(this.pawn, JoyTickFullJoyAction.EndJob, 1f);
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Delay,
                defaultDuration = base.job.def.joyDuration
            };
            this.bath.FailOn(() => this.pawn.Position.GetTiberium(this.pawn.Map) != null);
            yield return bath;
        }

    }
}
