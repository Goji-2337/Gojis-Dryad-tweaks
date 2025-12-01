using System;
using HarmonyLib;
using LudeonTK;
using RimWorld;
using UnityEngine;
using Verse;

namespace GojisDryadTweaks
{
    [HarmonyPatch(typeof(GenThing), nameof(GenThing.TrueCenter), new Type[] { typeof(Thing) })]
    public static class GenThing_TrueCenter_Patch
    {
        [TweakValue("0GojisDryadTweaks", -1f, 1f)]
        public static float GauranlenTreeInBonsaiPotOffset = 0.6f;
        public static void Postfix(Thing t, ref Vector3 __result)
        {
            if (t != null && t.IsGauranlenTreeInBonsaiPot())
            {
                __result.z += GauranlenTreeInBonsaiPotOffset;
            }
        }
    }
}
