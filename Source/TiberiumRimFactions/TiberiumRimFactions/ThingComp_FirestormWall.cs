using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse;
using RimWorld;

namespace TiberiumRimFactions
{
    public class ThingComp_FirestormWall : ThingComp
    {
        private CompPowerTrader powerComp;
        private CompProperties_FirestormWall def;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            this.powerComp = this.parent.TryGetComp<CompPowerTrader>();
            this.def = (CompProperties_FirestormWall)this.props;
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            ThingComp_FirestormCenter.fsWalls.Remove(this.parent.Position);
            base.PostDestroy(mode, previousMap);
        }

        private bool thisIsFWall
        {
            get
            {
                return this.parent.def.defName.Contains("GDI_FirestormDummy_FSW");
            }
        }

        private bool Firestorm
        {
            get
            {
                return ThingComp_FirestormCenter.activatedFS;
            }
        }

        public bool OnGrid
        {
            get
            {
                if (ThingComp_FirestormCenter.fsWalls != null)
                {
                    return ThingComp_FirestormCenter.fsWalls.Contains(this.parent.Position);
                }
                return false;
            }
        }

        public override void CompTick()
        {
            if (this.OnGrid)
            {
                if (Firestorm)
                {
                    if (thisIsFWall)
                    {
                        DoFirestorm();
                        return;
                    }
                    else
                    {
                        if (this.powerComp.PowerOn)
                        {
                            ThingDef thing = DefDatabase<ThingDef>.GetNamed("GDI_FirestormDummy_FSW");
                            GenSpawn.Spawn(thing, this.parent.Position, this.parent.Map);
                            this.parent.DeSpawn();
                            return;
                        }
                    }
                }
            }
            if (thisIsFWall)
            {
                ThingDef thing = DefDatabase<ThingDef>.GetNamed("GDI_FirestormWall_FSW");
                GenSpawn.Spawn(thing, this.parent.Position, this.parent.Map);
                this.parent.DeSpawn();
                return;
            }
        }
        private void DoFirestorm()
        {
            List<Thing> thinglist = this.parent.Position.GetThingList(this.parent.Map);
            for (int i = 0; i < thinglist.Count; i++)
            {
                if (thinglist[i].def.projectile != null)
                {
                    thinglist[i].Destroy();
                }
                else if (!thinglist[i].def.defName.Contains("FSW"))
                {
                    int ii = Rand.Range(2, 22);
                    DamageInfo d = new DamageInfo(DamageDefOf.Crush, ii);
                    if (thinglist[i].def.IsCorpse)
                    {
                        Corpse body = (Corpse)thinglist[i];
                        if (body.AnythingToStrip())
                        {
                            body.Strip();
                        }
                    }
                    thinglist[i].TakeDamage(d);
                }
            }
        }
    }

    public class CompProperties_FirestormWall : CompProperties
    {
        public CompProperties_FirestormWall()
        {
            this.compClass = typeof(ThingComp_FirestormWall);
        }
    }
}
