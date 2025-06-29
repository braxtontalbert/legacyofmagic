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
    public class Evanesco : Spell
    {
        internal Vector3 endPoint;
        internal Vector3 ogScale;
        internal Material evanescoDissolve;
        List<Material> myMaterials;
        private Item hitItem;

        public override void Start()
        {
            base.Start();
            usedWand = GetComponent<Item>();
            evanescoDissolve = ModEntry.local.evanescoDissolveMat.DeepCopyByExpressionTree();
            Cast();

        }

        public override void Cast()
        {
            hitItem = GetItemsWithinAngleAndDistance(usedWand.flyDirRef.transform.position,
                usedWand.flyDirRef.forward, 45f, 60f);

            if (hitItem != null) EvanescoExecute(hitItem);
        }

        IEnumerator CastSpellEffect(VisualEffect vfx, Item item)
        {
            yield return base.CastSpellEffect(vfx);
            usedWand.mainHandler.playerHand.ragdollHand.PlayHapticClipOver(new AnimationCurve(new []{new Keyframe(0,0), new Keyframe(1, 2)}), 0.3f);
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

        public override void ExecuteAfterInstantiate()
        {
            GameManager.local.StartCoroutine(CastSpellEffect(activeCast, hitItem));
        }
        public override void ExecuteIfCached()
        {
            GameManager.local.StartCoroutine(CastSpellEffect(activeCast, hitItem));
        }
        void EvanescoExecute(Item item)
        {
            followTransform = item.transform;
            hitItem = item;
            GameManager.local.StartCoroutine(SetupCast(item.Center, ModEntry.evanescoCastEffect));
        }
    }
}