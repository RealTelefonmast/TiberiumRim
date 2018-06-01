using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace TiberiumRim
{
    public class MissionObjectiveDef : Def
    {
        public ThingDef objective;

        public ObjectiveType oType;

        public List<SkillDef> neededSkills;

        public BuildableDef stationDef;

        public int workCost = 0;

        public int workDone = 0;

        public List<string> images = new List<string>();

        public TiberiumMissionDef ParentMission
        {
            get
            {
                return this.GetMission();
            }
        }

        public bool CanDoObjective(Pawn pawn)
        {
            if (pawn == null)
            {
                return false;
            }
            if (pawn.skills.skills.FindAll((SkillRecord x) => neededSkills.Contains(x.def)).Count < neededSkills.Count)
            {
                return false;
            }
            switch (oType)
            {
                case ObjectiveType.Construct | ObjectiveType.Craft:
                    int num1 = 0;
                    foreach (ThingCountClass c in (objective as ThingDef).costList)
                    {
                        if (Find.AnyPlayerHomeMap.itemAvailability.ThingsAvailableAnywhere(c, pawn))
                        {
                            num1++;
                        }
                        if (num1 >= (objective as ThingDef).costList.Count)
                        {
                            return true;
                        }
                    }
                    break;
                case ObjectiveType.Discover | ObjectiveType.Research:
                    return true;
                case ObjectiveType.Destroy | ObjectiveType.Examine:
                    if (Find.AnyPlayerHomeMap.listerThings.AllThings.Find((Thing x) => x.def == objective) != null)
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }



        public bool IsActive
        {
            get
            {
                return true;
            }
        }

        public bool IsFinished
        {
            get
            {
                if(workCost > 0)
                {
                    if(workDone >= workCost)
                    {
                        return true;
                    }
                }
                if(oType == ObjectiveType.Construct || oType == ObjectiveType.Craft)
                {
                    if(Find.AnyPlayerHomeMap.listerThings.AllThings.Find((Thing x) => x.def == objective) != null)
                    {
                        return true;
                    };
                }
                return false;
            }
        }

        public int WorkLeft
        {
            get
            {
                return workCost - workDone;
            }
        }

        public float ProgressPct
        {
            get
            {
               return workDone / workCost;
            }
        }

        public TiberiumMissionDef GetMission()
        {
            return DefDatabase<TiberiumMissionDef>.AllDefsListForReading.FirstOrDefault((TiberiumMissionDef x) => x.objectives.Contains(this));
        }
    }

    public enum ObjectiveType
    {
        Research,
        Discover,
        Destroy,
        Examine,
        Construct,
        Craft
    }
}
