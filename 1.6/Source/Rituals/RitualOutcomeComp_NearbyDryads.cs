using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GojisDryadTweaks
{
    public class RitualOutcomeComp_NearbyDryads : RitualOutcomeComp
    {
        public float qualityOffsetPerDryad = 0.03f;
        public float maxQualityOffset = 0.35f;
        public float searchRadius = 15f;
        public override bool Applies(LordJob_Ritual ritual)
        {
            return true;
        }

        public override float QualityOffset(LordJob_Ritual ritual, RitualOutcomeComp_Data data)
        {
            int dryadCount = 0;
            TargetInfo target = ritual.selectedTarget;

            if (target == null || !target.HasThing || target.Thing.Map == null)
            {
                return 0f;
            }

            Map map = target.Thing.Map;
            IntVec3 center = target.Thing.Position;


            foreach (Pawn p in map.mapPawns.AllPawnsSpawned)
            {
                if (p.Position.InHorDistOf(center, searchRadius) && p.RaceProps != null && p.RaceProps.Dryad)
                {
                    dryadCount++;
                }
            }

            float offset = Mathf.Min(dryadCount * qualityOffsetPerDryad, maxQualityOffset);
            return offset;
        }

        public override string GetDesc(LordJob_Ritual ritual = null, RitualOutcomeComp_Data data = null)
        {
            string bonusDesc = $"({qualityOffsetPerDryad:P0} {"Goji_PerDryad".Translate()}, {"Goji_Max".Translate()} +{maxQualityOffset:P0})";
            return $"{label.CapitalizeFirst()} +{QualityOffset(ritual, data):P0} {bonusDesc}";
        }
    }
}
