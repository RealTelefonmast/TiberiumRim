using System;
using System.Linq;
using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;

namespace TiberiumRim
{
    public class PlaceWorker_TNW: PlaceWorker
    {
        private List<IntVec3> cellList = new List<IntVec3>();

        private List<IntVec3> obstructedCells = new List<IntVec3>();

        public Map Map = Current.Game.VisibleMap;
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null)
        {
            FillLists(map);
            if (cellList.Contains(loc))
            {
                return true;
            }
            return false;
        }

        public void FillLists(Map map)
        {
            List<Thing> thingList = map.listerThings.AllThings.FindAll((Thing x) => x.TryGetComp<Comp_TNW>() != null && x.TryGetComp<Comp_TNW>()?.Connector == null);
            if (thingList.Count > 0)
            {
                for (int i = 0; i < thingList.Count; i++)
                {
                    List<IntVec3> cells = thingList[i].CellsAdjacent8WayAndInside().Where((IntVec3 x) => !thingList[i].OccupiedRect().Contains(x) && thingList[i].InteractionCell != x).ToList<IntVec3>();
                    foreach (IntVec3 cell in cells)
                    {
                        if (cell.GetFirstBuilding(map) != null)
                        {
                            if (!obstructedCells.Contains(cell))
                            { 
                                obstructedCells.Add(cell);
                            }
                        }
                        else if (!cellList.Contains(cell))
                        {
                            cellList.Add(cell);
                        }
                    }
                }
            }
        }

        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot)
        {
            FillLists(Map);
            for (int i = 0; i < cellList.Count; i++)
            {
                IntVec3 c = cellList[i];
                CellRenderer.RenderCell(c, 0.44f);             
            }
            for(int i = 0; i < obstructedCells.Count; i++)
            {
                IntVec3 c = obstructedCells[i];
                CellRenderer.RenderCell(c, 1f);
            }
        }
    }
}
