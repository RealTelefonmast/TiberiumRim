using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace TiberiumRim
{
    public class CompGlower2 : CompGlower
    {
        public new CompProperties_Glower props;



        public CompGlower2() : base()
        {
            this.props = new CompProperties_Glower() { glowRadius = this.props.glowRadius, overlightRadius = this.props.overlightRadius, glowColor = this.props.glowColor };           
        }
    }
}
