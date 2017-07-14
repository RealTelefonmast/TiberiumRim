using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace TiberiumRim
{
    class MapComponent_Inhibition : MapComponent
    {
        public static List<IntVec3> ProtectedCells = new List<IntVec3>();

        public MapComponent_Inhibition(Map map) : base(map) { }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
        }
    }
}
