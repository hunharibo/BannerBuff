﻿using BannerBuff.TypeDefinitions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace BannerBuff.Behaviours
{
    class BuffManager : CampaignBehaviorBase
    {
        private Dictionary<Hero, HeroBuffState> _dictionary = new Dictionary<Hero, HeroBuffState>();
        private readonly Dictionary<string, Buff> _registeredbuffs = new Dictionary<string, Buff>();
        private static BuffManager _instance = null;
        public event EventHandler<ActiveBuffsChangedEventArgs> OnActiveBuffRemoved;
        private ResourceDepot _depot = null;
        public override void RegisterEvents()
        {
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, onHourlyTick);
            CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, onGameLoaded);
            CampaignEvents.OnNewGameCreatedEvent2.AddNonSerializedListener(this, onNewGame);
        }

        public static BuffManager Instance
        {
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
            this._depot = new ResourceDepot(BasePath.Name);
            this._depot.AddLocation("Modules/BannerBuff/ModuleData/");
            this._depot.CollectResources();
            var files = this._depot.GetFiles("", ".xml");
            if (files.Length > 0)
            {
                foreach (var path in files)
                {
                    if (Path.GetFileNameWithoutExtension(path) == "BannerBuffs")
                    {
                        XmlSerializer ser = new XmlSerializer(typeof(List<Buff>));
                        var list = ser.Deserialize(File.OpenRead(path)) as List<Buff>;
                        if (list != null)
                        {
                            foreach (var item in list)
                            {
                                this._registeredbuffs.Add(item.stringID, item);
                            }
                        }
                    }
                }
            }
        }

        private void onNewGame() => this.InitializeBlankState();
        private void onGameLoaded(CampaignGameStarter obj) => this.InitializeBlankState();

        private void onHourlyTick()
        {
            foreach (var hero in this._dictionary.Keys)
            {
                foreach (var buff in this._dictionary[hero].ActiveBuffs.Keys)
                {
                    if (buff != null && buff.BuffCostType == BuffCostType.Continous)
                    {
                        var resource = ResourceManager.Instance.GetResourceForHero(hero);
                        if (resource != null)
                        {
                            if (resource.CurrentAmount < buff.CostAmount)
                            {
                                this._dictionary[hero].ActiveBuffs.Remove(buff);
                                this.OnActiveBuffRemoved?.Invoke(this, new ActiveBuffsChangedEventArgs { hero = hero, newState = this._dictionary[hero], removedBuffID = buff.stringID });
                                return;
                            }
                        }
                    }
                    this._dictionary[hero].ActiveBuffs[buff] -= 1f;
                    if (this._dictionary[hero].ActiveBuffs[buff] <= 0)
                    {
                        this._dictionary[hero].ActiveBuffs.Remove(buff);
                        this.OnActiveBuffRemoved?.Invoke(this, new ActiveBuffsChangedEventArgs { hero = hero, newState = this._dictionary[hero], removedBuffID = buff.stringID });
                    }
                }
            }
        }

        private void InitializeBlankState()
        {
            foreach (var hero in Hero.All)
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
            foreach (var item in this._registeredbuffs)
            {//TODO might want to alter this behaviour. This auto-assigns all registered buffs to all heroes. Might want to create a learning mechanism.
                state.KnownBuffs.Add(item.Value);
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

        private bool IsHeroValidForBuffs(Hero hero)
        {
            return hero.IsAlive && hero.IsActive && hero.CharacterObject.Occupation == Occupation.Lord;
        }

        private bool DoesHeroKnowBuff(Hero hero, string buffid)
        {
            if (this._dictionary.ContainsKey(hero))
            {
                return this._dictionary[hero].KnownBuffs.Contains(GetBuffByID(buffid));
            }
            else return false;
        }

        public void AddBuffForHeroAndDuration(Hero hero, string buffid, float duration)
        {
            if (hero != null && this._dictionary.ContainsKey(hero))
            {
                if (DoesHeroKnowBuff(hero, buffid))
                {
                    //TODO should check if hero has enough resources for "cast" of a buff
                    this._dictionary[hero].ActiveBuffs.Add(GetBuffByID(buffid), duration);
                }
            }
        }

        public bool IsBuffCurrentlyActiveForHero(Hero hero, string buffid)
        {
            if (this._dictionary.ContainsKey(hero))
            {
                return this._dictionary[hero].ActiveBuffs.ContainsKey(GetBuffByID(buffid));
            }
            else return false;
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("BannerBuffSaveData_Buffs", ref _dictionary);
        }

        public class SaveableTypes : SaveableTypeDefiner
        {
            public SaveableTypes() : base(770002) { }

            protected override void DefineClassTypes()
            {
                AddClassDefinition(typeof(Buff), 1);
                AddClassDefinition(typeof(HeroBuffState), 2);
                AddEnumDefinition(typeof(BuffCostType), 3);
            }

            protected override void DefineContainerDefinitions()
            {
                ConstructContainerDefinition(typeof(List<Buff>));
                ConstructContainerDefinition(typeof(Dictionary<Buff, float>));
                ConstructContainerDefinition(typeof(Dictionary<Hero, HeroBuffState>));
            }
        }
    }
}