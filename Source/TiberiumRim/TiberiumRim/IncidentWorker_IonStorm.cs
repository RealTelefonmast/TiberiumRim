﻿using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace TiberiumRim
{
    public class IncidentWorker_IonStorm : IncidentWorker_MakeGameCondition
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            int count = map.GetComponent<MapComponent_TiberiumHandler>().AllTiberiumCrystals.Count;
            bool flag = count > 350;
            bool result;
            if (flag)
            {
                int duration = Mathf.RoundToInt(this.def.durationDays.RandomInRange * 60000f);
                GameCondition cond = GameConditionMaker.MakeCondition(this.def.gameCondition, duration, 1);
                GameCondition cond2 = GameConditionMaker.MakeCondition(GameConditionDefOf.SolarFlare, duration, 1);
                map.gameConditionManager.RegisterCondition(cond);
                map.gameConditionManager.RegisterCondition(cond2);
                map.weatherManager.TransitionTo(WeatherDef.Named("IonStormWeather"));
                base.SendStandardLetter();
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }
    }
}