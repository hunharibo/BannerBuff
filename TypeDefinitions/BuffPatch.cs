using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BannerBuff.TypeDefinitions
{
    public abstract class BuffPatch
    {
        public string BuffStringID { get; }
        public virtual void DoPatch(Harmony harmony_instance)
        {

        }

        public abstract void PostFix(ref object __result);
    }
}
