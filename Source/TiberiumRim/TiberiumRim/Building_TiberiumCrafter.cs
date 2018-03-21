using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace TiberiumRim
{
    public class Building_TiberiumCrafter : Building_GraphicSwitchable
    {
        public Building_TiberiumCrafter() : base()
        {
            this.billStack = new BillStack(this);
        }

        public override bool IsFilled => false;

        public override bool IsActivated => false;
    }
}
