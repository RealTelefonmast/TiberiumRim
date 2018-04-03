using System.Collections.Generic;
using Verse;

namespace TiberiumRim
{
    [StaticConstructorOnStartup]
    public class TiberiumCrystalDef : ThingDef
    {
        public TiberiumProperties tiberium;

        //Corruption Options
        public TerrainDef defaultTerrain;

        public TerrainDef sandTerrain;

        public TerrainDef stoneTerrain;

        public TerrainDef mossyTerrain;

        public ThingDef monolithDef = new ThingDef();

        public ThingDef corruptedWallDef = new ThingDef();

        public ThingDef corruptedChunkDef = new ThingDef();

        public TiberiumCrystalDef waterType;

        public TiberiumCrystalDef sandType;

        public TiberiumCrystalDef stoneType;

        public List<ThingDef> friendlyTo = new List<ThingDef>();

        public bool ContainsTerraindDef(TerrainDef terrainDef)
        {
            if(terrainDef == defaultTerrain || terrainDef == sandTerrain || terrainDef == stoneTerrain || terrainDef == mossyTerrain)
            {
                return true;
            }
            return false;
        }

        public TiberiumType TibType
        {
            get
            {
                if (this == TiberiumDefOf.TiberiumGreen || this == TiberiumDefOf.TiberiumPod || this == TiberiumDefOf.TiberiumMossGreen || this == TiberiumDefOf.TiberiumShardsGreen || this == TiberiumDefOf.TiberiumVein)
                {
                    return TiberiumType.Green;
                }
                if (this == TiberiumDefOf.TiberiumBlue || this == TiberiumDefOf.TiberiumMossBlue || this == TiberiumDefOf.TiberiumShardsBlue)
                {
                    return TiberiumType.Blue;
                }
                if (this == TiberiumDefOf.TiberiumRed || this == TiberiumDefOf.TiberiumShardsRed)
                {
                    return TiberiumType.Red;
                }
                return TiberiumType.None;
            }
        }

        public new static TiberiumCrystalDef Named(string defName)
        {
            return DefDatabase<TiberiumCrystalDef>.GetNamed(defName, true);
        }
    }
}
