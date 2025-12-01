using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace GojisDryadTweaks
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class HotSwappableAttribute : Attribute
    {
    }
}
