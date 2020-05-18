using BannerBuff.Screens;
using BannerBuff.TypeDefinitions;
using SandBox.View.Map;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine.Screens;
using TaleWorlds.SaveSystem;

namespace BannerBuff.Behaviours
{
    class ResourceManager : CampaignBehaviorBase
    {
        private Dictionary<Hero, ResourceState> _dictionary = new Dictionary<Hero, ResourceState>();
        private bool _isMapOpen = false;
        public event EventHandler<ResourceChangedEventArgs> onResourceAmountChanged;
        private GauntletResourceView _uiView;

        private static ResourceManager _manager;
        public static ResourceManager Instance
        {
            get
            {
                if (_manager == null) _manager = new ResourceManager();
                return _manager;
            }
        }

        private ResourceManager()
        {
            ScreenManager.OnPushScreen += ScreenManager_OnPushScreen;
        }

        private void ScreenManager_OnPushScreen(ScreenBase pushedScreen)
        {
            if (!(pushedScreen is MapScreen))
            {
                this._isMapOpen = false;
                this.ToggleShowUI();
            }
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnNewGameCreatedEvent2.AddNonSerializedListener(this, onNewGameCreated);
            CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, onGameLoaded);
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, onHourlyTick);
            CampaignEvents.TickEvent.AddNonSerializedListener(this, onTick);
        }

        private void onTick(float obj)
        {
            if (!this._isMapOpen && ScreenManager.TopScreen is MapScreen)
            {
                this._isMapOpen = true;
                this.ToggleShowUI();
            }
        }

        private void onHourlyTick()
        {
            foreach (var hero in this._dictionary.Keys)
            {
                this.AddResourceToHeroInternal(hero, this._dictionary[hero].CurrentRechargeRatePerHour);
            }
        }

        private void onGameLoaded(CampaignGameStarter obj)
        {
            if (this._dictionary == null || this._dictionary.Count == 0) this.InitializeBlankState();
        }

        private void onNewGameCreated() => this.InitializeBlankState();        

        private void InitializeBlankState()
        {
            foreach (var hero in Hero.All)
            {
                if (this.checkHeroValidity(hero)) this.InitHeroStarterResources(hero);
            }
        }

        public void AddResourceToHero(Hero to, int amount)
        {
            if (this.checkHeroValidity(to)) this.AddResourceToHeroInternal(to, amount);
        }

        private void InitHeroStarterResources(Hero hero)
        {
            if (checkHeroValidity(hero))
            {
                this._dictionary.Add(hero, new ResourceState());
            }
        }

        private bool checkHeroValidity(Hero hero)
        {
            return hero.IsAlive && hero.IsActive && hero.CharacterObject.Occupation == Occupation.Lord;
        }

        private void AddResourceToHeroInternal(Hero to, int amount)
        {
            if (this._dictionary.Count == 0)
            {
                this.InitializeBlankState(); //HACK
            }
            if (to == Hero.MainHero && !this._dictionary.ContainsKey(to))
            {
                this.InitHeroStarterResources(to); //Also a bit hacky, main hero is not available at initblank time
            }
            else if (!this._dictionary.ContainsKey(to)) return; //no lord found - TODO: maybe add if new heros are spawned during game???

            var state = this._dictionary[to];
            if (state != null)
            {
                int oldval = state.CurrentAmount;
                int newval = oldval + amount;
                newval = Math.Min(newval, state.MaxAmount);
                newval = Math.Max(newval, state.MinAmount);
                state.CurrentAmount = newval;
                if (oldval != newval && this.onResourceAmountChanged != null) this.onResourceAmountChanged(this, new ResourceChangedEventArgs { hero = to, changeAmount = newval - oldval, currentAmount = newval });
            }
        }
        public bool IsHeroValid(Hero hero)
        {
            return this.checkHeroValidity(hero);
        }

        public ResourceState GetResourceForHero(Hero hero)
        {
            if (this._dictionary.ContainsKey(hero))
            {
                if (this._dictionary[hero] != null)
                {
                    return this._dictionary[hero];
                }
            }
            else if (hero == Hero.MainHero)
            {
                this.InitHeroStarterResources(hero);
                return GetResourceForMainHero();
            }
            return null;
        }

        public ResourceState GetResourceForMainHero()
        {
            return this.GetResourceForHero(Hero.MainHero);
        }

        #region savesystem
        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("BannerBuffSaveData_Resources", ref _dictionary);
        }
        #endregion
        private void ToggleShowUI()
        {
            try
            {
                var screen = ScreenManager.TopScreen;
                if (this._isMapOpen && screen is MapScreen)
                {
                    var mapscreen = screen as MapScreen;
                    if (mapscreen.GetMapView<GauntletResourceView>() == null)
                    {
                        this._uiView = mapscreen.AddMapView<GauntletResourceView>() as GauntletResourceView;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        public class SaveableTypes : SaveableTypeDefiner
        {
            public SaveableTypes() : base(770001) { }

            protected override void DefineClassTypes()
            {
                AddClassDefinition(typeof(ResourceState), 1);
            }

            protected override void DefineContainerDefinitions()
            {
                ConstructContainerDefinition(typeof(Dictionary<Hero, ResourceState>));
            }
        }
    }

}