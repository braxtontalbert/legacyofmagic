using System;
using System.Collections;
using System.Linq;
using LegacyOfMagic.Management;
using LegacyOfMagic.Modules;
using LegacyOfMagic.Modules.ContainerContents;
using ThunderRoad;
using UnityEngine;
using UnityEngine.VFX;

namespace LegacyOfMagic.Spells.SpellMonos
{
    public class Revelio : Spell
    {
        private Item hitItem;
        private GameObject spawnedEffect;
        private bool loopSpellNames;
        public override void Start()
        {
            usedWand = GetComponent<Item>();
            base.Start();
            Cast();
        }
        void ExecuteRevelio(Item item)
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

                
                var instance = item.gameObject.GetComponent<SpellBookInstance>();
                if (!instance.hasSpellBeenRevealed)
                {
                    instance.hasSpellBeenRevealed = true;
                    instance.spellNameAssigned = instance.ReturnRandomFromAvailable();
                    instance.SetContent();
                }
                (string,Texture) tuple = ModEntry.local.ReturnSpellTextureByName(instance.spellNameAssigned);

                if (tuple.Item1 != "Null")
                {
                    foreach (var vfxReveal in vfxList)
                    {
                        if (vfxReveal.gameObject.name == "MenuEffect")
                        {
                            spawnedEffect = vfxReveal.gameObject;
                            loopSpellNames = true;
                            if (instance.counterCursesToLearnWithSpell.TryGetValue(tuple.Item1, out var value1))
                            {
                                GameManager.local.StartCoroutine(
                                    LoopBetweenMainAndCounterSpell(vfxReveal, value1, tuple.Item1));
                            }
                            else
                            {
                                vfxReveal.SetTexture("currentSDF", tuple.Item2);
                                vfxReveal.Play();
                            }

                            if (!GameModeSpellManager.local.currentSpellsForCharacter.Contains(tuple.Item1))
                            {
                                GameModeSpellManager.local.currentSpellsForCharacter.Add(tuple.Item1);
                                if (instance.counterCursesToLearnWithSpell.TryGetValue(tuple.Item1, out var value))
                                    GameModeSpellManager.local.currentSpellsForCharacter.Add(value);
                                instance.availableSpells.Remove(tuple.Item1);
                                instance.availableSpells.TrimExcess();
                                GameModeSpellManager.local.ModifyChoices();
                            }
                        }
                        else vfxReveal.Play();
                    }
                }
            }
        }

        private IEnumerator LoopBetweenMainAndCounterSpell(VisualEffect vfx, string counter, string main)
        {
            vfx.Play();
            yield return new WaitUntil(() => vfx.aliveParticleCount > 0);
            while (loopSpellNames && vfx.aliveParticleCount > 0)
            {
                vfx.SetTexture("currentSDF", ModEntry.local.ReturnSpellTextureByName(main).Item2);

                yield return new WaitForSeconds(3f);
                
                vfx.SetTexture("currentSDF", ModEntry.local.ReturnSpellTextureByName(counter).Item2);

                yield return new WaitForSeconds(3f);
            }
        }
        public override void Cast()
        {
            if (Physics.Raycast(usedWand.flyDirRef.transform.position, usedWand.flyDirRef.transform.forward, out RaycastHit hit,
                    float.MaxValue, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                Debug.Log(hit.collider.name);
                if(hit.collider.gameObject.GetComponentInParent<Item>() is Item item){
                    ExecuteRevelio(item);
                }
            }
        }

        public override void Update()
        {
            if(spawnedEffect != null) spawnedEffect.transform.LookAt(Player.currentCreature.ragdoll.headPart.transform);
            base.Update();
        }
    }
}