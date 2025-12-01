using HarmonyLib;
using Verse;

namespace GojisDryadtweaks
{
    public class GojisDryadtweaksMod : Mod
    {
        public GojisDryadtweaksMod(ModContentPack pack) : base(pack)
        {
            new Harmony("GojisDryadtweaksMod").PatchAll();
        }
    }
}