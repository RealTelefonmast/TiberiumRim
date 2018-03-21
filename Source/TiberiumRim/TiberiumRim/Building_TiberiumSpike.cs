using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace TiberiumRim
{
    public class Building_TiberiumSpike : Building_GraphicSwitchable
    {
        private Building_TiberiumGeyser geyser;

        public override bool IsFilled
        {
            get
            {
                return false;
            }
        }

        public override bool IsActivated
        {
            get
            {
                return false;
            }
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            bool flag = this.geyser == null;
            if (flag)
            {
                ThingDef def = ThingDef.Named("TiberiumGeyser");
                this.geyser = (Building_TiberiumGeyser)Map.thingGrid.ThingAt(Position, def);
                geyser.harvester = this;
            }
        }

        public override void DeSpawn()
        {
            base.DeSpawn();
            bool flag = this.geyser != null;
            if (flag)
            {
                this.geyser.harvester = null;
            }
        } 
    }
}
