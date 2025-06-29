using System;
using System.Collections.Generic;
using System.Linq;
using ThunderRoad;
using UnityEngine;

namespace LegacyOfMagic
{
    public class CommonFunctions : ThunderScript
    {
        public static CommonFunctions local;
        public override void ScriptLoaded(ModManager.ModData modData)
        {
            if (local == null)
            {
                local = this;
            }
            base.ScriptLoaded(modData);
        }
    }
}