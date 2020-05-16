using BannerBuff.TypeDefinitions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace BannerBuff.Services
{
    class ActiveBuffsChangedEventArgs : EventArgs
    {
        public Hero hero { get; set; }
        public HeroBuffState newState { get; set; }
        public string removedBuffID { get; set; }
    }

    class BuffManager
    {
        private readonly Dictionary<Hero, HeroBuffState> _dictionary = new Dictionary<Hero, HeroBuffState>();
        private readonly Dictionary<string, Buff> _registeredbuffs = new Dictionary<string, Buff>();
        private static BuffManager _instance = null;
        public event EventHandler<ActiveBuffsChangedEventArgs> OnActiveBuffRemoved;
        private ResourceDepot _depot = null;
        public static BuffManager Instance {
            get
            {
                if (BuffManager._instance == null) BuffManager._instance = new BuffManager();
                return BuffManager._instance;
            }
        }

        private BuffManager()
        {
            try
            {
                Initialize();
            }
            catch (Exception e)
            {
                //TODO handle gracefully XML schema errors.
                throw e;
            }
        }

        private void Initialize()
        {
            this._depot = new ResourceDepot(BasePath.Name);
            this._depot.AddLocation("Modules/BannerBuff/ModuleData/");
            this._depot.CollectResources();
            var files = this._depot.GetFiles("", ".xml");
            if(files.Length > 0)
            {
                foreach (var path in files)
                {
                    if (Path.GetFileNameWithoutExtension(path) == "BannerBuffs")
                    {
                        XmlSerializer ser = new XmlSerializer(typeof(List<Buff>));
                        var list = ser.Deserialize(File.OpenRead(path)) as List<Buff>;
                        if (list != null)
                        {
                            foreach(var item in list)
                            {
                                this._registeredbuffs.Add(item.stringID, item);
                            }
                        }
                    }
                }
            }
        }

        internal void OnGameLoaded()
        {
            this.InitializeBlankState(); //TODO implement loading from savegame.
        }

        internal void OnNewGame()
        {
            this.InitializeBlankState();
        }

        private void InitializeBlankState()
        {
            foreach(var hero in Hero.All)
            {
                this.InitializeStateForHero(hero);
            }
        }

        private void InitializeStateForHero(Hero hero)
        {
            if (IsHeroValidForBuffs(hero))
            {
                if (!this._dictionary.ContainsKey(hero))
                {
                    this._dictionary.Add(hero, this.GetBlankState());
                }
            }
        }

        private HeroBuffState GetBlankState()
        {
            HeroBuffState state = new HeroBuffState();
            foreach(var item in this._registeredbuffs)
            {//TODO might want to alter this behaviour. This auto-assigns all registered buffs to all heroes. Might want to create a learning mechanism.
                state.KnownBuffs.Add(item.Key); 
            }
            return state;
        }

        private Buff GetBuffByID(string id)
        {
            if (this._registeredbuffs.ContainsKey(id))
            {
                return this._registeredbuffs[id];
            }
            else return null;
        }

        internal void OnHourlyTick()
        {
            foreach(var hero in this._dictionary.Keys)
            {
                foreach(var id in this._dictionary[hero].ActiveBuffs.Keys)
                {
                    var buff = this.GetBuffByID(id);
                    if(buff != null && buff.BuffCostType == BuffCostType.Continous)
                    {
                        var resource = ResourceManager.Instance.GetResourceForHero(hero, buff.CostResourceType);
                        if(resource != null)
                        {
                            if (resource.CurrentAmount < buff.CostAmount)
                            {
                                this._dictionary[hero].ActiveBuffs.Remove(id);
                                this.OnActiveBuffRemoved?.Invoke(this, new ActiveBuffsChangedEventArgs { hero = hero, newState = this._dictionary[hero], removedBuffID = id });
                                return;
                            }
                        }
                    }
                    this._dictionary[hero].ActiveBuffs[id] -= 1f;
                    if(this._dictionary[hero].ActiveBuffs[id] <= 0)
                    {
                        this._dictionary[hero].ActiveBuffs.Remove(id);
                        this.OnActiveBuffRemoved?.Invoke(this, new ActiveBuffsChangedEventArgs { hero = hero, newState = this._dictionary[hero], removedBuffID = id });
                    }
                }
            }
        }

        private bool IsHeroValidForBuffs(Hero hero)
        {
            return hero.IsAlive && hero.IsActive && hero.CharacterObject.Occupation == Occupation.Lord;
        }

        private bool DoesHeroKnowBuff(Hero hero, string buffid)
        {
            if (this._dictionary.ContainsKey(hero))
            {
                return this._dictionary[hero].KnownBuffs.Contains(buffid);
            }
            else return false;
        }

        public void AddBuffForHeroAndDuration(Hero hero, string buffid, float duration)
        {
            if(hero != null && this._dictionary.ContainsKey(hero))
            {
                if (DoesHeroKnowBuff(hero, buffid))
                {
                    this._dictionary[hero].ActiveBuffs.Add(buffid, duration);
                }
            }
        }

        public bool IsBuffCurrentlyActiveForHero(Hero hero, string buffid)
        {
            if (this._dictionary.ContainsKey(hero))
            {
                return this._dictionary[hero].ActiveBuffs.ContainsKey(buffid);
            }
            else return false;
        }
    }
}
