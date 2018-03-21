using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace TiberiumRim
{
    public class Comp_RepairDrone : ThingComp
    {
        public List<RepairDrone> repairDrones = new List<RepairDrone>();

        public CompProperties_RepairDrone repairProps;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this.repairProps = (CompProperties_RepairDrone)this.props;
        }

        public override void PostDraw()
        {
            base.PostDraw();
            GenDraw.DrawRadiusRing(this.parent.Position, this.repairProps.radius);
        }
    }

    public class CompProperties_RepairDrone : CompProperties
    {
        public int droneAmount = 1;

        public float radius = 4;

        public CompProperties_RepairDrone()
        {
            this.compClass = typeof(Comp_RepairDrone);
        }
    }
}
