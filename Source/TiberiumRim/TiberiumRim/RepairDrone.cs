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
    public class RepairDroneDef : ThingDef
    {
        public float repairAmount;
    }

    public class RepairDrone : Pawn
    {
        public new RepairDroneDef def;

        public Comp_RepairDrone parentComp;

        public Building parent;

        public int radiusCells;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.radiusCells = GenRadial.NumCellsInRadius(this.parentComp.repairProps.radius);
        }


        public bool MechAvailable
        {
            get
            {
                for (int i = 0; i < radiusCells; i++)
                {
                    IntVec3 pos = this.parent.Position + GenRadial.RadialPattern[i];
                    if (pos.InBounds(this.Map))
                    {
                        return pos.GetFirstPawn(this.Map) != null;
                    }
                }
                return false;
            }
        }

        public Pawn AvailableMech
        {
            get
            {
                return null;
            }
        }

    }
}
