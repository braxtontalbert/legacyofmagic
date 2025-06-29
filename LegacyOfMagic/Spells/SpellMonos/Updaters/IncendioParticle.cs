using System;
using System.Collections.Generic;
using System.Linq;
using ThunderRoad;
using UnityEngine;

namespace LegacyOfMagic.Spells.SpellMonos.Updaters
{
    public class IncendioParticle : MonoBehaviour
    {
        Dictionary<Creature, float> creaturesToBurn = new Dictionary<Creature, float>();
        private ParticleSystem ps;
        public bool spellCasted = false;
        private Item wand;
        
        
        public void Start()
        {
            ps = GetComponent<ParticleSystem>();
        }

        private void OnParticleCollision(GameObject other)
        {
            if (!spellCasted)
            {
                creaturesToBurn.Clear();
            }
            if (other.gameObject.GetComponentInParent<Creature>() is Creature creature)
            {
                if (!creaturesToBurn.ContainsKey(creature)) creaturesToBurn.Add(creature, 0 + 0.1f);
                else creaturesToBurn[creature] += 0.1f;
            }
        }

        private void Update()
        {
            if (creaturesToBurn.Count > 0)
            {
                var keys = creaturesToBurn.Keys.ToList();
                for (int i = 0; i < keys.Count; i++)
                {
                    Creature creature = keys[i];
                    float value = creaturesToBurn[creature];
                    if (value > 0f)
                    {
                        creature.Inflict("Burning", this, parameter:70f * 0.3f);
                        creaturesToBurn[creature] -= 0.1f;
                    }
                    else
                    {
                        creaturesToBurn.Remove(creature);
                    }
                }
            }
        }
    }
}