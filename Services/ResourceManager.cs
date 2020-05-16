using BannerBuff.TypeDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using Newtonsoft.Json;
using ResourceType = BannerBuff.TypeDefinitions.ResourceType;
using SandBox.View.Map;
using TaleWorlds.Engine.Screens;
using BannerBuff.Screens;
using TaleWorlds.Engine;

namespace BannerBuff.Services
{
    public class ResourceChangedEventArgs : EventArgs
    {
        public Hero hero { get; set; }
        public ResourceType resourcetype { get; set; }
        public int changeAmount { get; set; }
        public int currentAmount { get; set; }
    }

    class ResourceManager : IDisposable
    {
        private readonly Dictionary<Hero, Dictionary<ResourceType, ResourceState>> _dictionary = new Dictionary<Hero, Dictionary<ResourceType, ResourceState>>();
        private bool _isMapOpen = false;
        public event EventHandler<ResourceChangedEventArgs> onResourceAmountChanged;
        private GauntletResourceView _uiView;

        private static ResourceManager _manager;
        public static ResourceManager Instance {
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
        #region ApplicationEvents
        private void ScreenManager_OnPushScreen(ScreenBase pushedScreen)
        {
            if(!(pushedScreen is MapScreen))
            {
                this._isMapOpen = false;
                this.ToggleShowUI();
            }
        }

        internal void OnApplicationTick(float dt)
        {
            if(!this._isMapOpen && ScreenManager.TopScreen is MapScreen)
            {
                this._isMapOpen = true;
                this.ToggleShowUI();
            }
        }

        internal void onGameLoaded(CampaignGameStarter obj)
        {
            //Hopefully LoadFromSaveGame has run by now so _dictionary is valid.
            if (this._dictionary.Count == 0)//load was unsuccessful :(
            {
                this.InitializeBlankState();
            }
        }

        internal void onNewGameCreated()
        {
            this.InitializeBlankState();
        }
        #endregion

        #region ManagerServiceMethods
        private void InitializeBlankState()
        {
            foreach(var hero in Hero.All)
            {
                if (this.checkHeroValidity(hero)) this.InitHeroStarterResources(hero);
            }
        }

        public void AddResourceToHero(Hero to, int amount, ResourceType type)
        {
            if(this.checkHeroValidity(to)) this.AddResourceToHeroInternal(to, amount, type);
        }

        private void InitHeroStarterResources(Hero hero)
        {
            if (checkHeroValidity(hero))
            {
                //TODO make this XML driven.
                Dictionary<ResourceType, ResourceState> resources = new Dictionary<ResourceType, ResourceState>();
                resources.Add(ResourceType.Magicka, new ResourceState { MinAmount = 0, MaxAmount = 100, CurrentAmount = 0, CurrentRechargeRatePerHour = 1, DefaultRechargeRatePerHour = 1 });
                this._dictionary.Add(hero, resources);
            }
        }

        private bool checkHeroValidity(Hero hero)
        {
            return hero.IsAlive && hero.IsActive && hero.CharacterObject.Occupation == Occupation.Lord;
        }

        private void AddResourceToHeroInternal(Hero to, int amount, ResourceType type)
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

            Dictionary<ResourceType, ResourceState> herocurrentresourcestate = this._dictionary[to];
            if(herocurrentresourcestate != null && herocurrentresourcestate.ContainsKey(type))
            {
                ResourceState currentstate = herocurrentresourcestate[type];
                int oldval = currentstate.CurrentAmount;
                int newval = oldval + amount;
                newval = Math.Min(newval, currentstate.MaxAmount);
                newval = Math.Max(newval, currentstate.MinAmount);
                currentstate.CurrentAmount = newval;
                if(oldval != newval && this.onResourceAmountChanged != null) this.onResourceAmountChanged(this, new ResourceChangedEventArgs { hero = to, resourcetype = type, changeAmount = newval - oldval, currentAmount = newval });
            }
        }
        public static bool IsHeroValid(Hero hero)
        {
            return ResourceManager.Instance.checkHeroValidity(hero);
        }

        public ResourceState GetResourceForHero(Hero hero, ResourceType type)
        {
            if (this._dictionary.ContainsKey(hero))
            {
                if (this._dictionary[hero].ContainsKey(type))
                {
                    return this._dictionary[hero][type];
                }
            }
            return null;
        }

        public ResourceState GetResourceForMainHero(ResourceType type)
        {
            return this.GetResourceForHero(Hero.MainHero, type);
        }
        #endregion

        #region SaveSystem
        internal string GetDataForSaveGame()
        {
            try
            {
                Dictionary<string, Dictionary<ResourceType, ResourceState>> savedata = new Dictionary<string, Dictionary<ResourceType, ResourceState>>();
                foreach (var item in this._dictionary)
                {
                    savedata.Add(item.Key.StringId, item.Value);
                }
                return JsonConvert.SerializeObject(savedata);
            }
            catch (Exception)
            {
                throw;
            }
        }
        internal void LoadFromSaveGame(string jsondata)
        {
            try
            {
                //var savedstate = JsonSerializer.Deserialize<Dictionary<string, Dictionary<ResourceType, ResourceState>>>(jsondata);
                var savedstate = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<ResourceType, ResourceState>>>(jsondata);
                foreach (var heroid in savedstate.Keys)
                {
                    var match = from x in Hero.All where x.StringId == heroid select x;
                    if (match != null)
                    {
                        this._dictionary.Add(match.ElementAt(0), savedstate[match.ElementAt(0).StringId]);
                    }
                }
            }
            catch (Exception)
            {
                //smthing is up, initialize blank state
            }
        }
        
        #endregion

        public void Dispose()
        {
            ScreenManager.OnPushScreen -= ScreenManager_OnPushScreen;
        }

        internal void onHourlyTick()
        {
            foreach(var hero in this._dictionary.Keys)
            {
                foreach(var resource in this._dictionary[hero])
                {
                    this.AddResourceToHeroInternal(hero, resource.Value.CurrentRechargeRatePerHour, resource.Key);
                }
            }
        }

        #region UI stuff

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
                }/*
                else
                {
                    (screen as MapScreen).GetMapView<GauntletResourceView>();
                }*/
            }
            catch (Exception)
            {
                throw;
            }

        }
        #endregion
    }
}
