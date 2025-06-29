using System.Linq;
using LegacyOfMagic.Management;
using ThunderRoad;
using UnityEngine;
namespace LegacyOfMagic.Spells.SpellMonos
{
    public class ArrestoMomentum : Spell
    {
        bool canSlow;

        public override void Cast()
        {
            ModEntry.local.spellsOnPlayer.Add(typeof(ArrestoMomentum));
            usedWand?.mainHandler?.playerHand?.ragdollHand?.PlayHapticClipOver(new AnimationCurve(new []{new Keyframe(0,0), new Keyframe(1, 2)}), 0.3f);
            canSlow = true;
        }

        void Start() {
            Player.local.locomotion.OnGroundEvent += Locomotion_OnGroundEvent;
            usedWand = GetComponent<Item>();
            Cast();
        }

        private void Locomotion_OnGroundEvent(Locomotion locomotion, Vector3 groundpoint, Vector3 velocity, Collider groundcollider)
        {
            canSlow = false;
            ModEntry.local.spellsOnPlayer.Remove(typeof(ArrestoMomentum));
        }

        public void StartArrestoMomentum() {

            foreach (Rigidbody rigidbody in Player.currentCreature.ragdoll.parts.Select(part => part.physicBody.rigidBody))
            {
                rigidbody.velocity = -Vector3.down * 5f;
            }


        }

        void Update() {
            if (canSlow) StartArrestoMomentum();
        }
    }
}