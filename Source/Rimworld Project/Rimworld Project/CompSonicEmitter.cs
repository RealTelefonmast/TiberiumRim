using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using UnityEngine;

namespace TiberiumRim
{
    public class CompSonicEmitter : ThingComp
    {
        private CompPowerTrader powerComp;
        private CompProperties_SonicEmitter def;

        public static List<IntVec3> ProtectedCells = new List<IntVec3>();
        private List<IntVec3> cells = new List<IntVec3>();

        public void removeCells()
        {
            foreach(IntVec3 v in cells)
            {
                MapComponent_Inhibition.ProtectedCells.Remove(v);
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            this.powerComp = this.parent.TryGetComp<CompPowerTrader>();
            this.def = (CompProperties_SonicEmitter)this.props;
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

        public void DictionCheck()
        {
            if (!this.powerComp.PowerOn)
            {
                removeCells();
                cells.Clear();
                return;               
            }
            if (cells.Count == 0)
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
            //TiberiumBase.Instance.logMessage("Ticking");
            if (((double)parent.HitPoints / (double)parent.def.BaseMaxHitPoints > def.damageShutdownPercent) && this.Working)
            {
                //TiberiumBase.Instance.logMessage("Checking plants");
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
                   
                    if (!MapComponent_Inhibition.ProtectedCells.Contains(c))
                    {
                        cells.Add(c);
                        MapComponent_Inhibition.ProtectedCells.Add(c);
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
                        if (p.def.defName.Contains("Tiberium"))
                        {

                            if (GenSight.LineOfSight(this.parent.Position, c, this.parent.Map, true))
                            {
                                //TiberiumBase.Instance.logMessage("This should kill the plant");
                                p.Destroy(DestroyMode.Vanish);
                            }
                        }
                    }
                }
            }
            
            /*
            for (int z = rect.minZ; z <= rect.maxZ; z++)
            {
                for (int x = rect.minX; x <= rect.maxX; x++)
                {
                    var c = new IntVec3(x, 0, z);
                    
                    TiberiumBase.Instance.logMessage("We Can see the Cell");
                    Plant plant = c.GetPlant(this.parent.Map);
                    if (plant != null && plant.def.defName.Contains("Tiberium"))
                    {
                        if (GenSight.LineOfSight(this.parent.Position, c, this.parent.Map, true))
                        {
                            TiberiumBase.Instance.logMessage("This should kill the plant");
                            plant.Destroy(DestroyMode.Vanish);
                        }
                    }
                    
                }
            }
            */
        }
    }

    public class CompProperties_SonicEmitter : CompProperties
    {
        public int radius;
        public float damageShutdownPercent;

        public CompProperties_SonicEmitter()
        {
            this.compClass = typeof(CompSonicEmitter);
        }
    }

    public class PlaceWorker_TiberiumInhibitor : PlaceWorker
    {
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot)
        {
            int radius = def.GetCompProperties<CompProperties_SonicEmitter>().radius;
            List<IntVec3> cells = new List<IntVec3>();

            CellRect rect = CellRect.CenteredOn(center, Mathf.RoundToInt(radius));
            rect.ClipInsideMap(this.Map);
            for (int z = rect.minZ; z <= rect.maxZ; z++)
            {
                for (int x = rect.minX; x <= rect.maxX; x++)
                {
                    var c = new IntVec3(x, 0, z);
                    if (GenSight.LineOfSight(center, c, this.Map, true))
                    {
                        cells.Add(c);
                    }
                }
            }

            GenDraw.DrawFieldEdges(cells);
        }
    }
}
