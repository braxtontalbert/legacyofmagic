using System;
using LegacyOfMagic.Management;
using ThunderRoad;
using UnityEngine;

namespace LegacyOfMagic.Spells.SpellMonos
{
    public class Sectumsempra : Spell
    {

        public void Start()
        {
            spell = GetComponent<Item>();
        }
        private void OnCollisionEnter(Collision other)
        {
            
            if(other.gameObject.GetComponentInParent<RagdollPart>() is RagdollPart part)
            {
                Creature creature = part.GetComponentInParent<Creature>();
                if (part != creature.ragdoll.rootPart)
                {
                    bool sliced = part.TrySlice();
                    if(part.data.sliceForceKill) creature.Kill();
                    if (!sliced)
                    {
                        foreach (var rp in creature.ragdoll.parts)
                        {
                            if (rp != part && rp != creature.ragdoll.rootPart)
                            {
                                rp.TrySlice();
                            }
                        }
                        
                        creature.Kill();
                    }
                }
                else
                {
                    foreach (var rp in creature.ragdoll.parts)
                    {
                        if (rp != creature.ragdoll.rootPart)
                        {
                            rp.TrySlice();
                        }
                    }
                }
            }

            SpawnSparks(ModEntry.local.sectumsempraSparks, other.contacts[0].point);
            spell.Despawn();
        }
    }
}