using RimWorld;
using Verse;

namespace GojisDryadTweaks
{
    [DefOf]
    public static class GojiDefsOf
    {
        public static PreceptDef Goji_DryadLifespan_Immortal;
        public static PreceptDef Goji_DryadAutonomy_High;
        public static PreceptDef Goji_GauranlenConnection_NatureAttuned;
        public static PreceptDef Goji_GauranlenConnection_CompromisedPact;
        public static ThingDef PlantPot_Bonsai;
        public static JobDef Goji_EnterHealingPod;
        public static JobDef Goji_EnterCocoon;
        public static ThingDef Goji_HumanoidHealingCocoon, Goji_DryadCocoon;
        public static ThoughtDef Goji_TerribleCocoonRitual;
        public static ThoughtDef Goji_BoringCocoonRitual;
        public static ThoughtDef Goji_BeautifulCocoonRitual;
        public static ThoughtDef Goji_UnforgettableCocoonRitual;

        [MayRequire("vanillaracesexpanded.phytokin")]
        public static GeneDef VRE_GreenThumb;
        [MayRequire("vanillaracesexpanded.phytokin")]
        public static ThoughtDef VRE_GreenThumbHappy;
        [MayRequire("vanillaracesexpanded.phytokin")]
        public static GeneDef VRE_GauranlenAffinity;
        [MayRequire("vanillaracesexpanded.phytokin")]
        public static PawnKindDef VRE_CompanionDryad;
        static GojiDefsOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(GojiDefsOf));
        }
    }
}
