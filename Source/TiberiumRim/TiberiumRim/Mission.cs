using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace TiberiumRim
{
    public class Mission : IExposable
    {
        public TiberiumMissionDef def;

        public Pawn discoverer;

        public string date;

        public bool failed = false;

        public bool active = true;

        public bool seen = false;

        public Mission(TiberiumMissionDef def, Pawn pawn = null)
        {
            this.def = def;
            this.date = TiberiumUtility.GetCurrentDate();
            this.discoverer = pawn;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref failed, "failed");
            Scribe_Values.Look(ref active, "active");
            Scribe_Values.Look(ref seen, "seen");
            Scribe_Values.Look(ref date, "date");
            Scribe_References.Look(ref discoverer, "discoverer");
            Scribe_Defs.Look(ref def, "def");
        }
    }
}
