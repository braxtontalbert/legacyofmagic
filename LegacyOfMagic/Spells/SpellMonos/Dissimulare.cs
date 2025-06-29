using System.Collections.Generic;
using LegacyOfMagic.Management;
using ThunderRoad;
using UnityEngine;
namespace LegacyOfMagic.Spells.SpellMonos
{
    public class Dissimulare : Spell
    {
         private Material evanescoDissolve;
        private Creature creatureRendering;
        List<Material> myMaterials;
        private bool canDisillusion;
        private float dissolveVal;
        private Material[] original;
        void Start()
        {
            Cast();
        }

        public override void Cast()
        {
            creatureRendering = Player.currentCreature;
            evanescoDissolve = ModEntry.local.dissimuloDissolveMat.DeepCopyByExpressionTree();
            dissolveVal = 1f;
            canDisillusion = true;
            ModEntry.local.dissimuloActive = false;
            Object.Destroy(ModEntry.local.activeDisillusion);
            foreach (Creature creature in Creature.allActive)
            {
                if (creature.brain.instance.GetModule<BrainModuleDetection>() is BrainModuleDetection detector)
                {
                    if (ModEntry.local.creaturesFOV.TryGetValue(creature, out var value))
                    {
                        Debug.Log("Vertical: " + value[0]);
                        Debug.Log("Horizontal: " + ModEntry.local.creaturesFOV[creature][1]);
                        float vertical = ModEntry.local.creaturesFOV[creature][0];
                        float horizontal = ModEntry.local.creaturesFOV[creature][1];
                        detector.sightDetectionHorizontalFov = horizontal;
                        detector.sightDetectionVerticalFov = vertical;
                    }
                }
            }
        }

        private void Update()
        {
            if (canDisillusion)
            {
                if (dissolveVal > 0)
                { 
                    dissolveVal -= 0.01f;
                    foreach (Creature.RendererData var in creatureRendering.renderers)
                    {
                        foreach (Material mat in var.renderer.materials)
                        {
                            mat.SetFloat("_dissolve", dissolveVal);
                        }
                    }
                }
                else if (dissolveVal <= 0)
                {
                    dissolveVal = 1f;
                    canDisillusion = false;
                    for (int i = 0; i < creatureRendering.renderers.Count; i++) {
                        creatureRendering.renderers[i].renderer.materials = ModEntry.local.originalCreatureMaterial[i];
                    }

                }

            }
        }
    }
}