using System;
using LegacyOfMagic.Management;
using UnityEngine;
using ThunderRoad;
namespace LegacyOfMagic.Spells.SpellMonos
{
    class Lumos : Spell
    {
        
        internal string command;
        private Side lastSide;
        void Start()
        {
            usedWand = GetComponent<Item>();
            Cast();
        }

        public override void Cast()
        {
            String parsedType = GetType().ToString().Split('.')[3];
            if (!spell)
            {
                try
                {
                    Catalog.GetData<ItemData>(parsedType + "Object").SpawnAsync(callback =>
                    {
                        spell = callback;
                        usedWand.mainHandler.playerHand.ragdollHand.PlayHapticClipOver(
                            new AnimationCurve(new[] { new Keyframe(0, 0), new Keyframe(1, 2) }), 0.3f);
                        spell.IgnoreObjectCollision(usedWand);
                        spell.IgnoreRagdollCollision(Player.currentCreature.ragdoll);
                        ModEntry.local.activeHandlers.Add(usedWand, this);
                    });
                    ModEntry.local.currentTippers.Add(spell);
                }
                catch (Exception e)
                {
                    Debug.Log("Unable to spawn Lumos Object with error: " + e);
                }
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