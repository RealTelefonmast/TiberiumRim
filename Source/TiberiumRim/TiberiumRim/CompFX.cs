using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace TiberiumRim
{
    public class CompFX : ThingComp
    {
        public Graphic glowTexture;

        public CompFX_Properties Props
        {
            get
            {
                return (CompFX_Properties)base.props;
            }
        }

        public void SetTextures()
        {
            if(Props.glowTexture.Length > 0)
            {
                this.glowTexture = GraphicDatabase.Get(parent.def.graphicData.graphicClass, Props.glowTexture, ShaderDatabase.MoteGlow, parent.def.graphicData.drawSize, Color.white, Color.white);
            }
        }

        public override void PostDraw()
        {

        }
    }

    public class CompFX_Properties : CompProperties
    {
        public string glowTexture = "";

        public int secondsToPeak = 10;

        public CompFX_Properties()
        {
            this.compClass = typeof(CompFX);
        }
    }
}
