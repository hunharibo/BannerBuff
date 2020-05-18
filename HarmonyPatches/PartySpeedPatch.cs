using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Localization;

namespace BannerBuff.HarmonyPatches
{
    [HarmonyPatch(typeof(MobileParty))]
    public class PartySpeedPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("ComputeSpeed")]
        public static void PostFix1(ref float __result, MobileParty __instance)
        {
            __result += 3f;
        }

        [HarmonyPostfix]
        [HarmonyPatch("SpeedExplanation", MethodType.Getter)]
        public static void PostFix2(ref StatExplainer __result, MobileParty __instance)
        {
            __result.AddLine("Active Speed Buff", 3f, StatExplainer.OperationType.Add);
        }

    }
}
