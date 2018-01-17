using Harmony;
using RimWorld;
using System.Reflection;
using Verse;
using UnityEngine;
using TiberiumRim;

namespace TiberiumRimFactions
{
    public static class TiberiumRimFactionsSettings
    {
        public static FactionsSettings settings;
    }

    public class TiberiumRimFactionsMod : Mod
    {
        FactionsSettings settings;

        public TiberiumRimFactionsMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<FactionsSettings>();
            TiberiumRimFactionsSettings.settings = this.settings;
        }

        public override string SettingsCategory() => "TiberiumRimFactions";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            float width = 230f;
            Vector2 vec = new Vector2(0f, -32f);

            Rect rowRect1 = new Rect(inRect);
            rowRect1.height = 32f;
            string BothFactions = "BothFactionsOption_Label".Translate();
            rowRect1.width = width;
            Widgets.CheckboxLabeled(rowRect1, BothFactions, ref this.settings.BothFactions, false);
            if (this.settings.BothFactions)
            {
                Rect textRect = new Rect(rowRect1);
                textRect.xMin = rowRect1.xMax;
                textRect.xMax = textRect.xMin + 800f;
                Widgets.Label(textRect, "BothFactionsOption_Desc".Translate());
            }
        }     
    }

    public class FactionsSettings : ModSettings
    {
        public bool BothFactions = false;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref this.BothFactions, "BothFactions", false, true);
        }
    }

    [StaticConstructorOnStartup]
    class TiberiumRimFactions
    {
        static TiberiumRimFactions()
        {
            HarmonyInstance TiberiumRimFactions = HarmonyInstance.Create("com.tiberiumrimfactions.rimworld.mod");
            TiberiumRimFactions.PatchAll(Assembly.GetExecutingAssembly());
        }


        [HarmonyPatch(typeof(ResearchProjectDef))]
        [HarmonyPatch("CanStartNow", PropertyMethod.Getter)]
        static class ResearchPatch
        {
            [HarmonyPrefix]
            static bool PrefixMethod(ResearchProjectDef __instance)
            {
                if (TiberiumRimFactionsSettings.settings.BothFactions)
                {
                    return true;
                }

                int count = 0;

                if (__instance.defName.Contains("GDI"))
                {
                    if (DefDatabase<ResearchProjectDef>.GetNamed("NodResearch").IsFinished || DefDatabase<ResearchProjectDef>.GetNamed("NodResearch").ProgressPercent > 0)
                    {
                        if (count < 1)
                        {
                            Faction faction1 = FactionUtility.DefaultFactionFrom(DefDatabase<FactionDef>.GetNamed("GDI"));
                            Faction faction2 = FactionUtility.DefaultFactionFrom(DefDatabase<FactionDef>.GetNamed("Nod"));
                            faction1.AffectGoodwillWith(Faction.OfPlayer, -200);
                            faction2.AffectGoodwillWith(Faction.OfPlayer, 200);
                            count = 1;
                            return false;
                        }
                    }
                }
                else if (__instance.defName.Contains("Nod"))
                {
                    if (DefDatabase<ResearchProjectDef>.GetNamed("GDIResearch").IsFinished || DefDatabase<ResearchProjectDef>.GetNamed("GDIResearch").ProgressPercent > 0)
                    {
                        if (count < 1)
                        {
                            Faction faction1 = FactionUtility.DefaultFactionFrom(DefDatabase<FactionDef>.GetNamed("Nod"));
                            Faction faction2 = FactionUtility.DefaultFactionFrom(DefDatabase<FactionDef>.GetNamed("GDI"));
                            faction1.AffectGoodwillWith(Faction.OfPlayer, -200);
                            faction2.AffectGoodwillWith(Faction.OfPlayer, 200);
                            count = 1;
                            return false;
                        }
                    }
                }
                return true;
            }
        }
    }
}
