using BannerBuff.TypeDefinitions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;

namespace BannerBuff.HarmonyPatches
{
    public class PartySpeedPatch : BuffPatch
    {
        public override void PostFix(ref object __result)
        {
            __result = 2;
        }
    }
}
