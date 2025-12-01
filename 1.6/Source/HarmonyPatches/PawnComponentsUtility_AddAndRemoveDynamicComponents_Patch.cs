using HarmonyLib;
using RimWorld;
using Verse;

namespace GojisDryadTweaks
{
    [HarmonyPatch(typeof(PawnComponentsUtility), "AddAndRemoveDynamicComponents")]
    public static class PawnComponentsUtility_AddAndRemoveDynamicComponents_Patch
    {
        public static void Postfix(Pawn pawn)
        {
            if (pawn.IsDraftableControllableAnimal())
            {
                pawn.drafter ??= new Pawn_DraftController(pawn);
                pawn.equipment ??= new Pawn_EquipmentTracker(pawn);
                pawn.playerSettings ??= new Pawn_PlayerSettings(pawn);
            }
        }
    }
}
