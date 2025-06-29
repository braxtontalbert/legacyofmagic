using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LegacyOfMagic.Management;
using LegacyOfMagic.Spells.SpellMonos.Updaters;
using ThunderRoad;
using UnityEngine;
using UnityEngine.VFX;

namespace LegacyOfMagic.Spells.SpellMonos
{
    public class Imperio : Spell
    {
        private Creature lastCreature;
        public override void Start()
        {
            base.Start();
            usedWand = GetComponent<Item>();
            Cast();
        }

        public override void Cast()
        {
            lastCreature = GetCreaturesWithinAngleAndDistance(usedWand.flyDirRef.transform.position,
                usedWand.flyDirRef.forward, 45f, 60f);

            if (lastCreature != null) GameManager.local.StartCoroutine(ExecuteImperio(lastCreature));
        }

        
        IEnumerator ExecuteImperio(Creature creature)
        {
            followTransform = creature.ragdoll.headPart.transform;
            lastCreature = creature;
            GameManager.local.StartCoroutine(SetupCast(creature.ragdoll.headPart.transform.position, ModEntry.imperioEffect));
            yield return null;
        }
        public override void ExecuteAfterInstantiate()
        {
            GameManager.local.StartCoroutine(CastSpellEffect(activeCast, lastCreature));
        }
        public override void ExecuteIfCached()
        {
            GameManager.local.StartCoroutine(CastSpellEffect(activeCast, lastCreature));
        }
        private IEnumerator CastSpellEffect(VisualEffect vfx, Creature creature)
        {
            yield return base.CastSpellEffect(vfx);
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