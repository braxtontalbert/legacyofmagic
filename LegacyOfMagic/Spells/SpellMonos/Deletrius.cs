using System.Collections;
using System.Linq;
using LegacyOfMagic.Management;
using LegacyOfMagic.Modules;
using ThunderRoad;
using UnityEngine;
using UnityEngine.VFX;

namespace LegacyOfMagic.Spells.SpellMonos
{
    public class Deletrius : Spell
    {
        private Item hitItem;
        public override void Start()
        {
            usedWand = GetComponent<Item>();
            base.Start();
            Cast();
        }
        void ExecuteDeletrius(Item item)
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
            var containsChildClass = item.data.modules.OfType<SpellBookModule>().ToList();
            foreach (var module in containsChildClass)
            {
                var vfxList = item.GetComponentsInChildren<VisualEffect>();

                foreach (var vfxReveal in vfxList)
                {
                    vfxReveal.Stop();
                }
            }
        }
        public override void Cast()
        {
            Debug.Log("Ray cast start");
            if (Physics.Raycast(usedWand.flyDirRef.transform.position, usedWand.flyDirRef.transform.forward, out RaycastHit hit,
                    float.MaxValue, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                Debug.Log(hit.collider.name);
                if(hit.collider.gameObject.GetComponentInParent<Item>() is Item item){
                    Debug.Log("Item is: " + item.name);
                    ExecuteDeletrius(item);
                }
            }
        }
    }
}