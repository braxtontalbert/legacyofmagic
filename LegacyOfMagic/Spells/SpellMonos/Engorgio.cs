using System;
using System.Collections.Generic;
using System.Linq;
using LegacyOfMagic.Spells.SpellMonos.Updaters;
using UnityEngine;
using ThunderRoad;
namespace LegacyOfMagic.Spells.SpellMonos
{
    public class Engorgio : Spell
    {
        public void Start()
        {
            usedWand = GetComponent<Item>();
            Cast();
        }

        private void ExecuteEngorgio(Item item)
        {
            if (item.gameObject.GetComponent<SizeManager>() is SizeManager sm)
            {
                if (!sm.changeSize)
                {
                    sm.changeSize = true;
                    sm.direction = true;
                }
            }
            else
            {
                SizeManager local = item.gameObject.AddComponent<SizeManager>();
                local.direction = true;
                local.changeSize = true;
            }
        }
        public override void Cast()
        {
            RaycastHit hit;
            if (Physics.Raycast(usedWand.flyDirRef.transform.position, usedWand.flyDirRef.transform.forward, out hit,
                    20f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.gameObject.GetComponentInParent<Item>() is Item item)
                {
                    ExecuteEngorgio(item);
                    return;
                }
                Dictionary<Item, float> toCompare = new Dictionary<Item, float>();
                foreach (Item selected in Item.allActive)
                {
                    if(!Player.currentCreature.equipment.GetAllHolsteredItems().Contains(selected))
                    {
                        bool playerCheck = false;

                        if (selected.mainHandler is RagdollHand handler)
                        {
                            if (handler.creature is Creature creature && creature.isPlayer)
                            {
                                playerCheck = true;
                            }
                        }
                        if (!playerCheck)
                        {
                            float distance = (selected.transform.position - hit.point)
                                .sqrMagnitude;
                            if (distance < 1f * 1f)
                            {
                                toCompare.Add(selected, distance);
                            }
                        }
                    }
                }
                    
                try
                {
                    var returnItem = toCompare.Aggregate((l, r) => l.Value < r.Value ? l : r).Key;
                    ExecuteEngorgio(returnItem);
                }
                catch (InvalidOperationException e)
                {
                    Debug.Log("Engorgio found no items to return.");
                }
            }
        }
    }
}