using System;
using LegacyOfMagic.Management;
using ThunderRoad;
using UnityEngine;

namespace LegacyOfMagic.Spells.SpellMonos
{
    public class AvadaKedavra : Spell
    {
        private void Start()
        {
            spell = GetComponent<Item>();
        }
        
        public void OnCollisionEnter(Collision c)
        {
            if (c.gameObject.GetComponentInParent<Creature>() is Creature creature)
            {
                creature.Kill();
            }

            if(ProjectileProperties.local.emitSparks) SpawnSparks(ModEntry.local.avadaSparks, c.contacts[0].point);
            spell.Despawn();
        }
    }
}