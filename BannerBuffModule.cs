using BannerBuff.Behaviours;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace BannerBuff
{
    public class BannerBuffModule : MBSubModuleBase
    {
        public static string ModuleName => "BannerBuff";
        public static string ModuleDirectoryName => BannerBuffModule.ModuleName;

        protected override void OnSubModuleLoad() 
        {
            Harmony harmony = new Harmony("mod.harmony.bannerbuff");
            harmony.PatchAll();
        }

        protected override void OnSubModuleUnloaded() { }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            InformationManager.DisplayMessage(new InformationMessage(BannerBuffModule.ModuleName + " loaded."));
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            if (game.GameType is Campaign)
            {
                //The current game is a campaign
                CampaignGameStarter campaignStarter = (CampaignGameStarter)gameStarterObject;
                this.AddBehaviours(campaignStarter);
            }
        }

        private void AddBehaviours(CampaignGameStarter starter)
        {
            starter.AddBehavior(ResourceManager.Instance);
            starter.AddBehavior(BuffManager.Instance);
        }
    }
}