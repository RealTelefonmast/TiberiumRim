using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace TiberiumRim
{
    public class IncidentWorker_FiendsArrive : IncidentWorker
    {
        IntVec3 cell = IntVec3.Invalid;

        protected override bool CanFireNowSub(IIncidentTarget target)
        {
            Map map = (Map)target;
            Thing Tree = map.listerThings.AllThings.Find((Thing x) => x.def == TiberiumDefOf.BlossomTree_TBNS);
            if (Tree != null)
            {
                return true;
            }
            return false;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            IntVec3 intVec;
            if (!RCellFinder.TryFindRandomPawnEntryCell(out intVec, map, CellFinder.EdgeRoadChance_Animal, null))
            {
                return false;
            }


            //TODO: balance patch
            Rot4 rot = Rot4.FromAngleFlat((map.Center - intVec).AngleFlat);
            for (int i = 0; i < 12; i++)
            {
                Pawn pawn = null;
                PawnKindDef creature = SelectCreature();
                PawnGenerationRequest request = new PawnGenerationRequest(creature, null, PawnGenerationContext.NonPlayer, map.Tile, false, false, false, false, true, false, 1f, false, true, true);
                pawn = PawnGenerator.GeneratePawn(request);
                IntVec3 loc = CellFinder.RandomClosewalkCellNear(intVec, map, 10, null);
                GenSpawn.Spawn(pawn, loc, map, rot, false);

                IntVec3 pos = map.listerThings.AllThings.Find((Thing x) => x.def == TiberiumDefOf.BlossomTree_TBNS).Position.RandomAdjacentCell8Way();

                if (Rand.Chance(0.15f))
                {
                    pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.ManhunterPermanent, null, true, false, FindPawnTarget(pawn));
                }
                else
                {
                    pawn.jobs.TryTakeOrderedJob(new Job(JobDefOf.GotoWander, pos));
                }
            }
            Find.LetterStack.ReceiveLetter(this.def.letterLabel, this.def.letterText, this.def.letterDef, new TargetInfo(cell, map, false), null);
            Find.TickManager.slower.SignalForceNormalSpeedShort();
            return true;
        }

        private Pawn FindPawnTarget(Pawn pawn)
        {
            return (Pawn)AttackTargetFinder.BestAttackTarget(pawn, TargetScanFlags.NeedThreat, (Thing x) => x is Pawn && x.def.race.intelligence >= Intelligence.ToolUser, 0f, 9999f, default(IntVec3), 3.40282347E+38f, true);
        }

        public PawnKindDef SelectCreature()
        {
            PawnKindDef creature = null;

            switch (Rand.Range(1, 10))
            {
                case 1:
                    if (Rand.Chance(0.3f))
                    {
                        creature = PawnKindDef.Named("TiberiumTerror_TBI");
                    }
                    creature = SelectCreature();
                    break;
                case 2:
                    if (Rand.Chance(0.5f))
                    {
                        creature = PawnKindDef.Named("Thrimbo_TBI");
                    }
                    creature = SelectCreature();
                    break;
                case 3:
                    creature = PawnKindDef.Named("BigTiberiumFiend_TBI");
                    break;
                case 4:
                    creature = PawnKindDef.Named("Tibscarab_TBI");
                    break;
                case 5:
                    creature = PawnKindDef.Named("TiberiumFiend_TBI");
                    break;
                case 6:
                    creature = PawnKindDef.Named("SmallTiberiumFiend_TBI");
                    break;
                case 7:
                    creature = PawnKindDef.Named("Crawler_TBI");
                    break;
                case 8:
                    creature = PawnKindDef.Named("Boomfiend_TBI");
                    break;
                case 9:
                    creature = PawnKindDef.Named("Tiffalo_TBI");
                    break;
                case 10:
                    creature = PawnKindDef.Named("Spiner_TBI");
                    break;
            }
            return creature;
        }
    }
}