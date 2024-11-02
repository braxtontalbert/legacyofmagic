using LegacyOfMagic.Management;
using ThunderRoad;
using UnityEngine;

namespace LegacyOfMagic.Spells.SpellMonos
{
    public class EverteStatum : Spell
    {
        
        public void Start()
        {
            spell = GetComponent<Item>();
        }

        public void OnCollisionEnter(Collision c)
        {

            if (c.gameObject.GetComponentInParent<Creature>() is Creature creature)
            {
                creature.ragdoll.SetState(Ragdoll.State.Destabilized);
                foreach (var part in creature.ragdoll.parts)
                {
                    if (!part.isSliced)
                    {
                        var rigidbody = part.physicBody.rigidBody;
                        var direction = (c.contacts[0].point - usedWand.flyDirRef.transform.position).normalized;
                        rigidbody.AddForce(direction * (100f), ForceMode.Impulse);
                        rigidbody.AddForce(Vector3.up * (100f / 1.5f), ForceMode.Impulse);
                    }
                }
                
            }
            SpawnSparks(ModEntry.local.evertestatumSparks, c.contacts[0].point);
            
            spell.Despawn();
        }
    }
}