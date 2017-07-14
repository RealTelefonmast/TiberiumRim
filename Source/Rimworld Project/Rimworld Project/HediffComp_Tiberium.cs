﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace TiberiumRim
{
    public class HediffComp_Tiberium : HediffComp
    {
        public override void CompPostTick(ref float severityAdjustment)
        {
            if (base.Pawn.IsHashIntervalTick(5000))
            {
                CheckSeverity(this.Pawn);
            }
        }

        public HediffCompProperties_Tiberium Props
        {
            get
            {
                return (HediffCompProperties_Tiberium)this.props;
            }
        }

        public void CheckSeverity(Pawn p)
        {
            HediffDef Stage1 = DefDatabase<HediffDef>.GetNamed("TiberiumStage1", true);
            HediffDef Stage2 = DefDatabase<HediffDef>.GetNamed("TiberiumStage2", true);
            Hediff R = p.health.hediffSet.hediffs.Find((Hediff x) => x.def.defName.Contains("TiberiumStage1"));
            HediffDef Addiction = DefDatabase<HediffDef>.GetNamed("TiberiumAddiction", true);


            if (!p.health.hediffSet.HasHediff(Stage1) && !p.health.hediffSet.HasHediff(Stage2) && this.parent.Severity > 0.3 && Rand.Chance(0.3f))
            {
                HealthUtility.AdjustSeverity(p, this.parent.def, -0.2f);
                HealthUtility.AdjustSeverity(p, Stage1, 1.0f);
                return;
            }

            if (!p.health.hediffSet.HasHediff(Stage2) && this.parent.Severity > 0.7 && Rand.Chance(0.5f))
            {
                HealthUtility.AdjustSeverity(p, Stage2, 0.2f);
                p.health.RemoveHediff(this.parent);

                if (R != null)
                {
                    p.health.RemoveHediff(R);
                }
                return;
            }
        }

    }


    public class HediffCompProperties_Tiberium : HediffCompProperties
    {
        public HediffCompProperties_Tiberium()
        {
            this.compClass = typeof(HediffComp_Tiberium);
        }
    }

}
