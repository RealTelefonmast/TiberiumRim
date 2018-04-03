﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace TiberiumRim
{
    public class Building_TiberiumPowerPlant : Building_GraphicSwitchable
    {
        public Comp_TNW Network
        {
            get
            {
                return this.GetComp<Comp_TNW>();
            }
        }

        public override float FilledPct
        {
            get
            {
                return Network.Container.GetTotalStorage / Network.props.maxStorage;
            }
        }

        public override bool IsActivated => false;
    }
}