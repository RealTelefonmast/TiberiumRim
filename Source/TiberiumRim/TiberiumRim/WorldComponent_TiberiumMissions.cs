using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace TiberiumRim
{
    public class WorldComponent_TiberiumMissions : WorldComponent
    {
        public List<Building> ActiveBenches = new List<Building>();

        public List<Mission> Missions = new List<Mission>();

        public Dictionary<MissionObjectiveDef, float> activeObjectives = new Dictionary<MissionObjectiveDef, float>();

        public WorldComponent_TiberiumMissions(World world) : base(world)
        {
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
        }

        public void AddNewMission(TiberiumMissionDef mission, Pawn pawn = null)
        {
            Mission newMission = new Mission(mission, pawn);
            Missions.Add(newMission);
        }
    }
}
