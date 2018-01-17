using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using RimWorld;

namespace TiberiumRimFactions
{
    public class ThingComp_ScrinShip : ThingComp
    {
        private CompProperties_ScrinShip def;
        private bool spawned;
        private Lord lord;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this.def = (CompProperties_ScrinShip)this.props;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<bool>(ref this.spawned, "spawned", false, false);
        }

        public bool isDamaged
        {
            get
            {
                return this.parent.HitPoints < this.parent.MaxHitPoints;
            }
        }

        public override void CompTickRare()
        {
            if (isDamaged && !spawned)
            {
                SpawnGuys();
                spawned = true;
            }
            base.CompTickRare();
        }

        public void SpawnGuys()
        {
            Pawn pawn;
            PawnKindDef ShockTroop = DefDatabase<PawnKindDef>.GetNamed("Shocktrooper_TBI", true);
            PawnKindDef Buzzer = DefDatabase<PawnKindDef>.GetNamed("Buzzer_TBI", true);
            Faction Scrin = Find.FactionManager.FirstFactionOfDef(FactionDef.Named("Scrin"));
            IntVec3 invalid = IntVec3.Invalid;
            LordJob_MechanoidsDefendShip lordJob = new LordJob_MechanoidsDefendShip(this.parent, Scrin, 21f, invalid);
            this.lord = LordMaker.MakeNewLord(Scrin, lordJob, this.parent.Map, null);

            for (int i = 0; i < 7; i++)
            {
                if (Rand.Chance(0.3f))
                {
                    pawn = PawnGenerator.GeneratePawn(ShockTroop, Scrin);
                    GenSpawn.Spawn(pawn, this.parent.RandomAdjacentCell8Way(), this.parent.Map);
                    this.lord.AddPawn(pawn);
                }
                else
                {
                    pawn = PawnGenerator.GeneratePawn(Buzzer, Scrin);
                    GenSpawn.Spawn(pawn, this.parent.RandomAdjacentCell8Way(), this.parent.Map);
                    this.lord.AddPawn(pawn);
                }
            }
        }
    }

    public class CompProperties_ScrinShip : CompProperties
    {
        public CompProperties_ScrinShip()
        {
            this.compClass = typeof(ThingComp_ScrinShip);
        }
    }
}
