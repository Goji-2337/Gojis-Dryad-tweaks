using Verse;
using RimWorld;
using Verse.Sound;

namespace GojisDryadTweaks
{
    public class CompDryadCocoonUntilReturnToTree : CompDryadHolder
    {
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (!respawningAfterLoad)
            {
                innerContainer = new ThingOwner<Thing>(this, oneStackOnly: false);
            }
        }

        public override void TryAcceptPawn(Pawn p)
        {
            base.TryAcceptPawn(p);
            p.Rotation = Rot4.South;
        }

        public override void Complete()
        {
            CompTreeConnection treeComp = base.TreeComp;
            if (treeComp != null && innerContainer.Count > 0)
            {
                EjectDryad(parent.Map);
                EffecterDefOf.DryadEmergeFromCocoon.Spawn(parent.Position, parent.Map).Cleanup();
            }
            parent.Destroy();
        }

        public override void PostDeSpawn(Map map, DestroyMode mode = DestroyMode.Vanish)
        {
            EjectDryad(map);
        }

        private void EjectDryad(Map map)
        {
            innerContainer.TryDropAll(parent.Position, map, ThingPlaceMode.Near, delegate (Thing t, int c)
            {
                t.Rotation = Rot4.South;
                SoundDefOf.Pawn_Dryad_Spawn.PlayOneShot(parent);
            }, null, playDropSound: false);
        }

        public override void CompTick()
        {
            base.CompTick();
            if (!parent.Destroyed)
            {
                if (innerContainer.Count > 0 && innerContainer[0] is Pawn dryad && tree != null)
                {
                    if (base.TreeComp.dryads.Count > base.TreeComp.MaxDryads)
                    {
                        return;
                    }
                    Complete();
                }
            }
        }
    }
}
