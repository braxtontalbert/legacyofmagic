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
    public class Reducio : Spell
    {
        private Item hitItem;
        public void Start()
        {
            usedWand = GetComponent<Item>();
            Cast();
        }

        public IEnumerator CastSpellEffect(VisualEffect vfx, Item item)
        {
            yield return base.CastSpellEffect(vfx);
            if (item.gameObject.GetComponent<SizeManager>() is SizeManager sm)
            {
                if (!sm.changeSize)
                {
                    sm.changeSize = true;
                    sm.direction = false;
                }
            }
            else
            {
                var local = item.gameObject.AddComponent<SizeManager>();
                local.changeSize = true;
            }
        }

        public override void ExecuteIfCached()
        {
            GameManager.local.StartCoroutine(CastSpellEffect(activeCast, hitItem));
        }

        public override void ExecuteAfterInstantiate()
        {
            GameManager.local.StartCoroutine(CastSpellEffect(activeCast, hitItem));
        }

        void ExecuteReducio(Item item)
        {
            followTransform = item.transform;
            hitItem = item;
            GameManager.local.StartCoroutine(SetupCast(item.Center, ModEntry.engorgioCastEffect));
        }
        public override void Cast()
        {
            hitItem = GetItemsWithinAngleAndDistance(usedWand.flyDirRef.transform.position,
                usedWand.flyDirRef.forward, 45f, 60f);

            if (hitItem != null) ExecuteReducio(hitItem);
        }
    }
}