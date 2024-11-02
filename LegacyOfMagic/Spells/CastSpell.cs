using System;
using System.Reflection;
using JetBrains.Annotations;
using LegacyOfMagic.Management;
using ThunderRoad;
using UnityEngine;

namespace LegacyOfMagic.Spells
{
    public class CastSpell : MonoBehaviour
    {
        public void Activate(String spell, Item wand, [CanBeNull] String extraData)
        {
            if (ProjectileProperties.local.spellTypes[spell].Equals("projectile"))
            {
                Catalog.GetData<ItemData>(spell + "Object").SpawnAsync(projectile =>
                {
                    wand.mainHandler.playerHand.ragdollHand.PlayHapticClipOver(new AnimationCurve(new []{new Keyframe(0,0), new Keyframe(1, 2)}), 0.3f);
                    var transform1 = wand.flyDirRef.transform;
                    projectile.transform.position = transform1.position;
                    projectile.transform.rotation = transform1.rotation;

                    var refSpell = (Spell) projectile.gameObject.AddComponent(Type.GetType("LegacyOfMagic.Spells.SpellMonos."+spell));
                    refSpell.usedWand = wand;
                    projectile.IgnoreObjectCollision(wand);
                    projectile.IgnoreRagdollCollision(Player.currentCreature.ragdoll);
                    
                    projectile.Throw();
                    
                    projectile.physicBody.rigidBody.useGravity = ProjectileProperties.local.useGravity;
                    projectile.physicBody.rigidBody.drag = ProjectileProperties.local.drag;

                    foreach (var source in wand.GetComponentsInChildren<AudioSource>())
                    {
                        if (source.name.Contains(spell)) source.Play();
                    }
                    
                    projectile.physicBody.rigidBody.AddForce(wand.flyDirRef.forward * ProjectileProperties.local.spellSpeed, ForceMode.Impulse);
                });
            }
            else if(ProjectileProperties.local.spellTypes[spell].Equals("cast"))
            {
                Debug.Log("In cast");
                if (wand.gameObject.GetComponent(Type.GetType("LegacyOfMagic.Spells.SpellMonos." + spell)) is Spell spellMono)
                {
                    spellMono.Cast();
                }
                else
                {
                    var spellInstantiated = (Spell) wand.gameObject.AddComponent(Type.GetType("LegacyOfMagic.Spells.SpellMonos." + spell));
                    if(extraData != null) spellInstantiated.AddExtraData(extraData);
                }
            }
        }
    }
}