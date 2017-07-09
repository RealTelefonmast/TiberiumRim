using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

namespace TiberiumRim
{
    public class Comp_TiberiumProducer : ThingComp
    {
        private CompProperties_TiberiumProducer def;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            this.def = (CompProperties_TiberiumProducer)this.props;

            removeGrass();
            spawnTerrain(def.corruptsInto);
            base.PostSpawnSetup(respawningAfterLoad);
        }

        public override void CompTickRare()
        {
            DestroyWalls();
            base.CompTickRare();
        }

        public void DestroyWalls()
        {
            var c = this.parent.CellsAdjacent8WayAndInside();
            foreach (IntVec3 d in c)
            {
                if (d.InBounds(this.parent.Map))
                {
                    var p = d.GetFirstBuilding(this.parent.Map);

                    if (p != null)
                    {
                        int amt = 150;

                        DamageInfo damage = new DamageInfo(DamageDefOf.Mining, amt);

                        if (!p.def.defName.Contains("TBNS"))
                        {
                            p.TakeDamage(damage);
                        }
                    }
                }
            }
        }

        public void spawnTerrain(TerrainDef setTerrain)
        {
            foreach (IntVec3 inside in this.parent.CellsAdjacent8WayAndInside())
            {
                if (inside.InBounds(this.parent.Map))
                {
                    this.parent.Map.terrainGrid.SetTerrain(inside, setTerrain);
                }
            }
        }

        public void removeGrass()
        {
            foreach (IntVec3 inside in this.parent.CellsAdjacent8WayAndInside())
            {
                if (inside.InBounds(this.parent.Map))
                {
                    Plant plant = inside.GetPlant(this.parent.Map);
                    if (plant != null)
                    {
                        plant.Destroy();
                    }
                }
            }
        }
    }

    class CompProperties_TiberiumProducer : CompProperties
    {

        public TerrainDef corruptsInto;
             
        public CompProperties_TiberiumProducer()
        {
            this.compClass = typeof(Comp_TiberiumProducer);
        }
    }
}
