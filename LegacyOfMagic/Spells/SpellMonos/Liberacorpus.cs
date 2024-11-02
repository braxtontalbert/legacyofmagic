using LegacyOfMagic.Management;
using ThunderRoad;
using UnityEngine;

namespace LegacyOfMagic.Spells.SpellMonos
{
    class Liberacorpus : Spell
    {
        void Start()
        {
            usedWand = GetComponent<Item>();
            usedWand.mainHandler.playerHand.ragdollHand.PlayHapticClipOver(new AnimationCurve(new []{new Keyframe(0,0), new Keyframe(1, 2)}), 0.3f);
            Cast();
        }

        public override void Cast()
        {
            usedWand.mainHandler.playerHand.ragdollHand.PlayHapticClipOver(new AnimationCurve(new []{new Keyframe(0,0), new Keyframe(1, 2)}), 0.3f);
            DestroyLevicorpus();
        }
        
        public void DestroyLevicorpus()
        {
            foreach (Creature creature in ModEntry.local.levicorpusedCreatures)
            {
                Destroy(creature.footLeft.GetComponent<SpringJoint>());
                Destroy(creature.footRight.GetComponent<SpringJoint>());
            }
            foreach (GameObject go in ModEntry.local.floaters) Destroy(go);
        }
    }
}