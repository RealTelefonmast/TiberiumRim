using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace TiberiumRim
{
    public class Comp_TNW : ThingComp
    {
        public Building_Connector Connector;

        public new CompProperties_TNW props;

        private float storedTiberium = 0;

        private int ticksToProduce;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this.props = base.props as CompProperties_TNW;
            if (!respawningAfterLoad)
            {
                ResetTimer();
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<int>(ref ticksToProduce, "ticksToProduce");
            Scribe_References.Look<Building_Connector>(ref Connector, "Connector");
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            this.Connector.Destroy(DestroyMode.Deconstruct);
            base.PostDestroy(mode, previousMap);
        }

        public override void CompTick()
        {
            base.CompTick();

            if (!NeedsPower)
            {
                if (PowerComp != null)
                {
                    if (!PowerComp.PowerOn)
                    {
                        return;
                    }
                }
            }
            if (props.isAutoProducer)
            {
                StringBuilder sb = new StringBuilder();
                if (ticksToProduce <= 0)
                {
                    Produce(this.props.produceAmt);
                    ResetTimer();
                }
                else { ticksToProduce -= 1; }
            }

            if (this.Connector != null)
            {
                if (NetworkIsPowered)
                    if (props.isLocalStorage && this.CurrentlyStoredTiberium > 0)
                    {
                        List<Comp_TNW> compList = Connector.Network.AllGlobalStorages.FindAll((Comp_TNW x) => !x.CapacityFull);
                        for (int i = 0; i < compList.Count; i++)
                        {
                            Comp_TNW comp = compList[i];
                            float flt = (storedTiberium - props.thresholdAmt > 0 ? props.thresholdAmt : props.thresholdAmt + (storedTiberium - props.thresholdAmt)) / compList.Count;
                            comp.storedTiberium += flt;
                            this.storedTiberium -= flt;
                        }
                    }
            }
        }
        

        public void ResetTimer()
        {
            this.ticksToProduce = GenTicks.SecondsToTicks(this.props.produceTime);
        }

        public void Receive(float value)
        {
            if (!CapacityFull)
            {
                this.storedTiberium += value;
            }
        }

        public void Take(float value)
        {
            if(this.storedTiberium > value)
            {
                this.storedTiberium -= value;
            }
        }

        public void Produce(float value)
        {
            if (!CapacityFull)
            {
                this.storedTiberium += value;
            }
        }

        public float CurrentlyStoredTiberium
        {
            get
            {
                if (storedTiberium >= props.maxStorage)
                {
                    storedTiberium = props.maxStorage;
                }
                else if(storedTiberium < 0)
                {
                    storedTiberium = 0;                  
                }
                return storedTiberium;
            }
        }

        public bool ConnectedToNetwork
        {
            get
            {
                return Connector?.Network != null;
            }
        }

        public bool CapacityFull
        {
            get
            {
                return this.CurrentlyStoredTiberium == props.maxStorage;
            }
        }

        public bool NetworkIsPowered => (bool)this.Connector?.Network?.TryGetComp<CompPowerTrader>().PowerOn;

        public bool NeedsPower => this.PowerComp != null;


        public CompPowerTrader PowerComp
        {
            get
            {
                return this.parent.GetComp<CompPowerTrader>();               
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            return base.CompGetGizmosExtra();
        }

        public override void PostDraw()
        {
            base.PostDraw();
            if (Find.Selector.IsSelected(this.parent))
            {
                if (Connector != null)
                {
                    List<IntVec3> pos = new List<IntVec3>
                    {
                        Connector.Position
                    };
                    GenDraw.DrawFieldEdges(pos);
                }
            }
        }

        public override string CompInspectStringExtra()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (!ConnectedToNetwork)
            {
                stringBuilder.AppendLine("MissingNetwork".Translate());
            }
            stringBuilder.AppendLine("StoredTiberiumSilo".Translate() + ": "+this.CurrentlyStoredTiberium);
            return stringBuilder.ToString().TrimEndNewlines();
        }
    }

    public class CompProperties_TNW : CompProperties
    {
        public float maxStorage = 0f;

        public bool isGlobalStorage = false;

        public bool isLocalStorage = false;

        public bool isAutoProducer = false;

        public float thresholdAmt = 10f;

        public float produceAmt = 0f;

        public int produceTime = 6;

        public CompProperties_TNW()
        {
            this.compClass = typeof(Comp_TNW);
        }
    }
}
