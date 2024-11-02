using LegacyOfMagic.Management;
using LegacyOfMagic.Spells.SpellMonos.Updaters;
using ThunderRoad;
using UnityEngine;
namespace LegacyOfMagic.Spells.SpellMonos
{
    public class Dissimulo : Spell
    {
         private Material evanescoDissolve;
        private Creature creature;
        private bool canDisillusion;
        private float dissolveVal;
        void Start()
        {
            Cast();
        }


        public override void Cast()
        {
            if (!ModEntry.local.dissimuloActive)
            {
                creature = Player.currentCreature;
                evanescoDissolve = ModEntry.local.dissimuloDissolveMat.DeepCopyByExpressionTree();
                ModEntry.local.activeDisillusion = new GameObject();
                ModEntry.local.activeDisillusion.AddComponent<DissimuloActive>();
                ModEntry.local.dissimuloActive = true;
                canDisillusion = true;
                dissolveVal = 0;
                StartDissimulo();

            }
        }


        void StartDissimulo()
        {
            foreach (Creature.RendererData data in creature.renderers)
            {
                Material evanescoTemp = evanescoDissolve = ModEntry.local.dissimuloDissolveMat.DeepCopyByExpressionTree();
                ModEntry.local.originalCreatureMaterial.Add(data.renderer.materials);
                Material[] myMaterials = data.renderer.materials;
                Material[] matDefGood = new Material[myMaterials.Length];
                
                for (int i = 0; i < myMaterials.Length; i++)
                {
                    evanescoTemp.SetTexture("_Albedo", myMaterials[i].GetTexture("_BaseMap"));
                    evanescoTemp.SetColor("_color", myMaterials[i].GetColor("_BaseColor"));
                    evanescoTemp.SetTexture("_Normal", myMaterials[i].GetTexture("_BumpMap"));
                    evanescoTemp.SetTexture("_Metallic", myMaterials[i].GetTexture("_MetallicGlossMap"));

                    matDefGood[i] = evanescoTemp.DeepCopyByExpressionTree();
                }

                data.renderer.materials = matDefGood;
            }
        }

        private void Update()
        {
            if (canDisillusion)
            {
                if (dissolveVal < 1)
                {
                    dissolveVal += 0.01f;
                    foreach (Creature.RendererData var in creature.renderers)
                    {
                        foreach (Material mat in var.renderer.materials)
                        {
                            mat.SetFloat("_dissolve", dissolveVal);
                        }
                    }
                }
                else if (dissolveVal >= 1f)
                {
                    dissolveVal = 0;
                    canDisillusion = false;
                }

            }
        }
    }
}