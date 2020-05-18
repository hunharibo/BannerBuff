using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace BannerBuff.TypeDefinitions
{
    [SaveableClass(7770012)]
    public class Buff : IEquatable<Buff>
    {
        [SaveableProperty(0)]
        public int CostAmount { get; set; }
        [SaveableProperty(1)]
        public string Name { get; set; }
        [SaveableProperty(2)]
        public string Description { get; set; }
        [SaveableProperty(3)]
        public string IconPath { get; set; }
        [SaveableProperty(4)]
        public string stringID { get; set; }
        [SaveableProperty(5)]
        public BuffCostType BuffCostType { get; set; }

        public bool Equals(Buff other) => this.stringID == other.stringID;

    }

    public class ActiveBuffsChangedEventArgs : EventArgs
    {
        public Hero hero { get; set; }
        public HeroBuffState newState { get; set; }
        public string removedBuffID { get; set; }
    }
}
