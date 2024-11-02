using System.Collections;
using System.Collections.Generic;
using LegacyOfMagic.Management;
using UnityEngine;
using ThunderRoad;
namespace LegacyOfMagic.Spells.SpellMonos.Updaters
{
    public class SpellParticles : MonoBehaviour
    {
        private Creature creature;
        private List<ParticleCollisionEvent> collisionEvents;
        private ParticleSystem ps;
        void Start()
        {
            ps = GetComponent<ParticleSystem>();
        }

        void OnParticleCollision(GameObject other)
        {
            if (creature) return;
            creature = other.GetComponentInParent<Creature>();
            if (!creature.GetComponent<OnCreature>())
            {
                creature.gameObject.AddComponent<OnCreature>();
                StartImperio(creature);
            }
        }
        void StartImperio(Creature creature)
        {
            if (creature)
            {
                GameManager.local.StartCoroutine(ImperioCounterCurse(creature, creature.factionId));

                creature.SetFaction(2);
                creature.brain.Load(creature.brain.instance.id);
            }
        }
        
        public IEnumerator ImperioCounterCurse(Creature creature, int factionOriginal)
        {
            bool started = true;
            
            while (started)
            {
                if (creature)
                {
                    if (creature.isKilled)
                    {
                        break;
                    }
                }

                bool canStart = true;
                int returnVal = UnityEngine.Random.Range(1, 101);
                if (returnVal == 67)
                {
                    canStart = false;
                }
                else canStart = true;
               

                yield return new WaitForSeconds(5f);
                started = canStart;
            }
            creature.SetFaction(factionOriginal);
            if(!creature.isKilled) creature.brain.Load(creature.brain.instance.id);
        }
    }
}