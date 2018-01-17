using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using UnityEngine;

namespace TiberiumRimFactions
{
    public class ThingComp_SuppressionWall : ThingComp
    {
        private CompPowerTrader powerComp;
        private CompProperties_SuppressionWall def;

        private List<IntVec3> cells = new List<IntVec3>();

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            this.powerComp = this.parent.TryGetComp<CompPowerTrader>();
            this.def = (CompProperties_SuppressionWall)this.props;
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            removeCells();
            cells.Clear();
            ThingComp_NodSuppressor.wallPos.Remove(this.parent.Position);
            base.PostDestroy(mode, previousMap);
        }

        public override void PostExposeData()
        {
            removeCells();
            cells.Clear();
            ThingComp_NodSuppressor.wallPos.Remove(this.parent.Position);
            base.PostExposeData();
        }

        public bool OnGrid
        {
            get
            {
                return ThingComp_NodSuppressor.wallPos.Contains(this.parent.Position);
            }
        }

        public void removeCells()
        {
            foreach (IntVec3 v in cells)
            {
                TiberiumRim.CheckLists.SuppressedCells.Remove(v);
            }
        }

        public void getCells()
        {
            var rect = CellRect.CenteredOn(parent.Position, Mathf.RoundToInt(def.radius));
            rect.ClipInsideMap(parent.Map);

            for (int z = rect.minZ; z <= rect.maxZ; z++)
            {
                for (int x = rect.minX; x <= rect.maxX; x++)
                {
                    var c = new IntVec3(x, 0, z);

                    if (GenSight.LineOfSight(this.parent.Position, c, this.parent.Map, true))
                    {
                        if (!TiberiumRim.CheckLists.SuppressedCells.Contains(c) && !cells.Contains(c))
                        {
                            cells.Add(c);
                            TiberiumRim.CheckLists.SuppressedCells.Add(c);
                        }
                    }
                }
            }
        }

        public override void CompTickRare()
        {
            if (!this.OnGrid)
            {
                if (cells.Count > 0)
                    removeCells();
                cells.Clear();
                return;
            }
            else if (cells.Count == 0)
            {
                getCells();
                return;
            }
        }

    }

    public class CompProperties_SuppressionWall : CompProperties
    {
        public int radius;

        public CompProperties_SuppressionWall()
        {
            this.compClass = typeof(ThingComp_SuppressionWall);
        }
    }

    public class PlaceWorker_NodSuppressionWall : PlaceWorker
    {
        public List<IntVec3> intList = new List<IntVec3>();

        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot)
        {
            foreach (IntVec3 c in TiberiumRim.CheckLists.SuppressedCells)
            {
                intList.Add(c);
            }
            GenDraw.DrawFieldEdges(intList);
        }
    }
}
