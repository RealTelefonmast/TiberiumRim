using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse;
using RimWorld;

namespace TiberiumRimFactions
{
    public class ThingComp_FirestormCenter : ThingComp
    {
        private CompPowerTrader powerComp;
        private CompProperties_FirestormCenter def;
        public static List<IntVec3> fsWalls = new List<IntVec3>();
        public static bool activatedFS = false;
        private List<IntVec3> thisCells = new List<IntVec3>();
        private int FirestormTicks;
        private int TicksUntilShutdown;
        private int warmUpTicks;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            this.powerComp = this.parent.TryGetComp<CompPowerTrader>();
            this.def = (CompProperties_FirestormCenter)this.props;

            activatedFS = false;
            base.PostSpawnSetup(respawningAfterLoad);
        }

        public override void PostDeSpawn(Map map)
        {
            removeCells();
            thisCells.Clear();
            fsWalls.Clear();
            base.PostDeSpawn(map);
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look<bool>(ref activatedFS, "activatedFS", false, false);
            Scribe_Values.Look<int>(ref this.FirestormTicks, "FirestormTicks", 0, false);
            Scribe_Values.Look<int>(ref this.TicksUntilShutdown, "TicksUntilShutdown", 0, false);
            Scribe_Values.Look<int>(ref this.warmUpTicks, "warmUpTicks", 0, false);
            base.PostExposeData();
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            activatedFS = false;
            removeCells();
            thisCells.Clear();
            base.PostDestroy(mode, previousMap);
        }

        public override void CompTick()
        {
            if (!this.powerComp.PowerOn)
            {
                if (thisCells.Count > 0)
                {
                    removeCells();
                    thisCells.Clear();
                    return;
                }
                activatedFS = false;
                resetTicks();
                return;
            }
            else
            {
                if (activatedFS)
                {
                    if (fsWalls.Count > 0)
                    {
                        FirestormTicks += 1;
                        if (Rand.Chance(0.01f))
                        {
                            int i = Rand.Range(1, fsWalls.Count);
                            this.parent.Map.weatherManager.eventHandler.AddEvent(new WeatherEvent_LightningStrike(this.parent.Map, fsWalls[i]));
                        }
                    }
                    if (FirestormTicks / TicksUntilShutdown >= 1)
                    {
                        activatedFS = !activatedFS;
                        FirestormTicks = 0;
                        warmUpTicks = 0;
                        return;
                    }
                }
                if (!firestormWarmedUp)
                {
                    warmUpTicks += 1;
                }
            }
        }

        public void removeCells()
        {
            foreach (IntVec3 v in thisCells)
            {
                fsWalls.Remove(v);
            }
        }

        public void resetTicks()
        {
            FirestormTicks = 0;
            TicksUntilShutdown = 0;
            warmUpTicks = 0;
        }

