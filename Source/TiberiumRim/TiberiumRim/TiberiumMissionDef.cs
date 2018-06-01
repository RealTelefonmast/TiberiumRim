using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace TiberiumRim
{
    public class TiberiumMissionDef : Def
    {
        public ResearchProjectDef basePrerequeisite;

        public List<TiberiumMissionDef> prerequisites;

        public List<MissionObjectiveDef> objectives = new List<MissionObjectiveDef>();

        public bool hideOnComplete;

        public bool IsFinished
        {
            get
            {
                return this.objectives.All((MissionObjectiveDef x) => x.IsFinished);
            }
        }

        public bool PrerequisitesCompleted
        {
            get
            {
                if (this.prerequisites != null)
                {
                    for (int i = 0; i < this.prerequisites.Count; i++)
                    {
                        if (!this.prerequisites[i].IsFinished)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        public bool CanStartNow
        {
            get
            {
                return !this.IsFinished && this.PrerequisitesCompleted;
            }
        }
    }
}
