using Harmony;
using System;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using RimWorld.BaseGen;
using Verse;
using Verse.AI.Group;
using UnityEngine;

namespace TiberiumRim
{
    [StaticConstructorOnStartup]
    public static class TiberiumRimPatches
    {
        static TiberiumRimPatches()
        {
            HarmonyInstance TiberiumRim = HarmonyInstance.Create("com.tiberiumrim.rimworld.mod");

            //Mechanoid fixer from Jecrell
            TiberiumRim.Patch(
                typeof(SymbolResolver_RandomMechanoidGroup).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                    .First(mi => mi.HasAttribute<CompilerGeneratedAttribute>() && mi.ReturnType == typeof(bool) &&
                                 mi.GetParameters().Count() == 1 && 
                                 mi.GetParameters()[0].ParameterType == typeof(PawnKindDef)),
                null, new HarmonyMethod(typeof(TiberiumRimPatches),
                    nameof(MechanoidsFixerAncient)));

            TiberiumRim.Patch(
                typeof(CompSpawnerMechanoidsOnDamaged).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).First(
                    mi => mi.HasAttribute<CompilerGeneratedAttribute>() && mi.ReturnType == typeof(bool) &&
                          mi.GetParameters().Count() == 1 &&
                          mi.GetParameters()[0].ParameterType == typeof(PawnKindDef)), null, new HarmonyMethod(
                    typeof(TiberiumRimPatches), nameof(MechanoidsFixer)));

            TiberiumRim.PatchAll(Assembly.GetExecutingAssembly());
        }

        public static void MechanoidsFixerAncient(ref bool __result, PawnKindDef kind)
        {
            if (typeof(Mechanical_Pawn).IsAssignableFrom(kind.race.thingClass)) __result = false;
        }

        public static void MechanoidsFixer(ref bool __result, PawnKindDef def)
        {
            if (typeof(Mechanical_Pawn).IsAssignableFrom(def.race.thingClass)) __result = false;
        }

        [HarmonyPatch(typeof(BillUtility)), HarmonyPatch("MakeNewBill")]
        class BillPatch
        {
            [HarmonyPostfix]
            static void Fix(ref Bill __result)
            {
                if (__result.recipe is RecipeDef_Tiberium)
                {
                    TibBill tibBill = new TibBill(__result.recipe as RecipeDef_Tiberium);
                    __result = tibBill;
                }
            }
        }

        [HarmonyPatch(typeof(Designator_Build)), HarmonyPatch("DesignateSingleCell")]
        class BuildPatch
        {
            [HarmonyPostfix]
            static void Fix()
            {
            }
        }

        [HarmonyPatch(typeof(UI_BackgroundMain)), HarmonyPatch("BackgroundOnGUI"), StaticConstructorOnStartup]
        internal static class Custom_UI_BackgroundMain
        {
            private static readonly Texture2D Custom_Background = ContentFinder<Texture2D>.Get("UI/Icons/TiberiumBackground", true);

            internal static readonly Vector2 MainBackgroundSize = new Vector2(2048f, 1280f);

            private static bool Prefix()
            {
                if (TiberiumRimSettings.settings.UseCustomBackground)
                {
                    if (Custom_Background)
                    {
                        float width = (float)UI.screenWidth;
                        float num = (float)UI.screenWidth * (MainBackgroundSize.y / MainBackgroundSize.x);
                        GUI.DrawTexture(new Rect(0f, (float)UI.screenHeight / 2f - num / 2f, width, num), Custom_Background, ScaleMode.ScaleToFit, true);
                    }
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(MainMenuDrawer)), HarmonyPatch("MainMenuOnGUI"), StaticConstructorOnStartup]
        class FirstGame
        {
            [HarmonyPostfix]
            static void Fix()
            {
                if (TiberiumRimSettings.settings.FirstStartUp)
                {
                    TiberiumRimMod mod = LoadedModManager.ModHandles.Where((Mod x) => x is TiberiumRimMod).RandomElement() as TiberiumRimMod;
                    TiberiumSettings sets = TiberiumRimSettings.settings;
                    sets.SetBool(ref sets.FirstStartUp, false);
                    Find.WindowStack.Add(new Dialog_Difficulty(delegate
                    {
                        TiberiumRimSettings.settings.SetEasy();
                        mod.WriteSettings();
                    }, delegate
                    {
                        TiberiumRimSettings.settings.ResetToDefault();
                        mod.WriteSettings();
                    },
                    delegate {
                        TiberiumRimSettings.settings.SetHard();
                        mod.WriteSettings();
                    }));
                }               
            }
        }

        [HarmonyPatch(typeof(WorldInspectPane))]
        [HarmonyPatch("TileInspectString", PropertyMethod.Getter)]
        public class WorldTilePatch
        {
            [HarmonyPostfix]
            static void PostFix(WorldInspectPane __instance, ref String __result)
            {
                int SelectedTile = Traverse.Create(__instance).Property("SelectedTile").GetValue<int>();

                StringBuilder stringBuilder = new StringBuilder();
                if (Find.World.GetComponent<WorldComponent_TiberiumSpread>().TiberiumTiles.ContainsKey(SelectedTile))
                {
                    stringBuilder.Append("InfestationPct".Translate(new object[] {
                        Math.Round(Find.World.GetComponent<WorldComponent_TiberiumSpread>().TiberiumTiles[SelectedTile], 5) * 100 + "%"
                    }));
                }
                __result = __result + "\n\n" + stringBuilder.ToString();
            }
        }

        [HarmonyPatch(typeof(Pawn_PlayerSettings))]
        [HarmonyPatch("UsesConfigurableHostilityResponse", PropertyMethod.Getter)]
        public class HarvesterHostilityResponse
        {
            [HarmonyPostfix]
            static void PostFix(Pawn_PlayerSettings __instance, ref bool __result)
            {
                if (!__result)
                {
                    Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
                    if(pawn is Mechanical_Pawn && (pawn.Faction.def == FactionDefOf.PlayerColony))
                    {
                        __result = true;
                    }                  
                }
            }
        }

        //TODO: Better light thing
        [HarmonyPatch(typeof(CompGlower))]
        [HarmonyPatch("ShouldBeLitNow", PropertyMethod.Getter)]
        public class GlowerPatch
        {
            [HarmonyPostfix]
            static void PostFix(CompGlower __instance, ref bool __result)
            {
                Thing parent = __instance.parent;
                Comp_TNW compTNW = parent.TryGetComp<Comp_TNW>();

                if (compTNW != null)
                {
                    if (compTNW.Container != null)
                    {
                        __result = compTNW.Container.GetTotalStorage > 0;
                    }
                    __result = compTNW.IsGeneratingPower;
                }
            }
        }

        [HarmonyPatch(typeof(Designator_Build))]
        [HarmonyPatch("Visible", 0)]
        internal static class Harmony_Designator_Build_Patch
        {
            // Token: 0x0600000C RID: 12 RVA: 0x000021B4 File Offset: 0x000003B4
            public static void Postfix(Designator_Build __instance, ref bool __result)
            {
                TRThingDef thingDef;
                if ((thingDef = (__instance.PlacingDef as TRThingDef)) != null && !DebugSettings.godMode && thingDef.objectivePrerequisites != null)
                {
                    if (thingDef.objectivePrerequisites.Any((MissionObjectiveDef x) => x.IsFinished))
                    {
                        __result = true;
                        return;
                    }
                    __result = false;
                }
            }
        }
    }
}
