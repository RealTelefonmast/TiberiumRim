using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
using UnityEngine;

namespace TiberiumRimFactions
{
    public class ThingComp_NodCrane : ThingComp
    {
        private CompPowerTrader powerComp;
        private CompProperties_NodCrane def;
        private bool flag = true;
        public static List<Pawn> pawnList = new List<Pawn>();

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            this.powerComp = this.parent.TryGetComp<CompPowerTrader>();
            this.def = (CompProperties_NodCrane)this.props;

            base.PostSpawnSetup(respawningAfterLoad);
        }

        public bool HasPawns
        {
            get
            {
                return pawnList.Count > 0;
            }
        }

        public int PawnCount
        {
            get
            {
                return pawnList.Count();
            }
        }

        public override void CompTick()
        {
            if (!this.powerComp.PowerOn)
            {
                if (flag)
                {
                    resetStats();
                    flag = false;
                }
            }
            else
            {
                if (!HasPawns)
                {
                    getPawns();
                }
                flag = true;
            }
            base.CompTick();
        }

        public void resetStats()
        {
            foreach (StatModifier statm in this.def.statList)
            {
                foreach (Pawn p in pawnList)
                {
                    p.def.statBases.Remove(statm);
                }
            }
            pawnList.Clear();
        }

        public void getPawns()
        {
            List<Thing> thingList = this.parent.Map.listerThings.AllThings;
            for (int i = 0; i < thingList.Count; i++)
            {
                if (thingList[i] != null && thingList[i] is Pawn)
                {
                    Pawn pawn = thingList[i] as Pawn;
                    if (pawn != null && !pawnList.Contains(pawn))
                    {
                        if (pawn.RaceProps.Humanlike)
                        {
                            if (pawn.Faction.IsPlayer)
                            {
                                pawnList.Add(pawn);
                                pawn.def.statBases.AddRange(this.def.statList);
                            }
                        }
                    }
                }
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo g in base.CompGetGizmosExtra())
            {
                yield return g;
            }

            if (true)
            {
                yield return new Command_Action
                {
                    defaultLabel = "GetCranePawnsLabel".Translate(),
                    defaultDesc = ("GetCranePawnsDesc".Translate()),
                    icon = (ContentFinder<Texture2D>.Get("ui/icons/PawnsSelect", true)),
                    activateSound = SoundDefOf.Click,
                    action = delegate
                    {
                        if (this.powerComp.PowerOn)
                        {
                            getPawns();
                        }
                        else
                        {
                            Messages.Message("CraneNoPower".Translate(), MessageTypeDefOf.RejectInput);
                        }
                    }
                };
            }
        }

        public override string CompInspectStringExtra()
        {
            base.CompInspectStringExtra();
            return PawnCount + "CranePawnCount".Translate();
        }
    }

    public class CompProperties_NodCrane : CompProperties
    {
        public List<StatModifier> statList = new List<StatModifier>();

        public CompProperties_NodCrane()
        {
            this.compClass = typeof(ThingComp_NodCrane);
        }
    }
}
