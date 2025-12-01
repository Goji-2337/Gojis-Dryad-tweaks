using HarmonyLib;
using Verse;

namespace GojisDryadTweaks
{
    [HarmonyPatch(typeof(Pawn), "IsColonistPlayerControlled", MethodType.Getter)]
    public static class Pawn_IsColonistPlayerControlled_Patch
    {
        public static void Postfix(Pawn __instance, ref bool __result)
        {
            if (Pawn_ConnectionsTracker_GetGizmos_Patch.isRunning is false && __instance.IsDraftableControllableAnimal())
            {
                __result = true;
            }
        }
    }
}
