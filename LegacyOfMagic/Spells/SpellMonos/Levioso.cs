using LegacyOfMagic.Management;
using LegacyOfMagic.Spells.SpellMonos.Updaters;
using ThunderRoad;
using UnityEngine;
namespace LegacyOfMagic.Spells.SpellMonos
{
    public class Levioso : Spell
    {
        private GameObject leviosoUpdate;
        private Rigidbody currentCreature;
        private GameObject go;
        private Vector3 position;
        public void Start()
        {
            spell = GetComponent<Item>();
            go = new GameObject();
            leviosoUpdate = new GameObject();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.GetComponentInParent<Creature>() is Creature creature)
            {
                creature.ragdoll.SetState(Ragdoll.State.Destabilized);
                currentCreature = creature.ragdoll.targetPart.physicBody.rigidBody;
                position = creature.ragdoll.targetPart.transform.position;
            }
            else if(collision.gameObject.GetComponentInParent<Item>() is Item item)
            {
                currentCreature = item.physicBody.rigidBody;
                position = item.transform.position;
            }
            go.AddComponent<LeviosoUpdate>().Setup(currentCreature,position);

            SpawnSparks(ModEntry.local.leviosoSparks, collision.contacts[0].point);
            spell.Despawn();
        }
    }
}