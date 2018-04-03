using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace TiberiumRim
{
    public class Building_TNC : Building
    {
        public List<Building_Connector> Connectors = new List<Building_Connector>();

        public List<CellRect> Positions = new List<CellRect>();

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look<Building_Connector>(ref Connectors, "Connectors", LookMode.Reference);
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            foreach(Building_Connector c in Connectors)
            {
                c.Destroy();
            }
            base.Destroy(mode);
        }

        public List<Comp_TNW> AllStorages
        {
            get
            {
                List<Comp_TNW> compList = new List<Comp_TNW>();
                foreach (Building_Connector b in Connectors)
                {
                    if (b != null)
                    {
                        Comp_TNW comp = b.Parent.GetComp<Comp_TNW>();
                        if (comp != null)
                        {
                            if (comp.props.isGlobalStorage || comp.props.isLocalStorage)
                            { compList.Add(comp); }
                        }
                    }
                }
                return compList;
            }
        }

        public List<Comp_TNW> AllGlobalStorages
        {
            get
            {
                List<Comp_TNW> compList = new List<Comp_TNW>();
                foreach(Building_Connector b in Connectors)
                {
                    if (b != null)
                    {
                        Comp_TNW comp = b.Parent.GetComp<Comp_TNW>();
                        if (comp != null)
                        {
                            if (comp.props.isGlobalStorage)
                            { compList.Add(comp); }
                        }
                    }
                }
                return compList;
            }
        }

        public float TotalStoredTiberium
        {
            get
            {
                float storage = 0f;            
                for(int i = 0; i < Connectors.Count; i++) 
                {
                    Building_Connector b = Connectors[i];
                    if (b != null)
                    {
                        Comp_TNW comp = b.Parent.GetComp<Comp_TNW>();
                        if (comp != null)
                        {
                            if (comp.Container.GetTotalStorage > 0)
                            {
                                storage += comp.Container.GetTotalStorage;
                            }
                        }
                    }
                }
                return storage;
            }
        }
        

        public override void Tick()
        {
            base.Tick();
            if (Positions.Count > 0)
            {
                for (int i = 0; i < Positions.Count; i++)
                {
                    CellRect cells = Positions[i];
                    Building building = cells.GetBuilding(DefDatabase<ThingDef>.GetNamed("Connector_TNW"), this.Map);
                    if (building != null)
                    {
                        Building_Connector connector = building as Building_Connector;
                        this.Connectors.Add(connector);
                        connector.Network = this;
                        this.Positions.Remove(cells);
                    }
                }
            }      
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach(Gizmo g in base.GetGizmos())
            {
                yield return g;
            }

            Designator_BuildWithParent builder = new Designator_BuildWithParent(DefDatabase<ThingDef>.GetNamed("Connector_TNW"), this)
            {
            };
            yield return builder;
        }

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(base.GetInspectString());
            stringBuilder.AppendLine("TotalStoredTib".Translate()+": " + TotalStoredTiberium);
            return stringBuilder.ToString().TrimEndNewlines();
        }
    }
}
