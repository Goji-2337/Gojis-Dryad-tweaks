using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;

namespace GojisDryadTweaks
{
    [HarmonyPatch(typeof(CompTreeConnection), nameof(CompTreeConnection.ConnectionStrength), MethodType.Setter)]
    public static class CompTreeConnection_ConnectionStrength_Patch
    {
        public static void Prefix(CompTreeConnection __instance, ref float value)
        {
            if (__instance.parent.IsGauranlenTreeInBonsaiPot())
            {
                value = Mathf.Min(value, 0.5f);
            }
        }
    }
}
