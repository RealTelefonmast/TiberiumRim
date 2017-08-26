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
        public override void TickLong()
        {
            if (Rand.Chance(0.01f))
            {
                int i = Rand.Range(0, 50);
                DamageInfo d = new DamageInfo(DamageDefOf.Deterioration, i);
                this.TakeDamage(d);
                return;
            }
        }
    }
}
