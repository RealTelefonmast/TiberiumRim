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


        public new static TiberiumCrystalDef Named(string defName)
        {
            return DefDatabase<TiberiumCrystalDef>.GetNamed(defName, true);
        }
    }
}
