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

        public TiberiumContainer Container;

        private IntVec3 savedPos;

        private int ticksToDoWork;

        private int ticksToPower;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this.props = base.props as CompProperties_TNW;
            if (!respawningAfterLoad)
            {
                savedPos = new IntVec3(parent.Position.x, parent.Position.y, parent.Position.z);
                Container = new TiberiumContainer(props.maxStorage);
                ResetWorkTimer();
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<int>(ref ticksToDoWork, "ticksToProduce");
            Scribe_Values.Look<int>(ref ticksToPower, "ticksToPower");
            Scribe_Values.Look<IntVec3>(ref savedPos, "savedPos");
            Scribe_Deep.Look<TiberiumContainer>(ref Container, "TiberiumContainer");
            Scribe_References.Look<Building_Connector>(ref Connector, "Connector");
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            if (Connector != null && !Connector.Destroyed)
            {
                this.Connector.Destroy(DestroyMode.Deconstruct);
            }
            if (mode == DestroyMode.KillFinalize)
            {
                LeakTiberium(savedPos, previousMap);
            }
            base.PostDestroy(mode, previousMap);
        }

        public override void CompTick()
        {
            base.CompTick();

            if (NeedsPower)
            {
                if (!PowerComp.PowerOn)
                {
                    return;
                }              
            }
            if (CompFlickable != null)
            {
                if (!CompFlickable.SwitchIsOn)
                {
                    return;
                }
            }
            if (props.isAutoProducer)
            {
                if (ticksToDoWork <= 0)
                {
                    Container.AddCrystal((this.parent as Building_TiberiumSpike).geyser.tiberiumType, props.produceAmt, out float flt);
                    ResetWorkTimer();
                }
                else { ticksToDoWork -= 1; }
            }           
            if (props.isConsumer)
            {
                if (ticksToPower <= 0)
                {
                    float pct = 0f;

                    if (Container.GetTotalStorage > 0)
                    {
                        float value = props.consumeAmtPerDay;
                        float flt = -(Container.GetTotalStorage - props.consumeAmtPerDay);
                        if (flt > 0)
                        {
                            value = props.consumeAmtPerDay - flt;
                        }
                        pct = value / props.consumeAmtPerDay;
                        Container.RemoveCrystal(Container.MainType, value);
                    }
                    else { PowerComp.PowerOutput = 0f; }

                    ResetPowerTimer((int)(GenDate.TicksPerDay * pct));

                    CompGlower glower = (CompGlower)this.parent.AllComps.Find((ThingComp x) => x.props.compClass == typeof(CompGlower));
                    glower.ReceiveCompSignal("Refueled");
                }
                else { ticksToPower -= 1; }
            }
            if (this.Connector != null)
            {
                if (NetworkIsPowered)
                {
                    if (props.isLocalStorage)
                    {
                        if (props.isConsumer)
                        {
                            if (!Container.CapacityFull)
                            {
                                Comp_TNW comp = Connector.Network.AllStorages.Find((Comp_TNW x) => x.Container.GetTotalStorage >= 1 && !x.props.isConsumer);
                                if (comp != null)
                                {
                                    TiberiumType type = comp.Container.MainType;
                                    Container.AddCrystal(type, 1f, out float leftOver);
                                    comp.Container.RemoveCrystal(type, (1f - leftOver));
                                }
                            }
                            return;
                        }
                        if (Container.GetTotalStorage > 0)
                        {
                            List<Comp_TNW> compList = Connector.Network.AllGlobalStorages.FindAll((Comp_TNW x) => !x.Container.CapacityFull && !x.props.isLocalStorage);
                            for (int i = 0; i < compList.Count; i++)
                            {
                                Comp_TNW comp = compList[i];
                                TiberiumContainer container = comp.Container;
                                TiberiumType type = Container.MainType;
                                container.AddCrystal(type, 1f, out float leftOver);
                                Container.RemoveCrystal(type, (1f - leftOver));
                            }
                        }
                    }
                }
            }
        }       

        public void ResetWorkTimer()
        {
            this.ticksToDoWork = GenTicks.SecondsToTicks(this.props.workTime);
        }

        public void ResetPowerTimer(int ticks)
        {
            this.ticksToPower = ticks;            
        }

        public Map Map
        {
            get
            {
                return this.parent.Map;
            }
        }

        public bool IsGeneratingPower
        {
            get
            {
                return this.ticksToPower > 0;
            }
        }

        public bool ConnectedToNetwork
        {
            get
            {
                return Connector?.Network != null;
            }
        }

        public bool NetworkIsPowered => (bool)this.Connector?.Network?.TryGetComp<CompPowerTrader>()?.PowerOn;

        public bool NeedsPower => this.PowerComp != null;

        public CompPowerTrader PowerComp
        {
            get
            {
                return this.parent.TryGetComp<CompPowerTrader>();
            }
        }

        public CompFlickable CompFlickable
        {
            get
            {
                return this.parent.GetComp<CompFlickable>();
            }
        }

        public void LeakTiberium(IntVec3 parentPos, Map map)
        {
            List<TiberiumCrystal> tibList = new List<TiberiumCrystal>();
            foreach (TiberiumType type in Container.GetTypes)
            {
                TiberiumCrystalDef crystalDef = TiberiumUtility.CrystalDefFromType(type);
                float tibAmt = Container.ValueForType(type) / crystalDef.tiberium.maxHarvestValue;
                float scale = tibAmt / MainTCD.MainTiberiumControlDef.TiberiumLeakScale;
                for (int i = 0; i < scale; i++)
                {
                    TiberiumCrystal newCrystal = (TiberiumCrystal)ThingMaker.MakeThing(crystalDef);
                    newCrystal.growthInt = 1f;
                    tibList.Add(newCrystal);
                }
            }
            int num = tibList.Count;
            for (int i = 0; i < num; i++)
            {
                IntVec3 pos = parentPos + GenRadial.RadialPattern[i];
                if(pos.InBounds(map) && GenSight.LineOfSight(parentPos, pos, map) && pos.GetFirstBuilding(map) == null)
                {
                    if (Rand.Chance(0.5f))
                    {
                        GenSpawn.Spawn(DefDatabase<ThingDef>.GetNamed("FilthTibLiquid"), pos, map);
                    }
                    GenSpawn.Spawn(tibList[i], pos, map);
                }
            }         
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (Prefs.DevMode)
            {
                yield return new Command_Action
                {
                    defaultLabel = "Fill Green",
                    icon = TexCommand.DesirePower,
                    action = delegate
                    {
                        this.Container.AddCrystal(TiberiumType.Green, 500f, out float flt);
                    }
                };

                yield return new Command_Action
                {
                    defaultLabel = "Fill Blue",
                    icon = TexCommand.DesirePower,
                    action = delegate
                    {
                        this.Container.AddCrystal(TiberiumType.Blue, 500f, out float flt);
                    }
                };

                yield return new Command_Action
                {
                    defaultLabel = "Fill Red",
                    icon = TexCommand.DesirePower,
                    action = delegate
                    {
                        this.Container.AddCrystal(TiberiumType.Red, 500f, out float flt);
                    }
                };

                yield return new Command_Action
                {
                    defaultLabel = "+100 credits",
                    icon = TexCommand.DesirePower,
                    action = delegate
                    {
                        this.Container.AddCrystal(Rand.Element(TiberiumType.Green, TiberiumType.Blue, TiberiumType.Red), 100f, out float flt);
                    }
                };
            }
            yield break;
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
            if (props.isLocalStorage || props.isGlobalStorage)
            {
                stringBuilder.AppendLine("StoredTib".Translate() + ": " + /* Math.Round(this.Container.GetTotalStorage, 0) */ Container.GetTotalStorage + " | ");
                foreach(TiberiumType type in Container.GetTypes)
                {
                    stringBuilder.Append(type + " x " + (Container.ValueForType(type) / TiberiumUtility.CrystalDefFromType(type).tiberium.maxHarvestValue)+"|");                   
                }
                stringBuilder.AppendLine("" + Container.Color);
            }
            if (props.isConsumer)
            {
                stringBuilder.AppendLine("Consumes".Translate(new object[] {
                    props.consumeAmtPerDay
                }));
                if (this.ticksToPower > 0)
                {
                    stringBuilder.AppendLine("DaysRemaining".Translate(new object[]
                    {
                    Math.Round(GenDate.TicksToDays(this.ticksToPower), 2)
                    }));
                }
            }
            return stringBuilder.ToString().TrimEndNewlines();
        }
    }

    public class CompProperties_TNW : CompProperties
    {       
        public bool isGlobalStorage = false;

        public bool isLocalStorage = false;

        public bool isAutoProducer = false;

        public bool isConsumer = false;

        public int consumeAmtPerDay = 0;

        public float maxStorage = 0f;

        public float produceAmt = 0f;

        public int workTime = 6;

        public CompProperties_TNW()
        {
            this.compClass = typeof(Comp_TNW);
        }
    }
}
