using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace TiberiumRim
{

    public class Building_TiberiumSilo : Building_GraphicSwitchable
    {
        public bool activatefilling = false;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (!respawningAfterLoad)
            {
            }
        }

        public Comp_TNW Network
        {
            get
            {
                return base.GetComp<Comp_TNW>();
            }
        }

        public override bool IsActivated => false;
    }
}
