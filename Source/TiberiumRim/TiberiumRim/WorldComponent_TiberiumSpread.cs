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
                Log.Message("should spread");
                AffectSurroundingTiles();
            }
            //gotta change this to ticksperday again       
            if (Find.TickManager.TicksGame % GenDate.TicksPerHour == 0)
            {
                Log.Message("should grow");
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
                        if (!TiberiumTiles.ContainsKey(tmpNeighbors[i]))
                        {
                            if (Find.WorldGrid.tiles[tmpNeighbors[i]].biome.defName.Contains("Ice"))
                            {
                                tmpNeighbors.Remove(tmpNeighbors[i]);
                            }
                            else
                            {
                                if (world.grid.tiles[tmpNeighbors[i]] != Find.AnyPlayerHomeMap.TileInfo)
                                {
                                    Log.Message("affacting neighbours but not colony");
                                    TiberiumTiles.Add(tmpNeighbors[i], MainTCD.MainTiberiumControlDef.WorldCorruptAdder);
                                    tmpNeighbors.Remove(tmpNeighbors[i]);
                                }
                                else
                                {
                                    Log.Message("affacting colony, I think");
                                    TiberiumType type = Rand.Element<TiberiumType>(TiberiumType.Green, TiberiumType.Blue);
                                    Map map = Find.Maps.Find((Map x) => x.TileInfo == world.grid.tiles[tmpNeighbors[i]]);
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
