using BannerBuff.Behaviours;
using BannerBuff.Services;
using HarmonyLib;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
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

        protected override void OnApplicationTick(float dt)
        {
            base.OnApplicationTick(dt);
            ResourceManager.Instance.OnApplicationTick(dt);
        }

        private void AddBehaviours(CampaignGameStarter starter)
        {
            starter.AddBehavior(new ResourceManagerBehaviour());
            starter.AddBehavior(new BuffManagerBehaviour());
        }
    }
}