using BannerBuff.Services;
using System;
using TaleWorlds.CampaignSystem;

namespace BannerBuff.Behaviours
{
    class BuffManagerBehaviour : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, onHourlyTick);
            CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, onGameLoaded);
            CampaignEvents.OnNewGameCreatedEvent2.AddNonSerializedListener(this, onNewGame);
        }

        private void onNewGame() => BuffManager.Instance.OnNewGame();
        private void onGameLoaded(CampaignGameStarter obj) => BuffManager.Instance.OnGameLoaded();
        private void onHourlyTick() => BuffManager.Instance.OnHourlyTick();

        public override void SyncData(IDataStore dataStore)
        {
            
        }
    }
}
