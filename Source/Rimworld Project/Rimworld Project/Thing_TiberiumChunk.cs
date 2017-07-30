using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse;
using RimWorld;

namespace TiberiumRim
{
    public class Thing_TiberiumChunk : Thing
    {
        public override void TickRare()
        {
            if (Rand.Chance(0.1f))
            {
                DamageInfo d = new DamageInfo(DamageDefOf.Deterioration, 7);
                this.TakeDamage(d);
            }
            base.TickRare();
        }
    }
}
