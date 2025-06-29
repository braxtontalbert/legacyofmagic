using System;
using System.Collections.Generic;
using System.Linq;
using LegacyOfMagic.Management;
using UnityEngine.VFX;

namespace LegacyOfMagic.Spells.SpellMonos
{
    using UnityEngine;
    using ThunderRoad;
    public class Confringo : Spell
    {
        private GameObject cachedExplosionEffect;

        private void Awake()
        {
            spell = GetComponent<Item>();
        }

        public void OnCollisionEnter(Collision c)
        {
            Collider[] colliders = Physics.OverlapSphere(c.contacts[0].point, 3f);
            List<Creature> creaturesInColliders = new List<Creature>();
            List<Item> itemsInColliders = new List<Item>();
            try
            {
                foreach (Collider collider in colliders)
                {
                    if (collider.GetComponentInParent<Creature>() is Creature creature && !creature.isPlayer &&
                        !creaturesInColliders.Contains(creature))
                    {
                        creaturesInColliders.Add(creature);
                    }

                    if (collider.GetComponentInParent<Item>() is Item item && !itemsInColliders.Contains(item))
                    {
                        itemsInColliders.Add(item);
                    }
                }

                foreach (Creature creature in creaturesInColliders)
                {
                    creature.ragdoll.SetState(Ragdoll.State.Destabilized);
                    foreach (Rigidbody body in creature.ragdoll.parts.Select(part => part.physicBody.rigidBody))
                    {
                        float mass = body.mass;
                        body.AddExplosionForce(mass * 2000f, c.contacts[0].point, 3f, 30f);
                    }

                }

                foreach (Item item in itemsInColliders)
                {

                    float mass = item.physicBody.rigidBody.mass;
                    item.physicBody.rigidBody.AddExplosionForce(mass * 2000f, c.contacts[0].point, 3f, 30f);

                }

                if (!cachedExplosionEffect)
                {
                    ModEntry.local.explosion.transform.position = c.contacts[0].point;
                    cachedExplosionEffect = Instantiate(ModEntry.local.explosion);
                }
                else
                {
                    cachedExplosionEffect.transform.position = c.contacts[0].point;
                    cachedExplosionEffect.GetComponentInChildren<VisualEffect>().Play();
                }
                ModEntry.local.explosion.transform.position = c.contacts[0].point;
                GameObject effect = Instantiate(ModEntry.local.explosion);
                spell.Despawn();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                spell.Despawn();
            }
            
        }
    }
}