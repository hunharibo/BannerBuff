using BannerBuff.Behaviours;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.CampaignSystem.Siege;
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
            if (__instance != null && __instance.LeaderHero != null 
                && BuffManager.Instance.IsBuffCurrentlyActiveForHero(__instance.LeaderHero, "bannerbuff_partyspeedbuff_1"))
            {
                __result += 3f;
            }            
        }

        [HarmonyPostfix]
        [HarmonyPatch("SpeedExplanation", MethodType.Getter)]
        public static void PostFix2(ref StatExplainer __result, MobileParty __instance)
        {
            if (__instance != null && __instance.LeaderHero != null && 
                BuffManager.Instance.IsBuffCurrentlyActiveForHero(__instance.LeaderHero, "bannerbuff_partyspeedbuff_1"))
            {
                __result.AddLine("Active Speed Buff", 3f, StatExplainer.OperationType.Add);
            }
        }
    }
}
