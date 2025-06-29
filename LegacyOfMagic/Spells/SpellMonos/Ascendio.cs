using System.Linq;
using ThunderRoad;
using UnityEngine;
namespace LegacyOfMagic.Spells.SpellMonos
{
    public class Ascendio : Spell
    {
        Item item;
        float ascendioPower;
        float ascendioDefault = 2000f;

        public override void Cast()
        {
            Debug.Log("Ascending");
            if (Player.local.locomotion.isGrounded)
            {
                Player.local.locomotion.Jump(true);
            }
            Ascend();
        }

        public void Start() {

            item = GetComponent<Item>();
            Player.currentCreature.groundStabilizationMaxVelocity = 0f;
            ascendioPower = ascendioDefault;
            Player.local.creature.waterHandler.OnWaterEnter += WaterHandler_OnWaterEnter;
            Player.local.creature.waterHandler.OnWaterExit += WaterHandler_OnWaterExit;
            Cast();
        }

        private void WaterHandler_OnWaterExit()
        {
            ascendioPower = ascendioDefault;
        }

        private void WaterHandler_OnWaterEnter()
        {
            ascendioPower = ascendioDefault * 2f;
        }

        public void Ascend() {
            item?.mainHandler?.playerHand?.ragdollHand?.PlayHapticClipOver(new AnimationCurve(new []{new Keyframe(0,0), new Keyframe(1, 2)}), 0.3f);
            foreach (Rigidbody rigidbody in Player.currentCreature.ragdoll.parts.Select(part => part.physicBody.rigidBody)) {
                Debug.Log("rigidbody is: " + rigidbody);
                if (rigidbody != null)
                {
                    rigidbody.AddForce(item.flyDirRef.transform.forward * ascendioPower, ForceMode.Impulse);
                }

            
            }


        }
    }
}