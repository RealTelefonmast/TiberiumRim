using System.Collections.Generic;
using Verse;
using RimWorld;
using RimWorld.Planet;

namespace TiberiumRim
{
    public class WorldComponent_TiberiumSpread : WorldComponent
    {
        private List<int> tmpNeighbors = new List<int>();

        public Dictionary<int, float> TiberiumTiles = new Dictionary<int, float>();

        public WorldComponent_TiberiumSpread(World world) : base(world)
        {
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look<int, float>(ref this.TiberiumTiles, "TiberiumTiles", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look<int>(ref this.tmpNeighbors, "tmpNeighbors", LookMode.Value);
            base.ExposeData();
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            foreach (int tile in TiberiumTiles.Keys)
            {
                if (IsBiomeReady(tile))
                {
                    SetBiome(tile);
                }
            }
        }

        public override void WorldComponentTick()
        {
            base.WorldComponentTick();
            if (Find.TickManager.TicksGame % GenTicks.TickRareInterval == 0)
            { 
                AffectSurroundingTiles();
            }
            
            if (Find.TickManager.TicksGame % GenDate.TicksPerDay == 0)
            {
                GrowBiomes();
            }
        }

        public void AffectSurroundingTiles()
        {
            Dictionary<int, float> tmpDict = new Dictionary<int, float>();
            tmpDict = TiberiumTiles.CopyDictionary();
            foreach(int tile in tmpDict.Keys)
            {
                if (IsTiberiumTile(tile))
                {
                    Find.WorldGrid.GetTileNeighbors(tile, tmpNeighbors);
                    for (int i = 0; i < tmpNeighbors.Count; i++)
                    {
                        int tile2 = tmpNeighbors[i];
                        if (!TiberiumTiles.ContainsKey(tile2))
                        {
                            Tile tile3 = world.grid.tiles[tile2];
                            if (tile3.biome.defName.Contains("Ice"))
                            {
                                tmpNeighbors.Remove(tile2);
                            }
                            else
                            {
                                if (!TiberiumTiles.ContainsKey(tile2))
                                {
                                    TiberiumTiles.Add(tile2, MainTCD.MainTiberiumControlDef.WorldCorruptAdder);
                                    tmpNeighbors.Remove(tile2);
                                }
                            }
                        }
                        else
                        {
                            if(TiberiumTiles[tile2] < MainTCD.MainTiberiumControlDef.WorldCorruptMinPct)
                            if (Rand.Chance(0.001f))
                            {
                                Map map = Find.Maps.Find((Map x) => x.Tile == tile2);
                                if (map != null && map.IsPlayerHome)
                                {
                                    TiberiumType type = Rand.Element(TiberiumType.Green, TiberiumType.Blue);
                                    TiberiumUtility.SpawnTiberiumFromMapEdge(map, type, out TiberiumCrystal crystal);
                                    Messages.Message("TiberiumSpawnFromOutside".Translate(), crystal, MessageTypeDefOf.NeutralEvent);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (IsBiomeReady(tile))
                    {
                        SetBiome(tile);
                    }
                }
            }
        }

        public void GrowBiomes()
        {
            Dictionary<int, float> tmpDict = new Dictionary<int, float>();
            tmpDict = TiberiumTiles.CopyDictionary();
            foreach (int tile in tmpDict.Keys)
            {
                if(TiberiumTiles[tile] < 1f)
                {
                    Tile tile2 = Find.WorldGrid.tiles[tile];
                    if (tile2 != Find.AnyPlayerHomeMap.TileInfo)
                    {
                        float add = MainTCD.MainTiberiumControlDef.WorldCorruptAdder;
                        if (tile2.hilliness == Hilliness.Mountainous || tile2.hilliness == Hilliness.Impassable)
                        {
                            TiberiumTiles[tile] += Rand.Range(0f, add/4);
                        }
                        else
                        if (tile2.temperature < 10)
                        {
                            TiberiumTiles[tile] += Rand.Range(0f, add/2);
                        }
                        else
                        {
                            TiberiumTiles[tile] += Rand.Range(0f, add);
                        }
                        if (TiberiumTiles[tile] >= 1f)
                        {
                            TiberiumTiles[tile] = 1f;
                        }
                    }
                }
            }
        }

        public float GetPct(int tile)
        {
            if (TiberiumTiles.ContainsKey(tile))
            {
                return TiberiumTiles[tile];
            }
            return 0f;
        }

        public bool IsBiomeReady(int tile)
        {
            if (TiberiumTiles.ContainsKey(tile))
            {
                return TiberiumTiles[tile] >= MainTCD.MainTiberiumControlDef.WorldCorruptMinPct;
            }
            return false;
        }

        public bool IsTiberiumTile(int tileInt)
        {
            Tile tile = Find.WorldGrid.tiles[tileInt];
            return tile.biome.defName.Contains("_TB");
        }

        public void SetBiome(int tileID)
        {
            Tile tile = Find.WorldGrid.tiles[tileID];

            if (Find.WorldGrid.tiles[tileID].biome.canBuildBase)
            {
                tile.biome = DefDatabase<BiomeDef>.GetNamed("RedZone_TB");
            }
            else
            {
                tile.biome = DefDatabase<BiomeDef>.GetNamed("GlacierSea_TB");
            }
        }

    }
}
