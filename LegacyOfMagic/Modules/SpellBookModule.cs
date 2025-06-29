using System;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;
using UnityEngine.VFX;

namespace LegacyOfMagic.Modules
{
    public class SpellBookModule : ItemModule
    {
        public SpellBook spellBookType;
        private Item item;
        public override void OnItemLoaded(Item itemLoaded)
        {
            base.OnItemLoaded(itemLoaded);
            item = itemLoaded;
            item.OnItemRetrieved += OnItemRetrieved;

            if (!item.gameObject.GetComponent<SpellBookInstance>())
            {
                var instance = item.gameObject.AddComponent<SpellBookInstance>();
                instance.availableSpells = new List<string>();
                instance.spellBookType = spellBookType;
                instance.AddByType();
            }
        }

        private void SpawnEvent(EventTime eventtime)
        {
            if (eventtime == EventTime.OnStart)
            {
                Debug.Log(item);
                var vfxs = item.gameObject.GetComponentsInChildren<VisualEffect>();
                foreach (var VARIABLE in vfxs)
                {
                    VARIABLE.Stop();
                }
            }
        }

        private void OnItemRetrieved(UIInventory inventory, ItemContent itemcontent, Item item1)
        {
            foreach(var vfx in item1.GetComponentsInChildren<VisualEffect>())
            {
                vfx.Stop();
            }
        }
    }
}