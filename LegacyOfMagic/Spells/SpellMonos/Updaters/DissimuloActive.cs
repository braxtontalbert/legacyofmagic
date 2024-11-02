using LegacyOfMagic.Management;
using ThunderRoad;
using UnityEngine;
namespace LegacyOfMagic.Spells.SpellMonos.Updaters
{
    public class DissimuloActive : MonoBehaviour
    {
        public float ogVertFOV;
        

        private void Update()
        {
            foreach (Creature creature in Creature.allActive)
            {
                if (creature.brain.instance.GetModule<BrainModuleDetection>() is BrainModuleDetection detector)
                {
                    if (detector.sightDetectionHorizontalFov != 0 && detector.sightDetectionVerticalFov != 0)
                    {
                        if (!ModEntry.local.creaturesFOV.ContainsKey(creature))
                        {
                            ModEntry.local.creaturesFOV.Add(creature, new[] { detector.sightDetectionVerticalFov, detector.sightDetectionHorizontalFov });
                            
                            detector.sightDetectionHorizontalFov = 0f;
                            detector.sightDetectionVerticalFov = 0f;
                        }
                        else
                        {
                            detector.sightDetectionHorizontalFov = 0f;
                            detector.sightDetectionVerticalFov = 0f;
                        }
                    }
                    else {
                        if ((Player.currentCreature.transform.position - creature.transform.position).sqrMagnitude < 1f * 1f)
                        {
                            if (ModEntry.local.creaturesFOV.ContainsKey(creature))
                            {
                                detector.sightDetectionHorizontalFov = ModEntry.local.creaturesFOV[creature][1];
                                detector.sightDetectionVerticalFov = ModEntry.local.creaturesFOV[creature][0];
                            }
                        }
                        
                    }
                    
                }
            }
        }
    }
}