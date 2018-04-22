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

        public override void ExposeData()
        {
            base.ExposeData();
        }

        public override void Tick()
        {
            Comp_GraphicExtra extra = this.TryGetComp<Comp_GraphicExtra>();
            if(extra != null)
            {
                //TODO: Weird reee
                extra.FilledPct = (this.billStack.Bills.Find((Bill x) => (x as TibBill).isBeingDone) != null ? 1f : 0);
            }
        }

        public override bool IsActivated => false;
    }
}
