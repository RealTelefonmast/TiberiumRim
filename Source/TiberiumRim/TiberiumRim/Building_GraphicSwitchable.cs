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

        public Graphic BaseGraphic;
        public Graphic ActivatedGraphic;
        public Graphic FilledOverlay;
        public Graphic OverlayGraphic;

        public override void SpawnSetup(Map map,bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            def = (GraphicSwitchableDef)base.def;
            powerComp = this.GetComp<CompPowerTrader>();
        }

        public GraphicData GraphicData
        {
            get
            {
                return this.def.graphicData;
            }
        }

        public bool GraphicsSet
        {
            get
            {
                bool flag1 = false;
                bool flag2 = false;
                bool flag3 = false;
                if (def.activeGraphicPath.Length > 0)
                {
                    flag1 = ActivatedGraphic != null;
                }
                if (def.filledGraphicPath.Length > 0)
                {
                    flag2 = FilledOverlay != null;
                }
                if (def.overlayGraphicPath.Length > 0)
                {
                    flag3 = OverlayGraphic != null;
                }
                return flag1 && flag2 && flag3;
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
                ShaderType sType = GraphicData.shaderType;
                if (GraphicData.shaderType == ShaderType.None)
                {
                    sType = ShaderType.Cutout;
                }
                return ShaderDatabase.ShaderFromType(sType);
            }
        }

        public void SetBaseGraphic()
        {
            BaseGraphic = GraphicDatabase.Get(GraphicData.graphicClass, GraphicData.texPath, Shader, GraphicData.drawSize, GraphicData.color, GraphicData.colorTwo, GraphicData);
        }

        public void SetActivatedGraphic()
        {
            if (def.activeGraphicPath.Length > 0)
            {
                ActivatedGraphic = GraphicDatabase.Get(GraphicData.graphicClass, def.activeGraphicPath, Shader, GraphicData.drawSize, GraphicData.color, GraphicData.colorTwo, GraphicData);
            }
        }

        public void SetFilledGraphic()
        {
            if (def.filledGraphicPath.Length > 0)
            {
                FilledOverlay = GraphicDatabase.Get(GraphicData.graphicClass, def.filledGraphicPath, ShaderDatabase.MoteGlow, GraphicData.drawSize, Color, Color);
            }
        }

        public void SetOverlayGraphic()
        {
            if (def.overlayGraphicPath.Length > 0)
            {
                OverlayGraphic = GraphicDatabase.Get(GraphicData.graphicClass, def.overlayGraphicPath, Shader, GraphicData.drawSize, GraphicData.color, GraphicData.colorTwo, GraphicData);
            }
        }

        public abstract bool IsActivated {get;}

        public abstract float FilledPct { get; }

        public Color Color
        {
            get
            {
                TiberiumContainer container = this.TryGetComp<Comp_TNW>().Container;
                if (container != null)
                {
                    return container.Color;
                }
                return Color.white;
            }
        }

        public Material FadedMaterial
        {
            get
            {
                Material fade1 = FilledOverlay.MatAt(base.Rotation, null);
                return fade1;
            }
        }

        public override void Draw()
        {
            if (Graphic != null)
            {
                foreach (ThingComp comp in AllComps)
                {
                    comp.PostDraw();
                }
                if (FilledPct > 0f)
                {
                    Graphics.DrawMesh(FilledOverlay.MeshAt(base.Rotation), base.DrawPos + Altitudes.AltIncVect, Quaternion.identity, FadedMaterial, 0);
                }
                Graphic.Draw(this.DrawPos, this.Rotation, this);
            }
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
                return graphic;
            }
        }    
    }
}
