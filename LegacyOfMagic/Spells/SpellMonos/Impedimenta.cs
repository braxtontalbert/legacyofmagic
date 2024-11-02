using System.Collections;
using LegacyOfMagic.Management;
using LegacyOfMagic.Spells.SpellMonos.Updaters;
using ThunderRoad;
using UnityEngine;
using UnityEngine.VFX;

namespace LegacyOfMagic.Spells.SpellMonos
{
    public class Impedimenta : Spell
    {
        GameObject go;
        private GameObject sfx;


        public override void Cast()
        {
            usedWand.mainHandler.playerHand.ragdollHand.PlayHapticClipOver(new AnimationCurve(new []{new Keyframe(0,0), new Keyframe(1, 2)}), 0.3f);
            StartImpedimenta();
        }

        void Start() {
            usedWand = GetComponent<Item>();
            Cast();
        }
        void StartImpedimenta() {
            foreach (var creature in Creature.allActive)
            {
                if (creature.isPlayer) continue;
                if((Player.currentCreature.transform.position - creature.transform.position).sqrMagnitude < 5f * 5f)
                {
                    creature.locomotion.SetSpeedModifier(this, 0.3f, 0.3f, 0.3f, 0.3f, 0.3f);
                    creature.animator.speed = 0.3f;
                    creature.gameObject.AddComponent<CreaturesReversalEvent>();
                }
            }

            if (!sfx)
            {
                sfx = Instantiate(ModEntry.local.impedimentaSoundFX);

            }
            else sfx.GetComponent<AudioSource>().Play();

            if (!go)
            {
                ModEntry.local.impedimentaEffect.transform.position = usedWand.flyDirRef.transform.position;
                go = Instantiate(ModEntry.local.impedimentaEffect);
            }
            else
            {
                go.transform.position = usedWand.flyDirRef.transform.position;
                go.GetComponentInChildren<VisualEffect>().Play();
            }

        }
    }
}