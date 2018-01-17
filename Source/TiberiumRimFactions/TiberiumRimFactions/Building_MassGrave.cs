using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace TiberiumRimFactions
{
    public class Building_MassGrave : Building_Grave
    {

        private GraveDef Def
        {
            get
            {
                return (GraveDef)this.def;
            }
        }

        private int Corpses
        {
            get
            {
                return innerContainer.Count;
            }
        }

        private bool CanAcceptCorpses
        {
            get
            {
                return Corpses < Def.CorpseCapacity;
            }
        }

        public override bool Accepts(Thing thing)
        {
            return innerContainer.CanAcceptAnyOf(thing) && CanAcceptCorpses && GetStoreSettings().AllowedToAccept(thing);
        }

        public override string GetInspectString()
        {
            string text =
                "GraveCapacity".Translate().CapitalizeFirst() + ": " + Corpses.ToString() + "/" + Def.CorpseCapacity.ToString() + "\n" +
                "AssignedColonist".Translate().CapitalizeFirst() + ": ";
            for (int i = 0; i < innerContainer.Count; i++)
                text += (i % 2 == 0 ? "\n" : "") + innerContainer[i].LabelCap + (i % 2 == 0 ? ", " : "");
            return text;
        }


        public override IEnumerable<Gizmo> GetGizmos()
        {
            var gizmos = base.GetGizmos();
            foreach (Gizmo giz in gizmos)
            {
                if ((giz as Command_Action)?.defaultLabel != "GraveAssignColonist".Translate())
                {
                    yield return giz;
                }
            }
        }
    }

    public class GraveDef : ThingDef
    {
        public int CorpseCapacity;
    }
}
