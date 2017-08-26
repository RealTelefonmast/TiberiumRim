using Harmony;
using HugsLib;
using HugsLib.Settings;
using HugsLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace TiberiumRim
{
    class TiberiumBase : ModBase
    {
        public static TiberiumBase Instance { get; private set; }

        public override string ModIdentifier
        {
            get
            {
                return "TiberiumRim";
            }
        }

        private TiberiumBase()
        {
            Instance = this;
        }

        public void logMessage(String message)
        {
            Logger.Message(message);
        }

        public void logError(String message)
        {
            Logger.Error(message);
        }

        public SettingHandle<bool> BuildingDamage;
        public SettingHandle<bool> EntityDamage;
        public SettingHandle<bool> UseProducerCap;

        internal SettingHandle<int> TiberiumProducersAmt
        {
            get;
            private set;
        }
        public SettingHandle<bool> UseSpreadRadius;

        public override void DefsLoaded()
        {
            BuildingDamage = Settings.GetHandle<bool>("BuildingDamage", "Tiberium_Damages_Structures".Translate(), "Tiberium_Damages_Structures_Desc".Translate(), true);
            EntityDamage = Settings.GetHandle<bool>("EntityDamage", "Tiberium_Damages_Items".Translate(), "Tiberium_Damages_Items_Desc".Translate(), true);
            UseProducerCap = Settings.GetHandle<bool>("UseProducerCap", "UseProducerCapLabel".Translate(), "UseProducerCapDesc".Translate(), false);
            UseSpreadRadius = Settings.GetHandle<bool>("UseSpreadRadius", "UseSpreadRadiusLabel".Translate(), "UseSpreadRadiusDesc".Translate(), false);
            TiberiumProducersAmt = Settings.GetHandle<int>("TiberiumProducersAmt", Translator.Translate("TiberiumProducersAmtLabel"), Translator.Translate("TiberiumProducersAmtDesc"), 6, Validators.IntRangeValidator(0, 100), null);
            TiberiumProducersAmt.SpinnerIncrement = 1;
        }
    }
}
