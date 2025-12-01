using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;

namespace GojisDryadTweaks
{
    public class CompProperties_HumanoidHealingCocoon : CompProperties
    {
        public float daysToComplete = 3f;
        public bool drawContents = true;

        public CompProperties_HumanoidHealingCocoon()
        {
            compClass = typeof(CompHumanoidHealingCocoon);
        }
    }

    [StaticConstructorOnStartup]
    public class CompHumanoidHealingCocoon : ThingComp, IThingHolder, ISuspendableThingHolder
    {
        protected int tickComplete = -1;
        protected ThingOwner innerContainer;
        protected Pawn containedPawn;
        protected int outcomePositivityIndex = 0;
        public CompProperties_HumanoidHealingCocoon Props => (CompProperties_HumanoidHealingCocoon)props;
        public bool IsContentsSuspended => true;
        private Material cachedFrontMat;
        private Material FrontMat
        {
            get
            {
                if (cachedFrontMat == null)
                {

                    cachedFrontMat = MaterialPool.MatFrom("Things/Building/Misc/DryadSphere/DryadSphereFront", ShaderDatabase.Cutout);
                }
                return cachedFrontMat;
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                innerContainer = new ThingOwner<Thing>(this, oneStackOnly: false);
            }
        }

        public override void CompTick()
        {
            base.CompTick();
            if (containedPawn != null && !containedPawn.Dead)
            {
                if (tickComplete >= 0 && Find.TickManager.TicksGame >= tickComplete)
                {
                    Complete();
                }
            }
            else if (tickComplete >= 0)
            {
                Complete();
            }
        }

        private void HealAllDiseases()
        {
            List<Hediff> diseases = new List<Hediff>();
            foreach (Hediff hediff in containedPawn.health.hediffSet.hediffs)
            {
                if (hediff.def.isBad && hediff.def.makesSickThought || hediff.def.HasModExtension<DiseaseMarker>())
                {
                    diseases.Add(hediff);
                }
            }

            foreach (Hediff disease in diseases)
            {
                containedPawn.health.RemoveHediff(disease);
            }
        }

        protected virtual void Complete()
        {
            if (containedPawn != null)
            {
                if (outcomePositivityIndex == -2)
                {
                    ApplyRandomWound();
                }
                else if (outcomePositivityIndex >= 1)
                {
                    HealAllWounds();
                    HealAllDiseases();
                    if (outcomePositivityIndex >= 3)
                    {
                        HealOnePermanentWound();
                    }
                }
                EjectPawn();
            }
            if (!parent.Destroyed)
            {
                parent.Destroy(DestroyMode.Vanish);
            }
        }

        protected void HealAllWounds()
        {
            List<Hediff_Injury> injuries = new List<Hediff_Injury>();
            foreach (Hediff hediff in containedPawn.health.hediffSet.hediffs)
            {
                if (hediff is Hediff_Injury injury && !injury.IsPermanent() && injury.CanHealNaturally())
                {
                    injuries.Add(injury);
                }
            }

            foreach (Hediff_Injury injury in injuries)
            {
                containedPawn.health.RemoveHediff(injury);
            }

            Messages.Message("Goji_CocoonHealedAllWounds".Translate(containedPawn.LabelCap), containedPawn, MessageTypeDefOf.PositiveEvent);
        }

