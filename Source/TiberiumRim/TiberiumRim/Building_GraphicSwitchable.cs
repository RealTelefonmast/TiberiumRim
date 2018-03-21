using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace TiberiumRim
{
    public class GraphicSwitchableDef : ThingDef
    {
        public string filledGraphicPath = "";
        public string activeGraphicPath = "";
        public string overlayGraphicPath = "";
    }

    public abstract class Building_GraphicSwitchable : Building_WorkTable
    {
        public new GraphicSwitchableDef def;
        private CompPowerTrader powerComp;
        private GraphicData graphicData;

        [Unsaved]
        public Graphic BaseGraphic;
        [Unsaved]
        public Graphic ActivatedGraphic;
        [Unsaved]
        public Graphic FilledGraphic;
        [Unsaved]
        public Graphic OverlayGraphic;

        public override void SpawnSetup(Map map,bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            def = (GraphicSwitchableDef)base.def;
            powerComp = this.GetComp<CompPowerTrader>();
            graphicData = this.def.graphicData;
        }

        public bool GraphicsSet
        {
            get
            {
                bool flag = false;
                if (def.activeGraphicPath.Length > 0)
                {
                    flag = ActivatedGraphic != null;
                }
                if (def.filledGraphicPath.Length > 0)
                {
                    flag = FilledGraphic != null;
                }
                if (def.overlayGraphicPath.Length > 0)
                {
                    flag = OverlayGraphic != null;
                }
                return flag;
            }
        }

        public void SetGraphics()
        {
            SetBaseGraphic();
            SetActivatedGraphic();
            SetFilledGraphic();
            SetOverlayGraphic();
        }

        public Shader Shader
        {
            get
            {
                ShaderType sType = graphicData.shaderType;
                if (graphicData.shaderType == ShaderType.None)
                {
                    sType = ShaderType.Cutout;
                }
                return ShaderDatabase.ShaderFromType(sType);
            }
        }

        public void SetBaseGraphic()
        {
            BaseGraphic = GraphicDatabase.Get(graphicData.graphicClass, graphicData.texPath, Shader, graphicData.drawSize, graphicData.color, graphicData.colorTwo, graphicData);
        }

        public void SetActivatedGraphic()
        {
            if (def.activeGraphicPath.Length > 0)
            {
                ActivatedGraphic = GraphicDatabase.Get(graphicData.graphicClass, def.activeGraphicPath, Shader, graphicData.drawSize, graphicData.color, graphicData.colorTwo, graphicData);
            }
        }

        public void SetFilledGraphic()
        {
            if (def.filledGraphicPath.Length > 0)
            {
                FilledGraphic = GraphicDatabase.Get(graphicData.graphicClass, def.filledGraphicPath, Shader, graphicData.drawSize, graphicData.color, graphicData.colorTwo, graphicData);
            }
        }

        public void SetOverlayGraphic()
        {
            if (def.overlayGraphicPath.Length > 0)
            {
                OverlayGraphic = GraphicDatabase.Get(graphicData.graphicClass, def.overlayGraphicPath, Shader, graphicData.drawSize, graphicData.color, graphicData.colorTwo, graphicData);
            }
        }

        public abstract bool IsActivated {get;}

        public abstract bool IsFilled { get; }

        public override void Draw()
        {
            foreach(ThingComp comp in AllComps)
            {
                comp.PostDraw();
            }
            Graphic.Draw(this.DrawPos, this.Rotation, this, 0f);
        }

        public override void Print(SectionLayer layer)
        {
            return;
        }

        public void Notify_GraphicsUpdated()
        {
            this.Notify_ColorChanged();
            base.Map.mapDrawer.MapMeshDirty(base.Position, MapMeshFlag.Things, true, false);
        }

        public new Graphic Graphic
        {
            get
            {
                Graphic graphic;
                if (!GraphicsSet)
                {
                    SetGraphics();
                }
                graphic = BaseGraphic;
                if (IsActivated)
                {
                    if(powerComp != null)
                    {
                        if (powerComp.PowerOn)
                        {
                            graphic = ActivatedGraphic;
                        }
                    }
                }
                if (IsFilled)
                {
                    graphic = FilledGraphic;
                }
                return graphic;
            }
        }
    }
}
