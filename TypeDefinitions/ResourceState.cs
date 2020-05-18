using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace BannerBuff.TypeDefinitions
{
    /// <summary>
    /// Describes the state of a resource per hero.
    /// </summary>
    [SaveableClass(7770011)]
    public class ResourceState
    {
        public const string ResourceName = "Magicka";
        [SaveableProperty(0)]
        public int MinAmount { get; set; } = 0;
        [SaveableProperty(1)]
        public int CurrentAmount { get; set; } = 0;
        [SaveableProperty(2)]
        public int MaxAmount { get; set; } = 100;
        [SaveableProperty(3)]
        public int DefaultRechargeRatePerHour { get; set; } = 1;
        [SaveableProperty(4)]
        public int CurrentRechargeRatePerHour { get; set; } = 1;
    }

    public class ResourceChangedEventArgs : EventArgs
    {
        public Hero hero { get; set; }
        public int changeAmount { get; set; }
        public int currentAmount { get; set; }
    }
}
