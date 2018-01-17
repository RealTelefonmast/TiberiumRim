using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;
using TiberiumRim;

namespace TiberiumRimFactions
{
    public class HediffComp_TiberBlock : HediffComp
    {
        public bool infectionRemoved = false;

        public bool canTurn = true;

        public bool canPoison = true;

        public HediffCompProperties_TiberiumDrug Props
        {
            get
            {
                return (HediffCompProperties_TiberiumDrug)this.props;
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (base.Pawn.IsHashIntervalTick(GenTicks.TickRareInterval))
            {
                Hediff hediff = HediffUtils.tibSickness(this.Pawn);
                doBlock(this.Pawn, hediff);
            }
        }

        public void doBlock(Pawn ingester, Hediff hediff)
        {
            if (Rand.Chance(0.95f))
            {
                canTurn = false;
                if (hediff != null)
                {
                    canPoison = false;
                    if (hediff.def == TiberiumHediffDefOf.TiberiumContactPoison)
                    {
                        float sev = hediff.Severity;
                        hediff.Severity -= 0.15f;
                        if (sev - 0.15f <= 0)
                        {
                            infectionRemoved = true;
                        }
                        return;
                    }
                    if (infectionRemoved)
                    {
                        BodyPartRecord target = hediff.Part;
                        HediffDef Posthediff = DefDatabase<HediffDef>.GetNamed("PostTiberiumPoisoning", true);
                        ingester.health.AddHediff(Posthediff, target, null);
                        infectionRemoved = false;
                    }
                    if (hediff.def == TiberiumHediffDefOf.TiberiumStage1)
                    {
                        hediff.Severity -= 0.15f;
                        return;
                    }
                    if (hediff.def == TiberiumHediffDefOf.TiberiumStage2)
                    {
                        hediff.Severity -= 0.18f;
                        return;
                    }
                }
                else if (canPoison)
                {
                    DamageInfo damage = new DamageInfo(DamageDefOf.Stun, 15);
                    HediffDef hediff2 = DefDatabase<HediffDef>.GetNamed("TiberblockPoisoning", true);
                    ingester.TakeDamage(damage);
                    ingester.health.AddHediff(hediff2);
                    this.parent.Severity = 0f;
                    return;
                }
            }
            else if (hediff != null && hediff.def == TiberiumHediffDefOf.TiberiumContactPoison && canTurn)
            {
                Pawn pawn = null;
                IntVec3 pos = ingester.Position;
                Map map = ingester.Map;
                PawnKindDef Visceroid = DefDatabase<PawnKindDef>.GetNamed("Visceroid_TBI", true);
                Visceroid.label = ingester.Label;
                PawnGenerationRequest request = new PawnGenerationRequest(Visceroid);
                pawn = PawnGenerator.GeneratePawn(request);
                if (ingester.apparel != null)
                {
                    ingester.apparel.DropAll(ingester.Position);
                }
                ingester.Destroy(DestroyMode.Vanish);
                GenSpawn.Spawn(pawn, pos, map);
                Messages.Message("TiberblockVisceroidSpawn".Translate(), MessageTypeDefOf.RejectInput);
                return;
            }
            else
            {
                this.parent.Severity = 0f;
            }
        }
    }

    public class HediffCompProperties_TiberiumDrug : HediffCompProperties
    {
        public HediffCompProperties_TiberiumDrug()
        {
            this.compClass = typeof(HediffComp_TiberBlock);
        }
    }
}