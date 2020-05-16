using System;
using TaleWorlds.SaveSystem;

namespace BannerBuff.TypeDefinitions
{
    /// <summary>
    /// Describes the state of a resource per hero.
    /// </summary>
    public class ResourceState
    {
        public int MinAmount { get; set; }
        public int CurrentAmount { get; set; }
        public int MaxAmount { get; set; }
        public int DefaultRechargeRatePerHour { get; set; }
        public int CurrentRechargeRatePerHour { get; set; }
    }
}
