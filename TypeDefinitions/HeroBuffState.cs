using System.Collections.Generic;
using TaleWorlds.SaveSystem;

namespace BannerBuff.TypeDefinitions
{
    /// <summary>
    /// Describes the state of a hero's buffs.
    /// </summary>
    [SaveableClass(77700113)]
    public class HeroBuffState
    {
        /// <summary>
        /// Provides a list of stringIDs that represent known buffs.
        /// </summary>
        [SaveableProperty(0)]
        public List<Buff> KnownBuffs { get; set; } = new List<Buff>();
        /// <summary>
        /// Provides a dictionary of currently active buff stringIDs with their remaining duration left in hours.
        /// </summary>
        [SaveableProperty(1)]
        public Dictionary<Buff, float> ActiveBuffs { get; set; } = new Dictionary<Buff, float>();
    }
}
