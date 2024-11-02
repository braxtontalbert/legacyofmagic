using System;
using LegacyOfMagic.Management;
using ThunderRoad;
using UnityEngine;

namespace LegacyOfMagic.Spells.SpellMonos
{
    public class Expelliarmus : Spell
    {
        public void Start()
        {
            spell = GetComponent<Item>();
        }
        
        public void OnCollisionEnter(Collision c)
        {
            if(ProjectileProperties.local.emitSparks) SpawnSparks(ModEntry.local.expelliarmusSparks, c.contacts[0].point);
            if (c.gameObject.GetComponentInParent<Creature>() is Creature creature)
            {
                Item item = creature.handRight.grabbedHandle?.item; 
                if(creature.handRight.grabbedHandle) creature.handRight.UnGrab(false);
                if(creature.handLeft.grabbedHandle) creature.handLeft.UnGrab(false);
                if (item)
                {
                    try
                    {
                        RagdollHand oppositeHand = ModEntry.local.currentlyHeldWands[0].mainHandler.otherHand;
                        var position = oppositeHand.transform.position;
                        var position1 = item.transform.position;
                        Vector3 direction = position - position1;
                        item.physicBody.rigidBody.AddForce(
                            direction.normalized * (item.physicBody.mass * 1.35f) *
                            Math.Min(Vector3.Distance(position, position1), 15f), ForceMode.Impulse);
                        item.physicBody.rigidBody.AddForce(
                            Vector3.up * (item.physicBody.mass * 1.35f) *
                            Math.Min(Vector3.Distance(position, position1), 15f), ForceMode.Impulse);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                        spell.Despawn();
                    }
                }
                spell.Despawn();
            }

            else if (c.gameObject.GetComponentInParent<Item>() is Item itemIn)
            {
                itemIn.physicBody.velocity = new Vector3(0,0,0);
                itemIn.mainHandler.playerHand.ragdollHand.UnGrab(false);

                if (itemIn)
                {
                    RagdollHand oppositeHand = ModEntry.local.currentlyHeldWands[0].mainHandler.otherHand;
                    var position = oppositeHand.transform.position;
                    var position1 = itemIn.transform.position;
                    Vector3 direction =  position - position1;
                    itemIn.physicBody.rigidBody.AddForce(direction.normalized * (itemIn.physicBody.mass * 1.35f) * Math.Min(Vector3.Distance(position,position1), 15f), ForceMode.Impulse);
                    itemIn.physicBody.rigidBody.AddForce(Vector3.up * (itemIn.physicBody.mass * 1.35f) * Math.Min(Vector3.Distance(position,position1),15f), ForceMode.Impulse);
                }
                spell.Despawn();
            }
            else spell.Despawn();
            
        }
    }
}