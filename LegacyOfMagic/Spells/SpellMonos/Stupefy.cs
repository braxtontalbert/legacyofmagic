using LegacyOfMagic.Management;
using ThunderRoad;
using UnityEngine;

namespace LegacyOfMagic.Spells.SpellMonos
{
    public class Stupefy : Spell
    {

        private void Start()
        {
            spell = GetComponent<Item>();
        }

        private void SpellEffect(Creature creature)
        {
            creature.ragdoll.SetState(Ragdoll.State.Destabilized);
            creature.TryElectrocute(1, 5, true, false);
        }
        public void OnCollisionEnter(Collision c)
        {
            if (c.gameObject.GetComponentInParent<RagdollPart>() is RagdollPart part &&
                part.GetComponentInParent<Creature>() is Creature creature && !part.isSliced && !creature.isKilled)
            {
                SpellEffect(creature);
            }
            if(ProjectileProperties.local.emitSparks) SpawnSparks(ModEntry.local.stupefySparks, c.contacts[0].point);
            spell.Despawn();
        }
    }
}