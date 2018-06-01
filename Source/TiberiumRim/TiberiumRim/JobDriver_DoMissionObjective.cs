using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace TiberiumRim
{
    public class JobGiver_DoMissionObjective : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            throw new NotImplementedException();
        }
    }

    public class JobDriver_DoMissionObjective : JobDriver
    {
        public override bool TryMakePreToilReservations()
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            throw new NotImplementedException();
        }
    }
}
