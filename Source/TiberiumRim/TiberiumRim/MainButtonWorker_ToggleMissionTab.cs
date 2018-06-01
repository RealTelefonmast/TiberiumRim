using System;
using Verse;
using RimWorld;

namespace TiberiumRim
{
    public class MainButtonWorker_ToggleMissionTab : MainButtonWorker_ToggleTab
    {
        public override void Activate()
        {
            Find.World.GetComponent<WorldComponent_TiberiumMissions>().AddNewMission(DefDatabase<TiberiumMissionDef>.GetNamed("Mission1_Discovery"));
            base.Activate();
        }

    }
}
