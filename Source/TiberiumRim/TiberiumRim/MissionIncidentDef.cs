using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;
using RimWorld;

namespace TiberiumRim
{
    public class MissionIncidentDef : IncidentDef
    {
        public List<JobDef> anyJobNeeded = new List<JobDef>();

        public List<ThingDef> thingRequisites = new List<ThingDef>();

        public List<TiberiumMissionDef> missionRequisites = new List<TiberiumMissionDef>();

        public List<TiberiumMissionDef> missionUnlocks;

        public bool CanStart(Map map)
        {
            WorldComponent_TiberiumMissions Missions = Find.World.GetComponent<WorldComponent_TiberiumMissions>();
            if (Missions.Missions.All((Mission x) => !missionUnlocks.Contains(x.def) && !x.active))
            {
                if(Missions.Missions.All((Mission x) => !missionRequisites.Contains(x.def) && !x.def.IsFinished || x.failed))
                {
                    return false;
                }
                if (map.listerThings.AllThings.All((Thing x) => !thingRequisites.Contains(x.def)))
                {
                    return false;
                }
                if (map.mapPawns.AllPawns.Any((Pawn x) => !x.IsColonist && !anyJobNeeded.Contains(x.CurJob.def)))
                {
                    return false;
                }
                return true;
            }
            return false;
        }
    }
}
