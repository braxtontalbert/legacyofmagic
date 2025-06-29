using System;
using System.Collections;
using System.Collections.Generic;
using LegacyOfMagic.Management;
using ThunderRoad;
using ThunderRoad.AI.Decorator;
using UnityEngine;

namespace LegacyOfMagic.Spells.VFX
{
    public class VFXPool : ThunderScript
    {
        public static VFXPool local;
        
        //Pool
        public Queue<GameObject> wingardiumVfxPool = new Queue<GameObject>();
        public Queue<GameObject> crucioVfxPool = new Queue<GameObject>();
        public Queue<GameObject> engorgioVfxPool = new Queue<GameObject>();
        public Queue<GameObject> imperioVfxPool = new Queue<GameObject>();
        public Queue<GameObject> evanescoVfxPool = new Queue<GameObject>();
        public Queue<GameObject> depulsoVfxPool = new Queue<GameObject>();
        public Queue<GameObject> geminioVfxPool = new Queue<GameObject>();
        public Queue<GameObject> flagratePointVfxPool = new Queue<GameObject>();
        public Queue<GameObject> flagrateWritingVfxPool = new Queue<GameObject>();
        
        public override void ScriptLoaded(ModManager.ModData modData)
        {
            if (local == null) local = this;
            base.ScriptLoaded(modData);
        }

        public Queue<GameObject> GetPoolByReference(String reference)
        {
            switch (reference)
            {
                case ModEntry.wingardiumCastEffect:
                    return wingardiumVfxPool;
                case ModEntry.imperioEffect:
                    return imperioVfxPool;
                case ModEntry.engorgioCastEffect:
                    return engorgioVfxPool;
                case ModEntry.evanescoCastEffect:
                    return evanescoVfxPool;
                case ModEntry.crucioCastEffect:
                    return crucioVfxPool;
                case ModEntry.geminioCastEffect:
                    return geminioVfxPool;
                case ModEntry.flagratePointEffect:
                    return flagratePointVfxPool;
                case ModEntry.flagrateWritingEffect:
                    return flagrateWritingVfxPool;
                default:
                    return null;
            }
        }
        public void ClearVfxPools()
        {
            foreach (var vfx in wingardiumVfxPool)
            {
                Catalog.ReleaseAsset(vfx);
            }
            wingardiumVfxPool.Clear();
            wingardiumVfxPool.TrimExcess();
            
            
            foreach (var vfx in engorgioVfxPool)
            {
                Catalog.ReleaseAsset(vfx);
            }
            engorgioVfxPool.Clear();
            engorgioVfxPool.TrimExcess();
            
            foreach (var vfx in imperioVfxPool)
            {
                Catalog.ReleaseAsset(vfx);
            }
            imperioVfxPool.Clear();
            imperioVfxPool.TrimExcess();
            
            foreach (var vfx in evanescoVfxPool)
            {
                Catalog.ReleaseAsset(vfx);
            }
            evanescoVfxPool.Clear();
            evanescoVfxPool.TrimExcess();
            
            foreach (var vfx in depulsoVfxPool)
            {
                Catalog.ReleaseAsset(vfx);
            }
            depulsoVfxPool.Clear();
            depulsoVfxPool.TrimExcess();
            
            foreach (var vfx in crucioVfxPool)
            {
                Catalog.ReleaseAsset(vfx);
            }
            crucioVfxPool.Clear();
            crucioVfxPool.TrimExcess();
            
            foreach (var vfx in geminioVfxPool)
            {
                Catalog.ReleaseAsset(vfx);
            }
            geminioVfxPool.Clear();
            geminioVfxPool.TrimExcess();
            
            foreach (var vfx in flagratePointVfxPool)
            {
                Catalog.ReleaseAsset(vfx);
            }
            flagratePointVfxPool.Clear();
            flagratePointVfxPool.TrimExcess();
            
            foreach (var vfx in flagrateWritingVfxPool)
            {
                Catalog.ReleaseAsset(vfx);
            }
            flagrateWritingVfxPool.Clear();
            flagrateWritingVfxPool.TrimExcess();
        }
    }
}