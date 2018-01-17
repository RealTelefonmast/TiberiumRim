using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TiberiumRimFactions
{
    [StaticConstructorOnStartup]
    public class Building_FirestormTexture : Building
    {
        public ThingDef_FirestormTextures def2 = null;

        public Graphic PrimaryGraphic;

        public Graphic SecondaryGraphic;

        public string SecondaryGraphicPath;

        private Graphic graphicOld;

        private bool updateGraphicForceNeeded = false;

        public CompPowerTrader powerComp;

        public override Graphic Graphic
        {
            get
            {
                bool flag = this.PrimaryGraphic == null;
                Graphic result;
                if (flag)
                {
                    this.GetGraphics();
                    bool flag2 = this.PrimaryGraphic == null;
                    if (flag2)
                    {
                        result = base.Graphic;
                        return result;
                    }
                }
                bool flag3 = ThingComp_FirestormCenter.activatedFS && this.powerComp.PowerOn;
                if (!flag3)
                {
                    result = this.PrimaryGraphic;
                }
                else
                {
                    bool flag4 = this.SecondaryGraphic == null;
                    if (flag4)
                    {
                        this.GetGraphics();
                        bool flag5 = this.SecondaryGraphic == null;
                        if (flag5)
                        {
                            result = this.PrimaryGraphic;
                        }
                        else
                        {
                            result = this.SecondaryGraphic;
                        }
                    }
                    else
                    {
                        result = this.SecondaryGraphic;
                    }
                }
                return result;
            }
        }

        public void GetGraphics()
        {
            bool flag = this.def2 == null;
            if (flag)
            {
                this.ReadXMLData();
            }
            bool flag2 = this.PrimaryGraphic == null || this.PrimaryGraphic == BaseContent.BadGraphic;
            if (flag2)
            {
                this.PrimaryGraphic = base.Graphic;
            }
            bool flag3 = this.SecondaryGraphic == null || this.SecondaryGraphic == BaseContent.BadGraphic;
            if (flag3)
            {
                this.SecondaryGraphic = GraphicDatabase.Get<Graphic_Single>(this.SecondaryGraphicPath, this.def2.graphic.Shader, this.def2.graphic.drawSize, this.def2.graphic.Color, this.def2.graphic.ColorTwo);
            }
        }

        private void ReadXMLData()
        {
            this.def2 = (this.def as ThingDef_FirestormTextures);
            bool flag = this.def2 == null;
            if (!flag)
            {
                this.SecondaryGraphicPath = this.def2.secondaryGraphicPath;
            }
        }

        public override void PostMake()
        {
            base.PostMake();
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            this.ReadXMLData();
            base.SpawnSetup(map, respawningAfterLoad);
            this.powerComp = base.GetComp<CompPowerTrader>();
            LongEventHandler.ExecuteWhenFinished(new Action(this.Setup_Part2));
        }

        public void Setup_Part2()
        {
            this.GetGraphics();
        }

        private void UpdateGraphic()
        {
            bool flag = this.Graphic != this.graphicOld || this.updateGraphicForceNeeded;
            if (flag)
            {
                this.updateGraphicForceNeeded = false;
                this.graphicOld = this.Graphic;
                this.Notify_ColorChanged();
                base.Map.mapDrawer.MapMeshDirty(base.Position, MapMeshFlag.Things, true, false);
            }
        }

        public override void Tick()
        {
            base.Tick();
            this.UpdateGraphic();
        }
    }
}
