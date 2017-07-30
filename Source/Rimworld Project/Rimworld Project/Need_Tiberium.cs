using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;


namespace TiberiumRim
{
    public class Need_Tiberium : Need
    {
        private const float ThreshDesire = 0.01f;

        private const float ThreshSatisfied = 0.3f;

        private float messageTick;

        public override int GUIChangeArrow
        {
            get
            {
                return -1;
            }
        }

        public TiberiumNeedCategory CurCategory
        {
            get
            {
                if (this.CurLevel > 0.3f)
                {
                    return TiberiumNeedCategory.Statisfied;
                }
                if (this.CurLevel > 0.01f)
                {
                    return TiberiumNeedCategory.Lacking;
                }
                return TiberiumNeedCategory.Urgent;
            }
        }

        private bool canMessage
        {
            get
            {
                return messageTick > 25;
            }

        }

        public override float CurLevel
        {
            get
            {
                return base.CurLevel;
            }
            set
            {
                TiberiumNeedCategory curCategory = this.CurCategory;
                base.CurLevel = value;
            }
        }

        private float TiberiumNeedFallPerTick
        {
            get
            {
                return this.def.fallPerDay / 60000f;
            }
        }

        public Need_Tiberium(Pawn pawn) : base(pawn)
		{
            this.threshPercents = new List<float>();
            this.threshPercents.Add(0.3f);
        }

        public override void SetInitialLevel()
        {
            base.CurLevelPercentage = Rand.Range(0.2f, 0.5f);
        }

        public override void NeedInterval()
        {
            if (this.pawn != null && this.pawn.Map != null)
            {
                if (this.pawn.Position.InBounds(this.pawn.Map))
                {
                    if (this.pawn.CarriedBy != null)
                    {
                        return;
                    }
                    var c = this.pawn.RandomAdjacentCell8Way();
                    if (c.InBounds(this.pawn.Map))
                    {
                        var t = c.GetPlant(this.pawn.Map);
                        if (t != null)
                        {
                            if (t.def.defName.Contains("Tiberium"))
                            {
                                this.CurLevel += 0.05f;
                                return;
                            }
                        }
                        this.CurLevel -= this.TiberiumNeedFallPerTick * 350f;
                    }
                }
            }

            messageTick += 1;

            if (this.CurLevel < this.MaxLevel * 0.3)
            {
                JobDef job = DefDatabase<JobDef>.GetNamed("TiberiumBath");
                Thing targetA = this.pawn.Map.listerThings.AllThings.Find((Thing x) => x.def.defName.Contains("Tiberium"));
                if (targetA != null && targetA.def.plant != null)
                {
                    if (pawn.CanReach(targetA, PathEndMode.OnCell, Danger.Deadly, false))
                    {
                        this.pawn.jobs.TryTakeOrderedJob(new Job(job, targetA));
                        return;
                    }                   
                }
                if (canMessage)
                {
                    Messages.Message("CannotReachTiberium".Translate(), new TargetInfo(pawn.Position, pawn.Map, false), MessageSound.Standard);
                    messageTick = 0;
                }
            }
        }
    }
}
