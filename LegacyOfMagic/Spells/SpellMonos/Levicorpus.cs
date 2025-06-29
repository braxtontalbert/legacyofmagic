using System;
using LegacyOfMagic.Management;
using ThunderRoad;
using UnityEngine;
namespace LegacyOfMagic.Spells.SpellMonos
{
    public class Levicorpus : Spell
    {
        GameObject floater1;
        GameObject floater2;
        SpringJoint joint;
        SpringJoint joint2;
        private Creature despawnCreature;
        private void Awake()
        {
            spell = GetComponent<Item>();
        }

        public void OnCollisionEnter(Collision c)
        {
            if (c.gameObject.GetComponentInParent<Creature>() is Creature creature)
            {

                despawnCreature = creature;
                floater1 = new GameObject();
                floater1.AddComponent<Rigidbody>();
                floater1.GetComponent<Rigidbody>().useGravity = false;

                
                floater2 = new GameObject();
                floater2.AddComponent<Rigidbody>();
                floater2.GetComponent<Rigidbody>().useGravity = false;

                creature.ragdoll.SetState(Ragdoll.State.Destabilized);
                joint =  creature.footLeft.gameObject.AddComponent<SpringJoint>();
                joint2 = creature.footRight.gameObject.AddComponent<SpringJoint>();

                floater1.transform.position = new Vector3(creature.ragdoll.headPart.transform.position.x, creature.ragdoll.headPart.transform.position.y + 2f, creature.ragdoll.headPart.transform.position.z);
                floater2.transform.position = new Vector3(creature.ragdoll.headPart.transform.position.x, creature.ragdoll.headPart.transform.position.y + 2f, creature.ragdoll.headPart.transform.position.z);


                joint.connectedBody = floater1.GetComponent<Rigidbody>();
                joint.autoConfigureConnectedAnchor = false;
                joint.connectedAnchor = new Vector3(0,0,0);
                joint.spring = 3000f;
                joint.damper = 100f;


                joint2.connectedBody = floater2.GetComponent<Rigidbody>();
                joint2.autoConfigureConnectedAnchor = false;
                joint2.connectedAnchor = new Vector3(0, 0, 0);
                joint2.spring = 3000f;
                joint2.damper = 100f;


                floater1.AddComponent<FixedJoint>();
                floater2.AddComponent<FixedJoint>();


                ModEntry.local.floaters.Add(floater1);
                ModEntry.local.floaters.Add(floater2);
                ModEntry.local.levicorpusedCreatures.Add(creature);

                creature.OnDespawnEvent += Creature_OnDespawnEvent;
            }

            SpawnSparks(ModEntry.local.levicorpusSparks, c.contacts[0].point);
            spell.Despawn();
        }
        private void Creature_OnDespawnEvent(EventTime eventTime)
        {
            if (despawnCreature.footLeft.gameObject.GetComponent<SpringJoint>() != null && despawnCreature.footRight.gameObject.GetComponent<SpringJoint>() != null)
            {
                Destroy(despawnCreature.footLeft.gameObject.GetComponent<SpringJoint>());
                Destroy(despawnCreature.footRight.gameObject.GetComponent<SpringJoint>());
            }
        }
    }
}