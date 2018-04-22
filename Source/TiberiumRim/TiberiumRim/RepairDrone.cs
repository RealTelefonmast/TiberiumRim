using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace TiberiumRim
{
    public class RepairDroneDef : PawnKindDef
    {
        public float healFloat = 0.1f;
    }

    public class RepairDrone : Mechanical_Pawn
    {
        public new RepairDroneDef kindDef;

        public Comp_RepairDrone parentComp;

        public Building parent;

        private int radialCells;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.kindDef = base.kindDef as RepairDroneDef;
            this.parentComp = this.parent.GetComp<Comp_RepairDrone>();
            this.radialCells = GenRadial.NumCellsInRadius(this.parentComp.repairProps.radius);
        }

        public override void DeSpawn()
        {
            this.parentComp.repairDrones.Remove(this);
            base.DeSpawn();    
        }

        public override void Kill(DamageInfo? dinfo, Hediff exactCulprit = null)
        {
            this.DeSpawn();
        }

        public Mechanical_Pawn AvailableMech
        {
            get
            {
                for (int i = 0; i < radialCells; i++)
                {
                    IntVec3 pos = this.parent.Position + GenRadial.RadialPattern[i];
                    if (pos.InBounds(this.Map))
                    {
                        Pawn pawn = pos.GetFirstPawn(this.Map);
                        if (pawn != null && pawn is Mechanical_Pawn && pawn.health.hediffSet.hediffs.Find((Hediff x) => x.Severity > 0) != null)
                        {
                            return pawn as Mechanical_Pawn;
                        }
                    }
                }
                return null;
            }
        }
    }
}
