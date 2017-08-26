using System;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace TiberiumRim
{
    public class Gas_Tiberium : Gas
    {

        public override void Tick()
        {
            IntVec3 c = this.Position;
            if (c.InBounds(Map))
            {
                List<Thing> thinglist = c.GetThingList(Map);
                foreach (Thing t in thinglist)
                {
                    Pawn p = t as Pawn;
                    if (p != null)
                    {
                        infect(p);
                    }                  
                }
            }
            base.Tick();
        }

        public void infect(Pawn p)
        {
            HediffDef tiberium = DefDatabase<HediffDef>.GetNamed("TiberiumBuildupHediff", true);
            HediffDef addiction = DefDatabase<HediffDef>.GetNamed("TiberiumAddiction", true);
            HediffDef Stage1 = DefDatabase<HediffDef>.GetNamed("TiberiumStage1", true);
            HediffDef Stage2 = DefDatabase<HediffDef>.GetNamed("TiberiumStage2", true);
            HediffDef Stage3 = DefDatabase<HediffDef>.GetNamed("TiberiumContactPoison", true);

            if (p != null)
            {
                if (p.health.hediffSet.HasHediff(Stage1) | p.health.hediffSet.HasHediff(Stage2) | p.health.hediffSet.HasHediff(addiction))
                {
                    return;
                }

                if (p.RaceProps.IsMechanoid)
                {
                    return;
                }

                if (p.def.defName.Contains("_TBI"))
                {
                    return;
                }

                if (p.Position.InBounds(this.Map))
                {
                    List<BodyPartRecord> list = new List<BodyPartRecord>();

                    foreach (BodyPartRecord i in p.RaceProps.body.AllParts)
                    {
                        if (i.depth == BodyPartDepth.Outside && !p.health.hediffSet.PartIsMissing(i))
                        {
                            list.Add(i);
                        }
                    }

                    if (p.apparel == null)
                    {
                        HealthUtility.AdjustSeverity(p, tiberium, +0.006f);
                        return;
                    }

                    List<Apparel> Clothing = p.apparel.WornApparel;

                    int parts = 0;

                    foreach (Apparel a in Clothing)
                    {
                        List<BodyPartGroupDef> covered = a.def.apparel.bodyPartGroups;
                        if (covered.Count > 0)
                        {
                            foreach (BodyPartGroupDef b in covered)
                            {
                                if (p.apparel.BodyPartGroupIsCovered(b))
                                {
                                    if (a.def.defName.Contains("_TBP"))
                                    {
                                        if (a.HitPoints < a.MaxHitPoints * 0.35)
                                        {
                                            HealthUtility.AdjustSeverity(p, tiberium, +0.00015f);
                                            Messages.Message("MessageTiberiumSuitLeak".Translate(), new TargetInfo(p.Position, Map, false), MessageSound.Standard);
                                            return;
                                        }
                                        parts = parts + 1;
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    if (parts < 2)
                    {
                        HealthUtility.AdjustSeverity(p, tiberium, +0.01f);
                        return;
                    }
                }
            }
        }
    }
}
