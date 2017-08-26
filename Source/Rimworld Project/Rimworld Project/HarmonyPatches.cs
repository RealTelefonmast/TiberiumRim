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
                    if (thingList[i] != null)
                    {
                        if (thingList[i] is Plant)
                        {
                            if (thingList[i].def.defName.Contains("Tiberium"))
                            {
                                return false;
                            }
                            return true;
                        }
                    }
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
            foreach(Thing t in room.ContainedAndAdjacentThings)
            {
                if(t != null)
                {
                    if (t.def.defName.Contains("Tiberium"))
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
                for (int i = 0; i < 9; i++)
                {
                    IntVec3 loc = c + GenRadial.RadialPattern[i];
                    if (loc != null)
                    {
                        Room room = loc.GetRoom(map, RegionType.Set_Passable);
                        if (room != null && !room.TouchesMapEdge)
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
                        }
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


               




