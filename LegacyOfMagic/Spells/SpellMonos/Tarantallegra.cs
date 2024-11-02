using LegacyOfMagic.Management;
using ThunderRoad;
using UnityEngine;
namespace LegacyOfMagic.Spells.SpellMonos
{
    public class Tarantallegra : Spell
    {
        private AnimationData animationData;
        System.Random random = new System.Random();
        void Start()
        {

            spell = GetComponent<Item>();
            animationData = Catalog.GetData<AnimationData>("HPSDances");
        
        }

        void OnCollisionEnter(Collision c) {


            if (c.gameObject.GetComponentInParent<RagdollPart>() is RagdollPart part && part.gameObject.GetComponentInParent<Creature>() is Creature creature) {

                if (!part.isSliced && !creature.isKilled)
                {
                    int index = random.Next(0, animationData.animationClips.Count - 1);

                    creature.PlayAnimation(animationData.animationClips[index].animationClip, false);
                }
            }

            SpawnSparks(ModEntry.local.tarantallegraSparks, c.contacts[0].point);
            spell.Despawn();
        }
    }
}