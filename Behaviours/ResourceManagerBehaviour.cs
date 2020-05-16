using BannerBuff.Services;
using System;
using TaleWorlds.CampaignSystem;

namespace BannerBuff.Behaviours
{
    class ResourceManagerBehaviour : CampaignBehaviorBase
    {
        internal const string saveDataID = "4bd08a3b-8f6e-4296-b1a2-f3c9d687aed3";
        public override void RegisterEvents()
        {
            CampaignEvents.OnNewGameCreatedEvent2.AddNonSerializedListener(this, onNewGameCreated);
            CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, onGameLoaded);
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, onHourlyTick);
        }

        private void onHourlyTick() => ResourceManager.Instance.onHourlyTick();
        private void onGameLoaded(CampaignGameStarter obj) => ResourceManager.Instance.onGameLoaded(obj);
        private void onNewGameCreated() => ResourceManager.Instance.onNewGameCreated();


        // savedata object ref for sync - flattened json string for example
        // need to override this for save -> get current data from manager -> write method
        // on load call construct state from savegame -> write method in manager
        public override void SyncData(IDataStore dataStore)
        {
            try
            {
                string savedataJson = null;
                if (dataStore.IsSaving)
                {
                    savedataJson = ResourceManager.Instance.GetDataForSaveGame();
                }

                dataStore.SyncData<string>(saveDataID, ref savedataJson);

                string data2 = savedataJson;

                if (dataStore.IsLoading && savedataJson != null)
                {
                    ResourceManager.Instance.LoadFromSaveGame(savedataJson);
                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
