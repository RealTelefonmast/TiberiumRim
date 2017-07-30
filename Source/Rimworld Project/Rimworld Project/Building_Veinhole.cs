using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace TiberiumRim
{
    class Building_Veinhole : Building
    {
        List<Thing> plantsChecked = new List<Thing>();

        private int radius = 14;

        //Once a Veinhole dies, all the Veins die with it, for convience and the sake of the players' PCs it just does it instantly. If this starts to cause lags on bigger fields of Veins this will just be left out
        //until a better method is found.
        public static void destroyVeins(List<IntVec3> checkCoords)
        {
            checkCoords.ForEach(i =>
            {
                if (i.InBounds(Find.VisibleMap))
                {
                    Thing thing = i.GetFirstThing(Find.VisibleMap, ThingDef.Named("TiberiumVein"));
                    if (!thing.DestroyedOrNull())
                    {
                        thing.Destroy(DestroyMode.Vanish);
                        IntVec3[] cells = GenAdj.AdjacentCells;
                        destroyVeins(new List<IntVec3>() { i + cells[0], i + cells[1], i + cells[2], i + cells[3], i + cells[4], i + cells[5], i + cells[6], i + cells[7] });
                    }
                }
            });
        }

        public override void TickRare()
        {
            int num = GenRadial.NumCellsInRadius(this.radius);
            for (int i = 0; i < num; i++)
            {
                IntVec3 positionToCheck = this.Position + GenRadial.RadialPattern[i];
                if (positionToCheck.InBounds(Map))
                {
                    if (!positionToCheck.GetTerrain(Map).MadeFromStuff)
                    {
                        this.TryAttack(positionToCheck);
                        TerrainDef t = DefDatabase<TerrainDef>.GetNamed("VeinSoilRed");
                        TerrainDef t2 = DefDatabase<TerrainDef>.GetNamed("TiberiumSoilRed");
                        TerrainDef tt = positionToCheck.GetTerrain(Map);
                        if (tt.defName.Contains("Water") || tt.defName.Contains("Ice") || tt.defName.Contains("Marsh"))
                        {
                            Map.terrainGrid.SetTerrain(positionToCheck, tt);
                        }
                        else if (tt.defName.Contains("Sand"))
                        {
                            Map.terrainGrid.SetTerrain(positionToCheck, t2);
                        }
                        else
                        {
                            Map.terrainGrid.SetTerrain(positionToCheck, t);
                        }
                    }
                }
            }

            spawnVeinmonster();
            base.TickRare();
        }

        private void spawnVeinmonster()
        {
            IntVec3 pos = this.Position.RandomAdjacentCell8Way();

            int maximum = Map.listerThings.AllThings.FindAll((Thing x) => x.def.defName.Contains("Veinhole")).Count * 12;
            int Veinmonsters = Map.listerThings.AllThings.FindAll((Thing x) => x.def.defName.Contains("Veinmonster_TBI")).Count;

            if (Rand.Chance(0.001f) && Veinmonsters < maximum)
            {
                PawnKindDef Veinmonster = DefDatabase<PawnKindDef>.GetNamed("Veinmonster_TBI", true);
                PawnGenerationRequest request = new PawnGenerationRequest(Veinmonster);
                Pawn pawn = PawnGenerator.GeneratePawn(request);
                GenSpawn.Spawn(pawn, pos, Map);
            }
        }

        private void TryAttack(IntVec3 c)
        {
            if (c.InBounds(Map))
            {
                List<Thing> thinglist = c.GetThingList(this.Map);
                for (int i = 0; i < thinglist.Count; i++)
                {
                    if (thinglist[i] is Pawn)
                    {
                        ThingDef tentacle = DefDatabase<ThingDef>.GetNamed("VeinTentacle");
                        if (c.GetFirstThing(Map, tentacle) == null)
                        {
                            GenSpawn.Spawn(tentacle, c, Map);
                        }
                    }
                }
            }
        }

        private void EatPawn()
        {
            List<IntVec3> list = GenAdj.OccupiedRect(this).ExpandedBy(1).EdgeCells.ToList();
            IntVec3 c = list[Rand.Range(0, list.Count)];
            List<Thing> thingList = c.GetThingList(this.Map);

            foreach(Pawn p in thingList)
            {

            }
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            destroyVeins(GenAdjFast.AdjacentCells8Way(this));
            base.Destroy(mode);
        }
    }
}
