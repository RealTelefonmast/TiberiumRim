using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Verse;
using RimWorld;

namespace TiberiumRim
{
    /*
     * MakeMapCondition and MapConditions don't exist anymore. Requires reevaluation. This also seems needlessly complex for deciding *if* we will fire the event, and dependent on rares.
     * Conclusion: Reanalyze purpose of incident, balance dev cost and running cost of decisions against gameplay gains.
     */
    class IncidentWorker_IonStorm : IncidentWorker_MakeGameCondition
    {

        public override bool TryExecute(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            int count = map.listerThings.AllThings.FindAll((Thing x) => x.def.defName.Contains("Tiberium")).Count;
            
            if (count > 350)
            {
                int duration = Mathf.RoundToInt(this.def.durationDays.RandomInRange * 60000f);
                GameCondition cond = GameConditionMaker.MakeCondition(this.def.gameCondition, duration, 1);
                GameCondition cond2 = GameConditionMaker.MakeCondition(GameConditionDefOf.SolarFlare, duration, 1);
                map.gameConditionManager.RegisterCondition(cond);
                map.gameConditionManager.RegisterCondition(cond2);
                map.weatherManager.TransitionTo(WeatherDef.Named("IonStormWeather"));
                base.SendStandardLetter();
                return true;
            }
            return false;
        }
    }
}
