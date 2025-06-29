using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LegacyOfMagic.Management;
using LegacyOfMagic.Spells.SpellMonos.Updaters;
using UnityEngine;
using ThunderRoad;
using UnityEngine.VFX;

namespace LegacyOfMagic.Spells.SpellMonos
{
    public class Engorgio : Spell
    {
        private Item hitItem;
        public override void Start()
        {
            base.Start();
            usedWand = GetComponent<Item>();
            Cast();
        }

        private IEnumerator CastSpellEffect(VisualEffect vfx, Item item)
        {
            yield return base.CastSpellEffect(vfx);
            usedWand.mainHandler.playerHand.ragdollHand.PlayHapticClipOver(new AnimationCurve(new []{new Keyframe(0,0), new Keyframe(1, 2)}), 0.3f);
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

        private IEnumerator ExecuteEngorgio(Item item)
        {
            followTransform = item.transform;
            hitItem = item;
           GameManager.local.StartCoroutine(SetupCast(item.Center, ModEntry.engorgioCastEffect));
           yield return null;
        }

        public override void ExecuteAfterInstantiate()
        {
            GameManager.local.StartCoroutine(CastSpellEffect(activeCast, hitItem));
        }
        public override void ExecuteIfCached()
        {
            GameManager.local.StartCoroutine(CastSpellEffect(activeCast, hitItem));
        }

        public override void Cast()
        {
            hitItem = GetItemsWithinAngleAndDistance(usedWand.flyDirRef.transform.position,
                usedWand.flyDirRef.forward, 45f, 60f);

            if (hitItem != null) GameManager.local.StartCoroutine(ExecuteEngorgio(hitItem));
        }
        
    }
}