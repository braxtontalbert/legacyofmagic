using System.Collections;
using LegacyOfMagic.Management;
using ThunderRoad;
using UnityEngine;
namespace LegacyOfMagic.Spells.SpellMonos
{
    public class PetrificusTotalus : Spell
    {
        private GameObject cachedFreezeSFX;
        public void Start()
        {
            spell = GetComponent<Item>();
        }

        public void OnCollisionEnter(Collision c)
        {
            if (c.gameObject.GetComponentInParent<RagdollPart>() is RagdollPart part 
                && part.gameObject.GetComponentInParent<Creature>() is Creature creature
                && !part.isSliced) {
               GameManager.local.StartCoroutine(Freeze(creature, c));
               if (!cachedFreezeSFX)
               {
                   cachedFreezeSFX = Instantiate(ModEntry.local.freezeSFX);
               }
               else
               {
                   cachedFreezeSFX.gameObject.GetComponent<AudioSource>().Play();
               }
               cachedFreezeSFX.transform.position = creature.ragdoll.targetPart.transform.position;
            }
            SpawnSparks(ModEntry.local.petrificusSparks, c.contacts[0].point);
            spell.Despawn();
        }
        
        public IEnumerator Freeze(Creature creature, Collision c)
        {
            bool started = true;
            if (creature)
            {
                creature.ragdoll.SetState(Ragdoll.State.Frozen);
                creature.brain.instance.Stop();
                while (started)
                {
                    if (creature.isKilled)
                    {
                        break;
                    }

                    bool canStart = true;
                    int returnVal = Random.Range(1, 101);
                    if (returnVal % 5 == 0)
                    {
                        canStart = false;
                    }

                    yield return new WaitForSeconds(5f);
                    started = canStart;
                }
            }
            if(!creature.isKilled) creature.ragdoll.SetState(Ragdoll.State.Destabilized);
            if(!creature.isKilled) creature.brain.instance.Start();
            
        }
    }
}