        protected void HealOnePermanentWound()
        {
            Hediff_Injury permanentInjury = null;
            foreach (Hediff hediff in containedPawn.health.hediffSet.hediffs)
            {
                if (hediff is Hediff_Injury injury && injury.IsPermanent())
                {
                    permanentInjury = injury;
                    break;
                }
            }

            if (permanentInjury != null)
            {
                Messages.Message("Goji_CocoonHealedPermanentWound".Translate(containedPawn.LabelCap, permanentInjury.Part.Label), containedPawn, MessageTypeDefOf.PositiveEvent);
                containedPawn.health.RemoveHediff(permanentInjury);
            }
        }
        protected void ApplyRandomWound()
        {
            BodyPartRecord partRecord = containedPawn.health.hediffSet.GetRandomNotMissingPart(DamageDefOf.Scratch, BodyPartHeight.Undefined, BodyPartDepth.Outside);
            if (partRecord == null)
            {
                partRecord = containedPawn.health.hediffSet.GetRandomNotMissingPart(DamageDefOf.Blunt, BodyPartHeight.Undefined, BodyPartDepth.Outside);
            }
            if (partRecord == null) return;
            float severity = Rand.Range(1f, 5f);
            DamageInfo dinfo = new DamageInfo(DamageDefOf.Scratch, severity, 0f, -1f, null, partRecord);
            containedPawn.TakeDamage(dinfo);
        }
        public virtual void AssignPawn(Pawn p, int outcomeIndex)
        {
            if (p == null)
            {
                Log.Error("Tried to assign a null pawn to HumanoidHealingCocoon.");
                return;
            }

            bool pawnSelected = Find.Selector.SelectedObjects.Contains(p);
            p.DeSpawnOrDeselect();

            if (innerContainer.TryAddOrTransfer(p, 1) > 0)
            {
                containedPawn = p;
                outcomePositivityIndex = outcomeIndex;
                tickComplete = Find.TickManager.TicksGame + (int)(Props.daysToComplete * 60000f);
                SoundDefOf.Pawn_EnterDryadPod.PlayOneShot(SoundInfo.InMap(parent));
                if (pawnSelected)
                {
                    Find.Selector.Select(p, playSound: false, forceDesignatorDeselect: false);
                }
            }
            else
            {
                Log.Error($"Could not add pawn {p.LabelShort} to HumanoidHealingCocoon container.");
            }
        }

        public virtual void EjectPawn()
        {
            if (containedPawn != null)
            {
                innerContainer.TryDropAll(parent.Position, parent.Map, ThingPlaceMode.Near);
                containedPawn = null;
                tickComplete = -1;
            }
        }

        public override void PostDeSpawn(Map map, DestroyMode mode = DestroyMode.Vanish)
        {
            base.PostDeSpawn(map, mode);
            EjectPawn();
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            EjectPawn();
            base.PostDestroy(mode, previousMap);
        }

        public override void PostDraw()
        {
            base.PostDraw();
            if (Props.drawContents && containedPawn != null)
            {

                Vector3 drawPos = parent.Position.ToVector3ShiftedWithAltitude(AltitudeLayer.BuildingOnTop);
                containedPawn.Drawer.renderer.RenderPawnAt(drawPos, Rot4.South, neverAimWeapon: true);

                Matrix4x4 matrix = default(Matrix4x4);
                Vector3 pos = parent.Position.ToVector3ShiftedWithAltitude(AltitudeLayer.BuildingOnTop.AltitudeFor() + 0.01f);
                Quaternion q = Quaternion.identity;
                Vector3 s = new Vector3(parent.Graphic.drawSize.x, 1f, parent.Graphic.drawSize.y);
                matrix.SetTRS(pos, q, s);
                Graphics.DrawMesh(MeshPool.plane10, matrix, FrontMat, 0);
            }
        }

        public override string CompInspectStringExtra()
        {
            string text = base.CompInspectStringExtra();
            if (!text.NullOrEmpty())
            {
                text += "\n";
            }
            if (containedPawn != null)
            {
                text += "CasketContains".Translate() + ": " + containedPawn.LabelCap;
                if (tickComplete >= 0)
                {
                    text += "\n" + "TimeLeft".Translate().CapitalizeFirst() + ": " + (tickComplete - Find.TickManager.TicksGame).ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
                }
            }
            else
            {
                text += "Empty".Translate().CapitalizeFirst();
            }

            return text;
        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return innerContainer;
        }

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref tickComplete, "tickComplete", -1);
            Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
            Scribe_References.Look(ref containedPawn, "containedPawn");
            Scribe_Values.Look(ref outcomePositivityIndex, "outcomePositivityIndex", 0);

            if (Scribe.mode == LoadSaveMode.PostLoadInit && innerContainer != null)
            {

                if (innerContainer.Count > 0 && innerContainer[0] is Pawn p)
                {
                    containedPawn = p;
                }
                else
                {
                    containedPawn = null;
                }
            }
        }
    }
}
