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

        public override float FilledPct
        {
            get
            {
                return ((this.billStack.Bills.Find((Bill x) => (x as TibBill).billDoer != null) != null) ? 1f : 0);
            }
        }

        public override bool IsActivated => false;
    }
}
