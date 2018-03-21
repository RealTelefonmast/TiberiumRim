using Harmony;
using RimWorld;
using System.Reflection;
using Verse;
using UnityEngine;
using RimWorld.Planet;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace TiberiumRim
{
    public static class TiberiumRimSettings
    {
        public static TiberiumSettings settings;
    }

    public class TiberiumRimMod : Mod
    {
        TiberiumSettings settings;

        public TiberiumRimMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<TiberiumSettings>();
            TiberiumRimSettings.settings = this.settings;
        }

        public override string SettingsCategory() => "TiberiumRim";

        public override void DoSettingsWindowContents(Rect inRect)
        {         
            Vector2 vec = new Vector2(0f, -32f);
            Rect newRect = new Rect(inRect);

            Rect boxRect = new Rect(inRect);
            boxRect.yMax = inRect.yMin + (6f*30f);
            boxRect.yMin = inRect.yMin + (4f*30f);
            boxRect.width = 500f;
            boxRect.center = boxRect.center - new Vector2(5f,0f);

            newRect.height = 32f;
            newRect.width = 230f;

            MakeNewCheckBox(newRect, "BuildingDamage".Translate(),ref this.settings.BuildingDamage, out newRect, 0f, 0f, false, "BuildingDamageDesc".Translate());
            MakeNewCheckBox(newRect, "EntityDamage".Translate(), ref this.settings.EntityDamage, out newRect, 30f, 0f, false, "EntityDamageDesc".Translate());
            MakeNewCheckBox(newRect, "PawnDamage".Translate(), ref this.settings.PawnDamage, out newRect, 30f, 0f, false, "PawnDamageDesc".Translate());
            MakeNewCheckBox(newRect, "UseSpecificProducersLabel".Translate(), ref this.settings.UseSpecificProducers, out newRect, 30f, 0f, false, "UseSpecificProducersDesc".Translate());

            if (this.settings.UseSpecificProducers)
            {
                Widgets.DrawBoxSolid(boxRect, new Color(0.12f, 0.15f, 0.18f));
                Rect tempRect = new Rect(newRect);          
                foreach (TiberiumProducerDef def in DefDatabase<TiberiumProducerDef>.AllDefsListForReading)
                {
                    if (!this.settings.ProducerBools.Keys.Any(sw => sw.value == def.defName))
                    {
                        this.settings.ProducerBools.Add(def.defName, true);
                    }
                }
                int setter = 1;
                for(int i = 0; i < this.settings.ProducerBools.Count; i++)
                {
                    StringWrapper defName = this.settings.ProducerBools.Keys.ToList()[i];
                    setter++;
                    if (setter % 2 == 0)
                    {
                        tempRect = newRect;
                        newRect.y = newRect.y + 32f;
                    }
                    bool temp = this.settings.ProducerBools[defName];
                    MakeNewCheckBox(tempRect, DefDatabase<TiberiumProducerDef>.GetNamed(defName).LabelCap, ref temp, out tempRect, setter % 2 == 0 ? 25f : 0f, setter % 2 == 0 ? 0f : tempRect.xMax + 25f);
                    this.settings.ProducerBools[defName] = temp;
                }
            }       
            MakeNewCheckBox(newRect, "UseProducerCapLabel".Translate(), ref this.settings.UseProducerCap, out newRect, 30f, 0f, false, "UseProducerCapDesc".Translate());
            if (this.settings.UseProducerCap)
            {
                Rect rowRect = new Rect(newRect);
                rowRect.center = newRect.center - vec;
                string buffer = this.settings.TiberiumProducersAmt.ToString();
                Widgets.TextFieldNumeric(rowRect, ref this.settings.TiberiumProducersAmt, ref buffer, 1f, 100f);
                newRect = rowRect;              
            }
            MakeNewCheckBox(newRect, "UseSpreadRadiusLabel".Translate(), ref this.settings.UseSpreadRadius, out newRect, 30f, 0f, false, "UseSpreadRadiusDesc".Translate());
            MakeNewCheckBox(newRect, "UseCustomBackgroundLabel".Translate(), ref this.settings.UseCustomBackground, out newRect, 30f, 0f, false, "UseCustomBackgroundDesc".Translate());
        }

        public void MakeNewCheckBox(Rect lastRect,  string label, ref bool checkOn, out Rect newRect, float yOffset = 0f, float xOffset = 0f, bool disabled = false, string desc = null)
        {
            //Label Part
            Rect rowRect = new Rect(lastRect);
            rowRect.center = lastRect.center + new Vector2(xOffset, yOffset);
            Widgets.CheckboxLabeled(rowRect, label, ref checkOn, disabled);

            // Description Part
            if (checkOn && desc != null)
            {
                Rect textRect = new Rect(rowRect);
                textRect.xMin = rowRect.xMax;
                textRect.xMax = rowRect.xMin + 800f;
                Widgets.Label(textRect, desc);
            }
            newRect = rowRect;
        }
    }

    public class TiberiumSettings : ModSettings
    {
        public bool BuildingDamage = true;
        public bool EntityDamage = true;
        public bool PawnDamage = true;
        public bool UseProducerCap = false;
        public bool UseSpecificProducers = false;

        public Dictionary<StringWrapper, bool> ProducerBools = new Dictionary<StringWrapper, bool>();

        public bool UseSpreadRadius = false;
        public bool UseCustomBackground = true;
        public int TiberiumProducersAmt = 7;

        public bool FirstStartUp = true;

        public void SetBeginner()
        {

        }
        public void SetMedium()
        {

        }
        public void SetRealExperience()
        {

        }

        public override void ExposeData()
        {          
            base.ExposeData();
            Scribe_Values.Look(ref this.BuildingDamage, "BuildingDamage", true, true);
            Scribe_Values.Look(ref this.EntityDamage, "EntityDamage", true, true);
            Scribe_Values.Look(ref this.PawnDamage, "PawnDamage", true, true);
            Scribe_Values.Look(ref this.UseProducerCap, "UseProducerCap", false, true);
            Scribe_Values.Look(ref this.UseSpecificProducers, "UseSpecificProducers", false, true);

            Scribe_Collections.Look<StringWrapper, bool>(ref this.ProducerBools, "ProducerBools", LookMode.Deep, LookMode.Value);

            Scribe_Values.Look(ref this.UseSpreadRadius, "UseSpreadRadius", false, true);
            Scribe_Values.Look(ref this.UseCustomBackground, "UseCustomBackground", true, true);
            Scribe_Values.Look(ref this.TiberiumProducersAmt, "TiberiumProducersAmt", 7, true);

            Scribe_Values.Look(ref this.FirstStartUp, "FirstStartUp", true, true);
        }
    }

    [StaticConstructorOnStartup]
    public static class TiberiumRim
    {
        static TiberiumRim()
        {
            HarmonyInstance TiberiumRim = HarmonyInstance.Create("com.tiberiumrim.rimworld.mod");
            TiberiumRim.PatchAll(Assembly.GetExecutingAssembly());
        }

        [HarmonyPatch(typeof(BillUtility)), HarmonyPatch("MakeNewBill")]
        class BillPatch
        {
            [HarmonyPostfix]
            static void Fix(ref Bill __result)
            {
                if(__result.recipe is RecipeDef_Tiberium)
                {
                    TibBill tibBill = new TibBill(__result.recipe as RecipeDef_Tiberium);
                    __result = tibBill;
                }
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
                    if (Custom_UI_BackgroundMain.Custom_Background)
                    {
                        float width = (float)UI.screenWidth;
                        float num = (float)UI.screenWidth * (Custom_UI_BackgroundMain.MainBackgroundSize.y / Custom_UI_BackgroundMain.MainBackgroundSize.x);
                        GUI.DrawTexture(new Rect(0f, (float)UI.screenHeight / 2f - num / 2f, width, num), Custom_UI_BackgroundMain.Custom_Background, ScaleMode.ScaleToFit, true);
                    }
                    return false;
                }
                return true;
            }
        }

        // TODO: Add worldcomp
        /* 
        [HarmonyPatch(typeof(WorldInspectPane))]
        [HarmonyPatch("TileInspectString", PropertyMethod.Getter)]
        class WorldTilePatch
        {
            [HarmonyPostfix]
            static void postFix(WorldInspectPane __instance, ref String __result)
            {
                int SelectedTile = Traverse.Create(__instance).Property("SelectedTile").GetValue<int>();
                Tile tile = Find.WorldGrid[SelectedTile];

                StringBuilder stringBuilder = new StringBuilder();
                if (Find.World.GetComponent<WorldComponent_TiberiumSpread>().tiberiumPcts.ContainsKey(SelectedTile))
                {
                    stringBuilder.Append("TiberiumTileInfestation".Translate() + Find.World.GetComponent<WorldComponent_TiberiumSpread>().tiberiumPcts[SelectedTile] * 100 + "%");
                }
                __result = __result + "\n\n" + stringBuilder.ToString();

            }
        }
        */
    }
}