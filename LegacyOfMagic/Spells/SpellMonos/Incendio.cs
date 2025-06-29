using System;
using LegacyOfMagic.Management;
using LegacyOfMagic.Spells.SpellMonos.Updaters;
using ThunderRoad;
using UnityEngine;

namespace LegacyOfMagic.Spells.SpellMonos
{
    public class Incendio : Spell
    {
        private GameObject incendioEffect;
        private GameObject hiddenEffect;
        private ParticleSystem ps;
        private ParticleSystem psHidden;
        public void Start()
        {
            usedWand = GetComponent<Item>();
            Cast();
        }

        public override void Cast()
        {
            if (!incendioEffect)
            {
                incendioEffect = Instantiate(ModEntry.local.incendioEffect);
                ps = incendioEffect.GetComponent<ParticleSystem>();
            }
            else
            {
                incendioEffect.GetComponent<AudioSource>().Play();
                ps.Play();
            }

            if (!hiddenEffect)
            {
                hiddenEffect = Instantiate(ModEntry.local.incendioHiddenEffect);
                psHidden = hiddenEffect.GetComponent<ParticleSystem>();
            }
            else
            {
                psHidden.Play();
            }
            var transform1 = usedWand.flyDirRef.transform;
            incendioEffect.transform.rotation = transform1.rotation;
            incendioEffect.transform.position = transform1.position;

            if (!hiddenEffect.GetComponent<IncendioParticle>())
            {
                var ip = hiddenEffect.AddComponent<IncendioParticle>();
                ip.spellCasted = true;
            }
        }

        private void Update()
        {
            if (ps.isPlaying)
            {
                var transform1 = usedWand.flyDirRef.transform;
                incendioEffect.transform.rotation = transform1.rotation;
                incendioEffect.transform.position = transform1.position;
            }

            if (psHidden.isPlaying)
            {
                var transform1 = usedWand.flyDirRef.transform;
                hiddenEffect.transform.rotation = transform1.rotation;
                hiddenEffect.transform.position = transform1.position;
            }

            if (psHidden.isEmitting || ps.isEmitting)
            {
                usedWand.Haptic(1f);
            }
        }
    }
}