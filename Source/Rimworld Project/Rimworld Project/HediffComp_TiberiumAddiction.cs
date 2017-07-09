using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace TiberiumRim
{
    public class HediffComp_TiberiumAddiction : HediffComp
    {
        public override void CompPostTick(ref float severityAdjustment)
        {
            if (base.Pawn.IsHashIntervalTick(500))
            {
                if (this.Pawn.CarriedBy == null)
                {
                    Adjust(this.Pawn);
                    return;
                }
            }
        }

        public void Adjust(Pawn pawn)
        {
            if (pawn != null && pawn.Map != null)
            {
                if (pawn.Position.InBounds(pawn.Map))
                {
                    var c = pawn.RandomAdjacentCell8Way();
                    if (c.InBounds(pawn.Map))
                    {
                        var t = c.GetPlant(pawn.Map);
                        Need N = pawn.needs.AllNeeds.Find((Need x) => x.def.defName.Contains("Need_Tiberium"));
                        HediffDef Exposure = DefDatabase<HediffDef>.GetNamed("TiberiumBuildupHediff", true);

                        if (N != null)
                        {
                            if (t != null)
                            {
                                if (t.def.defName.Contains("Tiberium"))
                                {
                                    HealthUtility.AdjustSeverity(pawn, this.parent.def, -this.parent.Severity);
                                    HealthUtility.AdjustSeverity(pawn, this.parent.def, 1 - N.CurLevelPercentage * 0.999999f);

                                    Hediff hediff;
                                    if ((from hd in pawn.health.hediffSet.hediffs where !hd.IsOld() && !hd.def.defName.Contains("Tiberium") select hd).TryRandomElement(out hediff))
                                    {
                                        hediff.Heal(0.2f);
                                    }
                                }
                            }
                        }
                        if (pawn.health.hediffSet.HasHediff(Exposure))
                        {
                            HealthUtility.AdjustSeverity(pawn, Exposure, -0.5f);
                        }
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