        public void getWalls2()
        {
            List<Thing> thingList = this.parent.Map.listerThings.AllThings.FindAll((Thing x) => x.def.defName.Contains("FSW"));
            if (thingList != null)
                if (thingList.Count == 0)
                {
                    Messages.Message("NoFirestormWalls".Translate(), MessageTypeDefOf.RejectInput);
                    return;
                }
            foreach (Thing t in thingList)
            {
                if (t != null)
                {
                    if (t.TryGetComp<CompPowerTrader>().PowerOn)
                    {
                        fsWalls.Add(t.Position);
                        thisCells.Add(t.Position);
                    }
                    if (t.Position.InBounds(this.parent.Map))
                    {
                        if (t.Faction != Faction.OfPlayer)
                        {
                            t.SetFaction(Faction.OfPlayer);
                        }
                    }
                }
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo g in base.CompGetGizmosExtra())
            {
                yield return g;
            }

            if (true)
            {
                yield return new Command_Action
                {
                    defaultLabel = "FirestormStart".Translate(),
                    defaultDesc = (activatedFS ? "FirestormActive".Translate() : "FirestormInactive".Translate()),
                    icon = (activatedFS ? ContentFinder<Texture2D>.Get("ui/icons/Firestorm_On", true) : ContentFinder<Texture2D>.Get("ui/icons/Firestorm_Off", true)),
                    activateSound = SoundDefOf.Click,

                    action = delegate
                    {
                        if (firestormWarmedUp && activatedFS == false)
                        {
                            if (fsWalls.Count > 0)
                            {
                                float powerCap = this.powerComp.PowerNet.CurrentStoredEnergy();

                                if (powerCap <= 1500)
                                {
                                    Messages.Message("FirestormNoPower".Translate(), MessageTypeDefOf.NeutralEvent);
                                    return;
                                }
                                float Percentage = powerCap / 7500;
                                float maxPower = (Percentage > 1 ? 7500 : powerCap);
                                TicksUntilShutdown = (int)(Percentage > 1 ? 60000 : 60000 * Percentage);
                                if (Percentage < 1)
                                {
                                    Messages.Message("FirestormPartial".Translate() + " " + TicksUntilShutdown.TicksToSecondsString(), MessageTypeDefOf.NeutralEvent);

                                }

                                List<CompPowerBattery> CPB = this.powerComp.PowerNet.batteryComps.FindAll((CompPowerBattery x) => x.StoredEnergy > 0);
                                foreach (CompPowerBattery c in CPB)
                                {
                                    float pct = (maxPower / CPB.Count) / c.StoredEnergy;
                                    if (pct < 1)
                                    {
                                        c.SetStoredEnergyPct(c.StoredEnergyPct - pct);
                                        maxPower -= maxPower / CPB.Count;
                                    }
                                    else
                                    {
                                        c.SetStoredEnergyPct(0f);
                                        maxPower -= c.StoredEnergy;
                                    }
                                }
                                activatedFS = !activatedFS;
                                this.parent.Map.weatherManager.TransitionTo(WeatherDef.Named("FoggyRain"));
                                return;
                            }
                            Messages.Message("NoFirestormWallsSelected".Translate(), MessageTypeDefOf.RejectInput);
                        }
                        Messages.Message(ready, MessageTypeDefOf.RejectInput);
                    }
                };
                yield return new Command_Action
                {
                    defaultLabel = "RecheckWallsLabel".Translate(),
                    defaultDesc = "RecheckWallsDesc".Translate(),
                    icon = ContentFinder<Texture2D>.Get("ui/icons/RecheckWalls", true),
                    activateSound = SoundDefOf.Click,
                    action = delegate
                    {
                        if (!activatedFS)
                        {
                            if (this.powerComp.PowerOn)
                            {
                                if (fsWalls != null)
                                {
                                    removeCells();
                                    thisCells.Clear();
                                }
                                getWalls2();
                            }
                        }
                    }
                };
                if (Prefs.DevMode)
                {
                    yield return new Command_Action
                    {
                        defaultLabel = "DEBUG: Ready Up",
                        action = delegate
                        {
                            this.warmUpTicks = 300000;
                        }
                    };
                }
            }
        }

        private bool firestormWarmedUp
        {
            get
            {
                return warmUpTicks / 300000 == 1;
            }
        }

        private String ready
        {
            get
            {
                if (!firestormWarmedUp)
                {
                    return "FirestormNotReady".Translate() + " " + (int)(300000 - warmUpTicks).TicksToSeconds();
                }
                else
                {
                    return "FirestormReady".Translate();
                }
            }
        }

        public override string CompInspectStringExtra()
        {
            base.CompInspectStringExtra();
            if (activatedFS)
            {
                return "FirestormActivated".Translate() + (int)(TicksUntilShutdown - FirestormTicks).TicksToSeconds();
            }
            return ready;
        }
    }

    public class CompProperties_FirestormCenter : CompProperties
    {
        public CompProperties_FirestormCenter()
        {
            this.compClass = typeof(ThingComp_FirestormCenter);
        }
    }

    public class PlaceWorker_FireStormWalls : PlaceWorker
    {
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot)
        {
            if (ThingComp_FirestormCenter.fsWalls != null)
            {
                GenDraw.DrawFieldEdges(ThingComp_FirestormCenter.fsWalls);
            }
        }
    }
}
