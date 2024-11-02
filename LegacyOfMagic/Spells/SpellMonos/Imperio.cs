using LegacyOfMagic.Management;
using LegacyOfMagic.Spells.SpellMonos.Updaters;
using ThunderRoad;
using UnityEngine;

namespace LegacyOfMagic.Spells.SpellMonos
{
    public class Imperio : Spell
    {
        private GameObject go;
        private GameObject visible;
        
        public void Start()
        {
            usedWand = GetComponent<Item>();
            go = Instantiate(ModEntry.local.imperioEffect);
            visible = Instantiate(ModEntry.local.imperioShown);
            go.GetComponent<ParticleSystem>().Play();
            visible.GetComponent<ParticleSystem>().Play();
            go.gameObject.AddComponent<SpellParticles>();

        }

        void Update()
        {
            if (go && visible)
            {
                var transform1 = usedWand.flyDirRef.transform;
                var rotation = transform1.rotation;
                go.transform.rotation = rotation;
                var position = transform1.position;
                go.transform.position = position;
                visible.transform.rotation = rotation;
                visible.transform.position = position;
            }
        }
    }
}