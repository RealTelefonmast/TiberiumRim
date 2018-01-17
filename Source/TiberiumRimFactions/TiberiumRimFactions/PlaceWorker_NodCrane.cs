using System.Collections.Generic;
using Verse;

namespace TiberiumRimFactions
{
    public class PlaceWorker_NodCrane : PlaceWorker
    {
        public List<IntVec3> intveclist = new List<IntVec3>();


        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot)
        {
            if (ThingComp_NodCrane.pawnList != null)
            {
                foreach (Pawn p in ThingComp_NodCrane.pawnList)
                {
                    if (p != null)
                    {
                        intveclist.Add(p.Position);
                    }
                }
                GenDraw.DrawFieldEdges(intveclist);
            }
        }
    }
}
