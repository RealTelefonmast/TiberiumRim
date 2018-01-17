using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using UnityEngine;
using TiberiumRim;

namespace TiberiumRimFactions
{
    public class ThingComp_SonicInhibition : ThingComp
    {
        private CompPowerTrader powerComp;
        private CompProperties_SonicInhibition def;

        private List<IntVec3> cells = new List<IntVec3>();

        public void removeCells()
        {
            foreach (IntVec3 v in cells)
            {
                CheckLists.ProtectedCells.Remove(v);
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            this.powerComp = this.parent.TryGetComp<CompPowerTrader>();
            this.def = (CompProperties_SonicInhibition)this.props;
            removeCells();
            cells.Clear();
            cacheCells();
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            removeCells();
            cells.Clear();
            base.PostDestroy(mode, previousMap);
        }

        public override void PostExposeData()
        {
            removeCells();
            cells.Clear();
            base.PostExposeData();
        }

        public void DictionCheck()
        {
            if (!this.powerComp.PowerOn)
            {
                if (cells.Count > 0)
                    removeCells();
                cells.Clear();
                return;
            }
            else if (cells.Count == 0)
            {
                cacheCells();
                return;
            }
        }

        private bool Working
        {
            get
            {
                return this.powerComp == null || this.powerComp.PowerOn;
            }
        }

        public override void CompTickRare()
        {
            DictionCheck();
            if (((double)parent.HitPoints / (double)parent.def.BaseMaxHitPoints > def.damageShutdownPercent) && this.Working)
            {
                checkPlantLife();
            }
        }

        public void cacheCells()
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
                        if (!CheckLists.ProtectedCells.Contains(c))
                        {
                            cells.Add(c);
                            CheckLists.ProtectedCells.Add(c);
                        }
                    }
                }
            }
        }

        public void checkPlantLife()
        {
            foreach (IntVec3 c in cells)
            {
                if (c.InBounds(parent.Map))
                {
                    Plant p = c.GetPlant(this.parent.Map);
                    if (p != null)
                    {
                        if (p.def.thingClass == typeof(TiberiumCrystal))
                        {
                            p.Destroy(DestroyMode.Vanish);
                        }
                    }
                }
            }
        }
    }

    public class CompProperties_SonicInhibition : CompProperties
    {
        public int radius;
        public float damageShutdownPercent;

        public CompProperties_SonicInhibition()
        {
            this.compClass = typeof(ThingComp_SonicInhibition);
        }
    }

    public class PlaceWorker_TiberiumInhibitorAll : PlaceWorker
    {
        public Map Map = Current.Game.VisibleMap;
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot)
        {
            Map Map = Current.Game.VisibleMap;
            int radius = def.GetCompProperties<CompProperties_SonicInhibition>().radius;
            List<IntVec3> cells = new List<IntVec3>();
            List<IntVec3> cells2 = new List<IntVec3>();

            if (center.InBounds(Map))
            {
                Thing thisThing = center.GetFirstBuilding(Map);
                CompPowerTrader powerComp = thisThing.TryGetComp<CompPowerTrader>();
                if (thisThing == null || powerComp == null || !powerComp.PowerOn)
                {
                    CellRect rect = CellRect.CenteredOn(center, Mathf.RoundToInt(radius));
                    rect.ClipInsideMap(Map);
                    for (int z = rect.minZ; z <= rect.maxZ; z++)
                    {
                        for (int x = rect.minX; x <= rect.maxX; x++)
                        {
                            var c = new IntVec3(x, 0, z);
                            if (GenSight.LineOfSight(center, c, Map, true))
                            {
                                cells.Add(c);
                            }
                        }
                    }
                    GenDraw.DrawFieldEdges(cells, Color.blue);
                }
                else if (thisThing != null || !powerComp.PowerOn)
                {
                    if (thisThing.def == def)
                    {
                        HashSet<IntVec3> DrawHash = CheckLists.ProtectedCells;
                        foreach (IntVec3 hashCell in DrawHash)
                        {
                            cells2.Add(hashCell);
                        }
                        GenDraw.DrawFieldEdges(cells2, Color.cyan);
                    }
                }
            }
        }
    }
}