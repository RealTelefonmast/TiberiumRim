using System;
using Verse;
using RimWorld;

namespace TiberiumRim
{
    public class PlaceWorker_OnTiberiumProducer : PlaceWorker
    {
        private Building_TiberiumProducer producer = null;

        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null)
        {
            Thing thing = map.thingGrid.AnyThingAt(loc, map, typeof(TiberiumProducerDef));            
            if (thing != null)
            {
                IntVec2 size = checkingDef.Size;
                CellRect checkingRect = new CellRect(loc.x - (size.x - 1) / 2, loc.z - (size.z - 1) / 2, size.x, size.z);
                CellRect thingRect = thing.OccupiedRect();

                if ((thing.def.graphicData.drawSize.y != 2) ? (checkingRect.CenterCell == thingRect.CenterCell) : (checkingRect.CenterCell == (thing.TrueCenter().ToIntVec3() + new IntVec3(0,0,1))))
                {
                    producer = (Building_TiberiumProducer)thing;
                    return true;
                }
            }
            return false;
        }

        public override bool ForceAllowPlaceOver(BuildableDef other)
        {
            return other == producer.def;
        }
    }
}
