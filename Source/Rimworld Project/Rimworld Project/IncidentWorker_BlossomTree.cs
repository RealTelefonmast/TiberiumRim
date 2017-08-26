using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace TiberiumRim
{
    class IncidentWorker_BlossomTree : IncidentWorker
    {
        IntVec3 cell = IntVec3.Invalid;

        protected override bool CanFireNowSub(IIncidentTarget target)
        {
            Map map = (Map)target;
            Thing Tree = map.listerThings.AllThings.Find((Thing x) => x.def.defName.Contains("BlossomTree"));
            if (Tree != null)
            {
                return true;
            }
            return false;
        }

        public override bool TryExecute(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            IntVec3 intVec;
            if (!RCellFinder.TryFindRandomPawnEntryCell(out intVec, map, CellFinder.EdgeRoadChance_Animal + 0.2f, null))
            {
                return false;
            }
            Pawn pawn = null;

            for (int i = 0; i < 8; i++)
            {
                PawnKindDef creature = selectcreature();
                IntVec3 loc = CellFinder.RandomClosewalkCellNear(intVec, map, 10, null);
                pawn = PawnGenerator.GeneratePawn(creature, null);
                GenSpawn.Spawn(pawn, loc, map, Rot4.Random, false);

                IntVec3 pos = map.listerThings.AllThings.Find((Thing x) => x.def.defName.Contains("BlossomTree")).Position;
                IntVec3 pos2 = pos.RandomAdjacentCell8Way();

                if (Rand.Chance(0.15f))
                {
                    pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.ManhunterPermanent, null, true, false, null);
                }
                else
                pawn.jobs.TryTakeOrderedJob(new Job(JobDefOf.GotoWander, pos2));
            }
            Find.LetterStack.ReceiveLetter(this.def.letterLabel, this.def.letterText, this.def.letterDef, new TargetInfo(cell, map, false), null);
            return true;
        }

        private Pawn FindPawnTarget(Pawn pawn)
        {
            return (Pawn)AttackTargetFinder.BestAttackTarget(pawn, TargetScanFlags.NeedThreat, (Thing x) => x is Pawn && x.def.race.intelligence >= Intelligence.ToolUser, 0f, 9999f, default(IntVec3), 3.40282347E+38f, true);
        }

        public PawnKindDef selectcreature()
        {
            PawnKindDef creature = null;

            switch (Rand.Range(1, 10))
            {
                case 1:
                    creature = DefDatabase<PawnKindDef>.GetNamed("TiberiumTerror_TBI", true);
                    break;

                case 2:
                    creature = DefDatabase<PawnKindDef>.GetNamed("Thrimbo_TBI", true);
                    break;

                case 3:
                    creature = DefDatabase<PawnKindDef>.GetNamed("BigTiberiumFiend_TBI", true);
                    break;


                case 4:
                    creature = DefDatabase<PawnKindDef>.GetNamed("Tibscarab_TBI", true);
                    break;

                case 5:
                    creature = DefDatabase<PawnKindDef>.GetNamed("TiberiumFiend_TBI", true);
                    break;


                case 6:
                    creature = DefDatabase<PawnKindDef>.GetNamed("SmallTiberiumFiend_TBI", true);
                    break;


                case 7:
                    creature = DefDatabase<PawnKindDef>.GetNamed("Crawler_TBI", true);
                    break;

                case 8:
                    creature = DefDatabase<PawnKindDef>.GetNamed("Boomfiend_TBI", true);
                    break;

                case 9:
                    creature = DefDatabase<PawnKindDef>.GetNamed("Tiffalo_TBI", true);
                    break;

                case 10:
                    creature = DefDatabase<PawnKindDef>.GetNamed("Spiner_TBI", true);
                    break;

            }
            return creature;
        }
    }
}
