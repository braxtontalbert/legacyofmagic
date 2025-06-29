using LegacyOfMagic.Management;
using ThunderRoad;
using UnityEngine;

namespace LegacyOfMagic.Spells.SpellMonos
{
    public class Flipendo : Spell
    {
        void Start() { 
        
            spell = GetComponent<Item>();

        }

        public void OnCollisionEnter(Collision c)
        {
            if (c.gameObject.GetComponentInParent<Creature>() is Creature creature)
            {

                creature.ragdoll.SetState(Ragdoll.State.Destabilized);
                foreach (var parts in c.gameObject.GetComponentInParent<Creature>().ragdoll.parts)
                {
                    var rigidbody = parts.physicBody.rigidBody;
                    rigidbody.AddForce(Vector3.up * 150f, ForceMode.Impulse);
                    if (rigidbody.name.Contains("Head"))
                    {
                        rigidbody.AddForce(spell.flyDirRef.transform.forward * 40f, ForceMode.Impulse);   
                    }
                }
            }
            SpawnSparks(ModEntry.local.flipendoSparks, c.contacts[0].point);
            spell.Despawn();
        }
    }
}