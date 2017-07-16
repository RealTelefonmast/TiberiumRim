using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Verse;

namespace TiberiumRim
{

    [HarmonyPatch(typeof(GameCondition_ToxicFallout), "DoCellSteadyEffects")]
    public static class ToxicFalloutPatch
    {
        [HarmonyPrefix]
        static bool PrefixMethod(GameCondition_ToxicFallout __instance, IntVec3 c)
        {
            var map = Traverse.Create(__instance).Property("Map").GetValue<Map>();
            if (c.InBounds(map) && !c.Roofed(map))
            {
                List<Thing> thingList = c.GetThingList(map);
                for (int i = 0; i < thingList.Count; i++)
                {
                    Thing thing = thingList[i];
                    if (thing.def.defName.Contains("Tiberium"))
                    {
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }

    }

    [HarmonyPatch(typeof(AutoBuildRoofAreaSetter))]
    [HarmonyPatch("TryGenerateAreaNow")]
    [HarmonyPatch(new Type[] { typeof(Room) })]
    class AutoBuildroofAreaPatch1
    {
        [HarmonyPrefix]
        static bool PrefixMethod(Room room)
        {
            var things = room.ContainedAndAdjacentThings;
            var count = things.Count;
            foreach (Thing thing in things)
            {
                if (thing != null)
                {
                    if (thing.def.defName.Contains("Tiberium") || count <= 9)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(AutoBuildRoofAreaSetter))]
    [HarmonyPatch("TryGenerateAreaOnImpassable")]
    [HarmonyPatch(new Type[] { typeof(IntVec3) })]
    class AutoBuildroofAreaPatch2
    {
        [HarmonyPrefix]
        static bool PrefixMethod(AutoBuildRoofAreaSetter __instance, IntVec3 c)
        {
            var map = Traverse.Create(__instance).Property("Map").GetValue<Map>();

            if (!c.Roofed(map) && c.Impassable(map) && RoofCollapseUtility.WithinRangeOfRoofHolder(c, map))
            {
                Building b = c.GetFirstBuilding(map);
                Plant p = c.GetPlant(map);
                if (b != null | p != null)
                {
                    if (b.def.defName.Contains("TBNS") | p.def.defName.Contains("Tiberium"))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }


    /*
    [StaticConstructorOnStartup]
    [HarmonyPatch]
    class PawnWound
    {
        private List<Vector2> locsPerSide = new List<Vector2>();

        private Material mat;

        private Quaternion quat;

        private static readonly Vector2 WoundSpan = new Vector2(0.18f, 0.3f);


        static MethodBase TargetMethod()
        {
            return AccessTools.Constructor(AccessTools.Inner(typeof(PawnWoundDrawer), "Wound"), new Type[] { typeof(Pawn) });
        }

        static void Prefix(object __instance, Pawn pawn)
        {

            if (pawn.RaceProps.FleshType != FleshTypeDefOf.Normal)
            {
                return;
            }
        }

    } */
}


               




