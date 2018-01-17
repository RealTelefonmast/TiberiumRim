using Verse;

namespace TiberiumRimFactions
{
    public class PlaceWorker_AlreadyExists : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null)
        {
            string defName = checkingDef.defName;
            Thing thing = map.listerThings.AllThings.Find((Thing x) => x.def.defName.Contains(defName));
            if (thing != null)
            {
                return checkingDef.label + "ThingAlreadyExists".Translate();
            }
            return true;
        }
    }
}
