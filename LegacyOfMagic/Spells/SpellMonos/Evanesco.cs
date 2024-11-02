using System;
using System.Collections.Generic;
using System.Linq;
using LegacyOfMagic.Management;
using LegacyOfMagic.Spells.SpellMonos.Updaters;
using ThunderRoad;
using UnityEngine;
namespace LegacyOfMagic.Spells.SpellMonos
{
    public class Evanesco : Spell
    {
        internal Vector3 endPoint;
        internal Vector3 ogScale;
        internal Material evanescoDissolve;
        List<Material> myMaterials;


        public void Start()
        {
            usedWand = GetComponent<Item>();
            evanescoDissolve = ModEntry.local.evanescoDissolveMat.DeepCopyByExpressionTree();
            Cast();

        }

        public override void Cast()
        {
            CastRay();
        }

        private void CastRay()
        {

            RaycastHit hit;
            GameObject parent;

            if (Physics.Raycast(usedWand.flyDirRef.transform.position, usedWand.flyDirRef.transform.forward, out hit, float.MaxValue, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                parent = hit.collider.gameObject;
                if (parent.GetComponentInParent<Item>() is Item evanescoItem)
                {
                    EvanescoExecute(evanescoItem);
                }
                else
                {
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
                        EvanescoExecute(returnItem);
                    }
                    catch (InvalidOperationException e)
                    {
                        Debug.Log("Evanesco found no items to return.");
                    }
                }
            }
        }

        void EvanescoExecute(Item item)
        {
            foreach (var renderer in item.renderers)
            {
                myMaterials = renderer.materials.ToList();
                Material[] matDefGood = new Material[myMaterials.Count];

                for (int i = 0; i < myMaterials.Count; i++)
                {
                    evanescoDissolve.SetTexture("_Albedo", myMaterials[i].GetTexture("_BaseMap"));
                    evanescoDissolve.SetColor("_color", myMaterials[i].GetColor("_BaseColor"));
                    evanescoDissolve.SetTexture("_Normal", myMaterials[i].GetTexture("_BumpMap"));
                    evanescoDissolve.SetTexture("_Metallic", myMaterials[i].GetTexture("_MetallicGlossMap"));

                    matDefGood[i] = evanescoDissolve;
                }
                renderer.materials = matDefGood;
                item.gameObject.AddComponent<EvanescoPerItem>(); 
            }
        }
    }
}