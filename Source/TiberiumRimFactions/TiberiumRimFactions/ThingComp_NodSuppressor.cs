using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse;
using RimWorld;

namespace TiberiumRimFactions
{
    public class ThingComp_NodSuppressor : ThingComp
    {
        private CompPowerTrader powerComp;
        private CompProperties_NodSuppressor def;
        public static List<IntVec3> wallPos = new List<IntVec3>();
        private List<IntVec3> thisCells = new List<IntVec3>();

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            this.powerComp = this.parent.TryGetComp<CompPowerTrader>();
            this.def = (CompProperties_NodSuppressor)this.props;

            base.PostSpawnSetup(respawningAfterLoad);
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            removeCells();
            thisCells.Clear();
            base.PostDestroy(mode, previousMap);
        }

        public override void PostExposeData()
        {
            removeCells();
            thisCells.Clear();
            wallPos.Clear();
            base.PostExposeData();
        }

        private bool Working
        {
            get
            {
                return this.powerComp == null || this.powerComp.PowerOn;
            }
        }

        public void RunningCheck()
        {
            if (!this.powerComp.PowerOn)
            {
                if (thisCells.Count > 0)
                    removeCells();
                thisCells.Clear();
                return;
            }
            else if (thisCells.Count == 0)
            {
                getWalls(GenAdjFast.AdjacentCells8Way(this.parent));
                return;
            }
        }

        public override void CompTickRare()
        {
            RunningCheck();
        }

        public void getWalls(List<IntVec3> getCoords)
        {
            getCoords.ForEach(i =>
            {
                if (i.InBounds(Find.VisibleMap))
                {
                    List<Thing> thingList = i.GetThingList(this.parent.Map);
                    foreach (Thing thing in thingList)
                    {
                        if (!thing.DestroyedOrNull())
                        {
                            if (thing.def.defName.Contains("TBNW"))
                            {
                                if (!wallPos.Contains(thing.Position))
                                {
                                    wallPos.Add(thing.Position);
                                    thisCells.Add(thing.Position);
                                    IntVec3[] cells = GenAdj.AdjacentCells;
                                    getWalls(new List<IntVec3>() { i + cells[0], i + cells[1], i + cells[2], i + cells[3], i + cells[4], i + cells[5], i + cells[6], i + cells[7] });
                                }
                            }
                        }
                    }
                }
            });
        }

        public void removeCells()
        {
            foreach (IntVec3 v in thisCells)
            {
                wallPos.Remove(v);
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo g in base.CompGetGizmosExtra())
                yield return g;

            if (true)
            {
                Command_Action Recheck = new Command_Action()
                {
                    defaultLabel = "RecheckWallsLabel".Translate(),
                    defaultDesc = "RecheckWallsDesc".Translate(),
                    icon = ContentFinder<Texture2D>.Get("ui/icons/RecheckWalls", true),
                    activateSound = SoundDefOf.Click
                };
                Recheck.action = delegate
                {
                    removeCells();
                    thisCells.Clear();
                    RunningCheck();
                };
                yield return Recheck;
            }
        }
    }

    public class CompProperties_NodSuppressor : CompProperties
    {
        public CompProperties_NodSuppressor()
        {
            this.compClass = typeof(ThingComp_NodSuppressor);
        }
    }

    public class PlaceWorker_NodSuppressor : PlaceWorker
    {
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot)
        {
            GenDraw.DrawFieldEdges(ThingComp_NodSuppressor.wallPos, Color.magenta);
        }
    }
}