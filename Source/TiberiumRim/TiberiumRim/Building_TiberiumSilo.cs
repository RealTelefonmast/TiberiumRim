using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace TiberiumRim
{

    public class Building_TiberiumSilo : Building_GraphicSwitchable
    {
        public bool activatefilling = false;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (!respawningAfterLoad)
            {
            }
        }

        public override bool IsFilled
        {
            get
            {
                if (activatefilling)
                {
                    return true;
                }
                return false;
            }
        }

        public override bool IsActivated
        {
            get
            {
                return false;
            }
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {      
            foreach(Gizmo g in base.GetGizmos())
            {
                yield return g;
            }

            if (Prefs.DevMode)
            {
                yield return new Command_Action
                {
                    defaultLabel = "DEBUG: Change Graphic",
                    icon = TexCommand.DesirePower,
                    action = delegate
                    {
                        this.activatefilling = !activatefilling;
                        base.Notify_GraphicsUpdated();
                    }
                };
            }
        }
    }
}
