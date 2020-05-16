using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BannerBuff.TypeDefinitions
{
    public class Buff
    {
        public int CostAmount { get; set; }
        public ResourceType CostResourceType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconPath { get; set; }
        public string stringID { get; set; }
        public BuffCostType BuffCostType { get; set; }
    }
}
