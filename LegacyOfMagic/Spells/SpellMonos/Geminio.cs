using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LegacyOfMagic.Management;
using ThunderRoad;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

namespace LegacyOfMagic.Spells.SpellMonos
{
    public class Geminio : Spell
    {
        ItemData copyData;
        private GameObject sfx;
        private Item hitItem;

        public void Start()
        {
            usedWand = GetComponent<Item>();
            Cast();
        }

        public override void Cast()
        {
            hitItem = GetItemsWithinAngleAndDistance(usedWand.flyDirRef.transform.position,
                usedWand.flyDirRef.forward, 45f, 60f);

            if (hitItem != null) ExecuteGeminio(hitItem);
        }

        void ExecuteGeminio(Item item)
        {
            followTransform = item.transform;
            hitItem = item;
            GameManager.local.StartCoroutine(SetupCast(item.Center, ModEntry.geminioCastEffect));
        }

        public override void ExecuteAfterInstantiate()
        {
            GameManager.local.StartCoroutine(CastSpellEffect(activeCast, hitItem));
        }

        public override void ExecuteIfCached()
        {
            GameManager.local.StartCoroutine(CastSpellEffect(activeCast, hitItem));
        }

        public IEnumerator CastSpellEffect(VisualEffect vfx, Item item)
        {
            yield return base.CastSpellEffect(vfx);
            usedWand.mainHandler.playerHand.ragdollHand.PlayHapticClipOver(new AnimationCurve(new []{new Keyframe(0,0), new Keyframe(1, 2)}), 0.3f);
            copyData = item.data;
            copyData.SpawnAsync(copy => {
                copy.IgnoreItemCollision(item);
                copy.transform.position = item.transform.position;
                copy.transform.rotation = item.transform.rotation;
                float x = Random.Range(-3f, 3f);
                float y = Random.Range(0.01f, 3f);
                float z = Random.Range(-3f, 3f);

                if (!sfx)
                {
                    sfx = Instantiate(ModEntry.local.geminioPop);
                    sfx.transform.position = copy.transform.position;
                }
                else
                {
                    sfx.transform.position = copy.transform.position;
                    sfx.gameObject.GetComponent<AudioSource>().Play();
                }

                Vector3 randomVector = new Vector3(x, y, z);
                copy.physicBody.rigidBody.AddForce((Vector3.up + randomVector.normalized) * 5f * copy.physicBody.mass, ForceMode.Impulse);
                GameManager.local.StartCoroutine(Timer(copy, item));
            });
            
        }

        IEnumerator Timer(Item copy, Item original)
        {
            yield return new WaitForSeconds(1f);
            copy.IgnoreItemCollision(original, false);
        }
    }
}