using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;

namespace TiberiumRim
{
    [StaticConstructorOnStartup]
    public class MainTabWindow_MissionObjectives : MainTabWindow
    {
        public WorldComponent_TiberiumMissions Missions;

        public Vector2 scrollPosLeft = Vector2.zero;

        public Vector2 scrollPosObj = Vector2.zero;

        public Mission selectedMission;

        public MissionObjectiveDef selectedObjective;

        public int imgNum = 0;

        public List<Texture2D> imageRow = new List<Texture2D>();

        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(1316f, 756f);
            }
        }

        public override void PreOpen()
        {
            base.PreOpen();
            this.Missions = Find.World.GetComponent<WorldComponent_TiberiumMissions>();
        }

        public List<Mission> AvailableMissions
        {
            get
            {
                return (from x in this.Missions.Missions.Where((Mission x) => x.active)
                        where x.def.PrerequisitesCompleted || x.def.IsFinished
                        select x).ToList();
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            /* Look what you made me do.
            Text.Font = GameFont.Tiny;
            string s = "Thanks Mehni. Look what you made me do.";
            Vector2 v = Text.CalcSize(s);
            Rect rect = new Rect( new Vector2(0f,0f), v);
            rect.center = inRect.center;
            Widgets.DrawMenuSection(rect);
            Widgets.Label(rect, s);
            */

            if (this.selectedMission == null)
            {
                this.selectedMission = this.Missions.Missions.FirstOrDefault((Mission x) => x.def.CanStartNow);
            }
            Log.Message("x: " + inRect.width + " y: " + inRect.height);
            Widgets.DrawMenuSection(inRect);
            Widgets.DrawTextureFitted(inRect.ContractedBy(1f), TiberiumMaterials.WorkBanner, 1f);

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;

            float num = 40f;
            float yHeight = 600f;
            float yOffset = (inRect.height - yHeight) / 2f;
            Rect refRect = new Rect(0f, yOffset, inRect.width, yHeight);
            GUI.BeginGroup(refRect);
            float height = num * AvailableMissions.Count();

            Rect leftPart = new Rect(0f, 0f, 250f, refRect.height).ContractedBy(5f);

            Widgets.DrawMenuSection(leftPart);
            Rect viewRect = new Rect(0f, 0f, leftPart.width, height);
            GUI.BeginGroup(leftPart);
            Widgets.BeginScrollView(new Rect(0f, 0f, leftPart.width, leftPart.height), ref this.scrollPosLeft, viewRect, true);
            int num2 = 0;
            foreach (Mission mission in this.AvailableMissions)
            {
                Rect rect4 = new Rect(0f, (float)num2, leftPart.width, num);
                this.DoMissionTab(rect4, mission);
                num2 += (int)num;
            }
            Widgets.EndScrollView();
            GUI.EndGroup();
            Rect rightPart = new Rect(250f, 0f, refRect.width - 250, refRect.height).ContractedBy(5f);
            Widgets.DrawMenuSection(rightPart);
            rightPart = rightPart.ContractedBy(10f);
            GUI.BeginGroup(rightPart);
            if(selectedMission != null && !selectedMission.def.IsFinished)
            {
                DoObjectiveWindow(rightPart, selectedMission);
            }
            GUI.EndGroup();
            GUI.EndGroup();
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private void DoMissionTab(Rect rect, Mission mission)
        {
            //Missions.unseenMissions.Remove(mission);

            rect = rect.ContractedBy(1f);
            if (Mouse.IsOver(rect) || this.selectedMission == mission)
            {
                GUI.color = Color.yellow;
                Widgets.DrawHighlight(rect);
            }
            GUI.color = Color.white;
            Rect middleRect = new Rect(rect.height / 6, 4f, rect.width, (rect.height / 3) * 2);
            WidgetRow widgetRow = new WidgetRow(0f, 0f, UIDirection.RightThenUp, 99999f, 0f);
            if (mission != null)
            {
                widgetRow.Icon(ContentFinder<Texture2D>.Get("UI/Icons/objectiveActive", false), null);
            }
            else if (Missions.Missions.Contains(mission))
            {
                widgetRow.Icon(ContentFinder<Texture2D>.Get("UI/Icons/objectiveFailed", false), null);
            }
            else if (mission.def.IsFinished)
            {
                widgetRow.Icon(ContentFinder<Texture2D>.Get("UI/Icons/objectiveFinished", false), null);
            }
            widgetRow.Gap(4f);
            widgetRow.Label(mission.def.label, 200f);
            if (Widgets.ButtonText(new Rect(0f, 0f, widgetRow.FinalX, middleRect.height), "", false, true, Color.blue, true))
            {
                SoundDefOf.Click.PlayOneShotOnCamera(null);
                this.selectedMission = mission;
            }
        }

        private void DoObjectiveTab(Rect rect, MissionObjectiveDef obj)
        {
            rect = rect.ContractedBy(1f);
            if (Mouse.IsOver(rect) || this.selectedObjective == obj)
            {
                GUI.color = Color.yellow;
                Widgets.DrawHighlight(rect);
            }
            if (Widgets.ButtonText(rect, "", false, true, Color.blue, true))
            {
                SoundDefOf.Click.PlayOneShotOnCamera(null);
                this.selectedObjective = obj;
                imgNum = 0;
            }
            GUI.color = Color.white;
            GUI.BeginGroup(rect);
            WidgetRow widgetRow = new WidgetRow(0f, 0f, UIDirection.RightThenUp, 99999f, 4f);
            widgetRow.Gap(4f);
            widgetRow.Label(obj.label, 200f);

            Rect cornerRect = new Rect(rect.xMax - 100f, 0f, 100f, 30f).ContractedBy(5f);           
            if (obj.workCost > 0)
            {
                Widgets.FillableBar(cornerRect, obj.ProgressPct, TiberiumMaterials.blue, TiberiumMaterials.black, true);
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(cornerRect, obj.workDone + "/" + obj.workCost);
            }
            else
            {
                string s1 = obj.objective.LabelCap + (obj.IsFinished ? " 1/1": " 0/1");
                Vector2 rectSize = Text.CalcSize(s1);
                cornerRect = new Rect(rect.xMax - rectSize.x - 7f, 0f + 7f, rectSize.x, rectSize.y);
                Widgets.DrawBoxSolid(cornerRect.ExpandedBy(2f), new ColorInt(15, 11, 12).ToColor);
                Widgets.Label(cornerRect, s1);
            }


            Text.Anchor = 0;
            if(obj.stationDef != null)
            {
                Widgets.Label(new Rect(rect.x + 30, rect.height / 2, rect.width, rect.height / 2), "StationNeeded".Translate() + ": " + obj.stationDef.LabelCap);
            }
            GUI.EndGroup();
        }

        public void DoObjectiveWindow(Rect rect, Mission mission)
        {
            if (selectedObjective == null)
            {
                selectedObjective = mission.def.objectives.FirstOrDefault();
            }
            Text.Font = GameFont.Small;
            GUI.backgroundColor = new Color(0.14f, 0.14f, 0.14f);
            Rect descRect = new Rect(0, 0, rect.width/3, rect.height).ContractedBy(10f);
            Widgets.DrawMenuSection(descRect);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(selectedMission.def.description);
            sb.AppendLine("");
            sb.AppendLine(selectedObjective.description);

            Text.Anchor = TextAnchor.UpperLeft;
            Widgets.Label(descRect.ContractedBy(5f), sb.ToString());
            Text.Anchor = 0;

            Rect imageRect = new Rect(rect.width / 3, 0, (rect.width * 2 )/3, rect.height/2).ContractedBy(10f);
            Rect imageSwapL = new Rect(imageRect.x, imageRect.y, 45f, imageRect.height).ContractedBy(5f);
            Rect imageSwapR = new Rect(imageRect.xMax - 45f, imageRect.y, 45f, imageRect.height).ContractedBy(5f);          

            Widgets.DrawShadowAround(imageRect);
            Widgets.DrawBoxSolid(imageRect, new Color(0.14f, 0.14f, 0.14f));

            if (selectedObjective != null)
            {
                SetDiaShow(selectedObjective);
                if (imageRow.Count > 0 && imgNum <= (imageRow.Count -1) && imageRow[imgNum] != null)
                {
                    Widgets.DrawTextureFitted(imageRect, imageRow[imgNum], 1f);
                }
            }
            if (Mouse.IsOver(imageSwapL))
            {
                if (imgNum > 0)
                {
                    GUI.color = Color.gray;
                    Widgets.DrawHighlight(imageSwapL);
                    if (Widgets.ButtonText(imageSwapL, "", false, true, Color.blue, true))
                    {
                        imgNum -= 1;
                    }
                }
            }
            if (Mouse.IsOver(imageSwapR))
            {
                if(imgNum < imageRow.Count -1)
                {
                    GUI.color = Color.gray;
                    Widgets.DrawHighlight(imageSwapR);
                    if (Widgets.ButtonText(imageSwapR, "", false, true, Color.blue, true))
                    {
                        imgNum += 1;
                    }
                }
            }

            float num = 45f;
            float height = num * mission.def.objectives.Count();
            Rect objectiveMenu = new Rect(rect.width/3, rect.height/2, rect.width - rect.width/3, rect.height/2).ContractedBy(10f);
            Widgets.DrawMenuSection(objectiveMenu);
            Rect viewRect = new Rect(0f, 0f, objectiveMenu.width, height);

            GUI.BeginGroup(objectiveMenu);
            Widgets.BeginScrollView(new Rect(0f, 0f, objectiveMenu.width, objectiveMenu.height), ref this.scrollPosObj, viewRect, true);
            int num2 = 0;
            foreach (MissionObjectiveDef obj in mission.def.objectives)
            {
                if (!obj.IsFinished)
                {

                }
                Rect rect4 = new Rect(0f, (float)num2, objectiveMenu.width, num);
                DoObjectiveTab(rect4, obj);
                num2 += (int)num;
            }
            Widgets.EndScrollView();
            GUI.EndGroup();
        }

        public void SetDiaShow(MissionObjectiveDef objective)
        {
            imageRow.Clear();
            foreach (string s in objective.images)
            {
                Texture2D text = ContentFinder<Texture2D>.Get(s, true);
                imageRow.Add(text);
            }
        }
    }
}
