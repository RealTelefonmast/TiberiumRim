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

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look<RepairDrone>(ref repairDrones, "repairDrones");
        }

        public override void CompTick()
        {
            if((bool)parent.TryGetComp<CompPowerTrader>()?.PowerOn)
            {
                if (repairDrones.Count < repairProps.droneAmount && Find.TickManager.TicksGame % 750 == 0)
                {
                    repairDrones.Add(SpawnDrone());
                }
            }
            else{RemoveDrones();}
            base.CompTick();
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            RemoveDrones();
            base.PostDestroy(mode, previousMap);
        }

        public void RemoveDrones()
        {
            for (int i = 0; i < repairDrones.Count; i++)
            {
                RepairDrone drone = repairDrones[i];
                if (drone != null)
                {
                    drone.Destroy();
                }
            }
        }

        public RepairDrone SpawnDrone()
        {
            RepairDrone drone = (RepairDrone)PawnGenerator.GeneratePawn(repairProps.droneDef, parent.Faction);
            drone.ageTracker.AgeBiologicalTicks = 0;
            drone.ageTracker.AgeChronologicalTicks = 0;
            drone.Rotation = Rot4.Random;
            drone.parent = this.parent as Building;
            IntVec3 spawnLoc = this.parent.Position;
            return (RepairDrone)GenSpawn.Spawn(drone, spawnLoc, this.parent.Map);
        }

        public override void PostDraw()
        {
            base.PostDraw();
        }
    }

    public class CompProperties_RepairDrone : CompProperties
    {
        public int droneAmount = 1;

        public float radius = 1;

        public PawnKindDef droneDef;

        public CompProperties_RepairDrone()
        {
            this.compClass = typeof(Comp_RepairDrone);
        }
    }
}
