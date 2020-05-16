using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.ObjectSystem;

namespace BannerBuff.TypeDefinitions
{
    /// <summary>
    /// Describes the state of a hero's buffs.
    /// </summary>
    public class HeroBuffState
    {
        /// <summary>
        /// Provides a list of stringIDs that represent known buffs.
        /// </summary>
        public List<string> KnownBuffs { get; set; }
        /// <summary>
        /// Provides a dictionary of currently active buff stringIDs with their remaining duration left in hours.
        /// </summary>
        public Dictionary<string,float> ActiveBuffs { get; set; }

        public HeroBuffState()
        {
            this.KnownBuffs = new List<string>();
            this.ActiveBuffs = new Dictionary<string, float>();
            
        }
    }
}
