using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace TiberiumRim
{
    public class TibBill : Bill_Production, IExposable
    {
        public Pawn billDoer;

        public TibBill(RecipeDef_Tiberium def) : base(def as RecipeDef)
        {           
        }

        public override void Notify_DoBillStarted(Pawn billDoer)
        {
            this.billDoer = billDoer;
            base.Notify_DoBillStarted(billDoer);
        }

        private float RecipeCost
        {
            get
            {
                return (this.recipe as RecipeDef_Tiberium).tiberiumCost;
            }
        }

        public override bool ShouldDoNow()
        {
            Building_TiberiumCrafter b = this.billStack.billGiver as Building_TiberiumCrafter;
            Comp_TNW comp = b.GetComp<Comp_TNW>();
            if (comp != null)
            {
                Building_TNC network = comp.Connector?.Network;
                if (network != null)
                {
                    if (network.TotalStoredTiberium >= RecipeCost)
                    {
                        if (base.ShouldDoNow())
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public override void Notify_IterationCompleted(Pawn billDoer, List<Thing> ingredients)
        {
            Building_TiberiumCrafter crafter = null;
            foreach(IntVec3 c in billDoer.CellsAdjacent8WayAndInside())
            {
                Thing thing = c.GetFirstBuilding(billDoer.Map);
                if (thing != null)
                {
                    if(thing is Building_TiberiumCrafter)
                    {
                        crafter = thing as Building_TiberiumCrafter;
                    }
                }
            }
            base.Notify_IterationCompleted(billDoer, ingredients);
            foreach(Comp_TNW comp in crafter.TryGetComp<Comp_TNW>()?.Connector?.Network?.AllStorages)
            {
                if(comp.Container.GetTotalStorage >= RecipeCost)
                {
                    comp.Container.RemoveCrystal(comp.Container.MainType, RecipeCost);
                }
            }
        }
    }
}
