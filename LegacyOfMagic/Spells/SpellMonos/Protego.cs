using System;
using System.Collections;
using System.Collections.Generic;
using LegacyOfMagic.Management;
using ThunderRoad;
using UnityEngine;
using UnityEngine.VFX;

namespace LegacyOfMagic.Spells.SpellMonos
{
    class Protego : Spell
    {
        private Side lastSide;
        public void Start()
        {
            usedWand = GetComponent<Item>();
            usedWand.OnHeldActionEvent += HeldActionEvent;
            Cast();
        }

        public override void Cast()
        {
            String parsedType = GetType().ToString().Split('.')[3];
            Catalog.GetData<ItemData>(parsedType + "Object").SpawnAsync(callback =>
            {
                spell = callback;
                spell.IgnoreObjectCollision(usedWand);
                spell.IgnoreRagdollCollision(Player.currentCreature.ragdoll);
                var transform1 = usedWand.flyDirRef.transform;
                spell.gameObject.transform.position = transform1.position;
                spell.gameObject.transform.rotation = transform1.rotation;
            });
        }

        private void HeldActionEvent(RagdollHand ragdollhand, Handle handle, Interactable.Action action)
        {
            if (action == Interactable.Action.AlternateUseStart)
            {
                Debug.Log(lastSide);
                PlayerControl.GetHand(lastSide).StopHapticLoop(this);
                spell.Despawn();
            }
        }


        public void OnTriggerEnter(Collider other) {

            if (other.gameObject.GetComponentInParent<AvadaKedavra>() && other.gameObject.GetComponentInParent<Item>() is Item itemParent)
            {
                itemParent.IgnoreObjectCollision(spell);
            }
        }
        
        void Update() {
            if (spell)
            {
                var transform1 = usedWand.flyDirRef.transform;
                spell.transform.position = transform1.position;
                spell.transform.rotation = transform1.rotation;
                if (usedWand.mainHandler != null)
                {
                    if (lastSide != usedWand.mainHandler.side)
                    {
                        PlayerControl.GetHand(lastSide).StopHapticLoop(this);
                    }
                    lastSide = usedWand.mainHandler.side;
                    float velocity = usedWand.physicBody.velocity.magnitude;
                    float ratio = Utils.CalculateRatio(velocity, 0f, 3f,
                        Catalog.gameData.haptics.telekinesisIntensity.x,
                        Catalog.gameData.haptics.telekinesisIntensity.y);
                    float period = Utils.CalculateRatio(velocity, 0f, 3f, Catalog.gameData.haptics.telekinesisPeriod.x,
                        Catalog.gameData.haptics.telekinesisPeriod.y);
                    float intensity =
                        (ratio * Catalog.gameData.haptics.telekinesisMassIntensity.Evaluate(usedWand.physicBody.mass))
                        .Clamp(0.0f, 1f);
                    PlayerControl.GetHand(lastSide).HapticLoop(this, intensity, period);
                }
                else
                {
                    PlayerControl.GetHand(lastSide).StopHapticLoop(this);
                }
            }
        }
    }
}