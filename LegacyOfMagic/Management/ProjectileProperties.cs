using System;
using System.Collections.Generic;
using ThunderRoad;

namespace LegacyOfMagic.Management
{
    public class ProjectileProperties : CustomData
    {
        public static ProjectileProperties local;
        public  Dictionary<String, String> spellTypes;
        public  float spellSpeed;
        public  bool useGravity;
        public  float drag;
        public  bool emitSparks;

        public override void OnCatalogRefresh()
        {
            if (local != null) return;
            local = this;
        }
    }
}