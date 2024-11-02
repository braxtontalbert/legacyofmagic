using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using LegacyOfMagic.Spells.SpellMonos.Updaters;
using ThunderRoad;
using UnityEngine;

namespace LegacyOfMagic.Spells.SpellMonos
{
    public class Accio : Spell
    {
        private String itemType;
        Item npcItem;
        internal Vector3 startPoint;
        internal Vector3 endPoint;
        internal GameObject parentLocal;
        private float elapsedTime;
        public string componentLevel;
        RagdollHand oppositeHand;


        public override void Cast()
        {
            itemType = getExtraData();
            CastRay();
        }

        public void Start()
        {
            usedWand = GetComponent<Item>();
            Cast();
        }


        void StartAccio(Item currentItem) {

            
            if (currentItem)
            {

                if (currentItem.mainHandler && !currentItem.mainHandler.creature.isPlayer)
                {
                    currentItem.mainHandler.UnGrab(false);
                }
                if (currentItem.gameObject.GetComponent<AccioPerItem>() is AccioPerItem api)
                {
                    api.AddWand(usedWand);
                    api.cantAccio = false;
                }

                else
                {
                    currentItem.gameObject.AddComponent<AccioPerItem>().AddWand(usedWand);
                }
            }
        }
        Item CheckHit(RagdollHand opposite, GameObject parentLocal, Vector3 hit) {

            if (!opposite.grabbedHandle)
            {
                
                if (itemType != null && itemType.ToLower().Equals("weapon"))
                {

                    if (parentLocal.GetComponentInParent<Item>() is Item accioItem2 &&
                        accioItem2.name.ToLower().Contains(itemType))
                    {
                       
                        return accioItem2;
                    }
                    
                    Dictionary<Item, float> toCompare = new Dictionary<Item, float>();
                    foreach (Item selected in Item.allActive)
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
                            if (!Player.local.creature.equipment.GetAllHolsteredItems().Contains(selected) &&
                                !selected.mainHandler.creature.isPlayer)
                            {
                                float distance = (selected.transform.position - hit)
                                    .sqrMagnitude;
                                if (distance < 5f * 5f)
                                {
                                    if (selected.data.type.ToString().ToLower().Contains(itemType.ToLower()) &&
                                        !toCompare.ContainsKey(selected))
                                    {
                                        toCompare.Add(selected, distance);
                                    }
                                }
                            }
                        }
                    }

                    Item returnItem;
                    try
                    {
                        returnItem = toCompare.Aggregate((l, r) => l.Value < r.Value ? l : r).Key;
                    }
                    catch (InvalidOperationException e)
                    {
                        Debug.Log("Accio found no items to return.");
                        returnItem = null;
                    }

                    return returnItem;
                }
                else
                {
                    if (parentLocal.GetComponentInParent<Item>() is Item accioItem2)
                    {
                        if (accioItem2.itemId.ToLower().Contains("door")) return null;
                        return accioItem2;

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
                                float distance = (selected.transform.position - hit)
                                    .sqrMagnitude;
                                if (distance < 5f * 5f)
                                {
                                    toCompare.Add(selected, distance);
                                }
                            }
                        }
                    }

                    Item returnItem;
                    try
                    {
                        returnItem = toCompare.Aggregate((l, r) => l.Value < r.Value ? l : r).Key;
                    }
                    catch (InvalidOperationException e)
                    {
                        Debug.Log("Accio found no items to return.");
                        returnItem = null;
                    }

                    return returnItem;
                }

            }
            return null;
        }

        internal void CastRay() {
            
            RaycastHit hit;
            GameObject parent;
            if (itemType != null)
            {
                if (Physics.Raycast(usedWand.flyDirRef.transform.position, usedWand.flyDirRef.transform.forward, out hit,
                        float.MaxValue, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                {

                    parent = hit.collider.gameObject;

                    oppositeHand = usedWand.mainHandler.otherHand;

                    StartAccio(CheckHit(oppositeHand, parent, hit.point));

                }

            }
            else
            {
                if (Physics.Raycast(usedWand.flyDirRef.transform.position, usedWand.flyDirRef.transform.forward, out hit,
                        float.MaxValue, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                {

                    parent = hit.collider.gameObject;

                    oppositeHand = usedWand.mainHandler.otherHand;

                    StartAccio(CheckHit(oppositeHand, parent, hit.point));

                }
            }

        }

    }
}