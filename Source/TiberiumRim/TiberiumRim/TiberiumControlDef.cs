using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;

namespace TiberiumRim
{
    public class TiberiumControlDef : Def
    {
        public float TouchInfectionFlt;

        public float GasInfectionFlt;

        public float VeinHitDamage;

        public float TiberiumGrowthRate;

        public float TiberiumLeakScale;

        public float WorldCorruptMinPct;

        public float WorldCorruptAdder;

        public Color GreenColor;

        public Color BlueColor;

        public Color RedColor;

        public Color SludgeColor;

        public static TiberiumControlDef Named(string defName)
        {
            return DefDatabase<TiberiumControlDef>.GetNamed(defName, true);
        }
    }

    [DefOf]
    public static class MainTCD
    {
        public static TiberiumControlDef MainTiberiumControlDef = TiberiumControlDef.Named("MainTiberiumControlDef");
    }
}