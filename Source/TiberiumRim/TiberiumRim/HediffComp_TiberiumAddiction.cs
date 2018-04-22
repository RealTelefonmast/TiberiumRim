﻿using System.Linq;
using Verse;
using RimWorld;

namespace TiberiumRim
{
    public class HediffComp_TiberiumAddiction : HediffComp
    {
        public override void CompPostTick(ref float severityAdjustment)
        {
            if (this.Pawn.CarriedBy == null)
            {
                Adjust(this.Pawn);
                return;
            }
        }

        public void Adjust(Pawn pawn)
        {
            if (pawn != null && pawn.Map != null)
            {
                if (pawn.Position.InBounds(pawn.Map))
                {
                    Need_Tiberium N = (Need_Tiberium)pawn.needs.AllNeeds.Find((Need x) => x.def.defName.Contains("Need_Tiberium"));
                    HediffDef Exposure = TiberiumHediffDefOf.TiberiumExposure;
                    if (N != null)
                    {
                        Log.Message("Pct: " + N.CurLevelPercentage + " final pct: " + (1 - N.CurLevelPercentage * 0.999999f));
                        this.parent.Severity = 1 - N.CurLevelPercentage * 0.999999f;
                    }

                    if (pawn.health.hediffSet.HasHediff(Exposure))
                    {
                        HealthUtility.AdjustSeverity(pawn, Exposure, -0.5f);
                    }
                }
            }
        }

        public HediffCompProperties_TiberiumAddiction Props
        {
            get
            {
                return (HediffCompProperties_TiberiumAddiction)this.props;
            }
        }
    }

    public class HediffCompProperties_TiberiumAddiction : HediffCompProperties
    {
        public HediffCompProperties_TiberiumAddiction()
        {
            this.compClass = typeof(HediffComp_TiberiumAddiction);
        }
    }
}

