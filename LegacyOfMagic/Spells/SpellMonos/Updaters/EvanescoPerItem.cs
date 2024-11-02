using System.Collections.Generic;
using UnityEngine;
using ThunderRoad;
namespace LegacyOfMagic.Spells.SpellMonos.Updaters
{
    public class EvanescoPerItem : MonoBehaviour
    {
        bool cantEvanesco;
        Item item;
        private float elapsedTime;
        float dissolveVal;
        private List<Renderer> renderers;

        void Start()
        {
            item = GetComponent<Item>();
            cantEvanesco = false;
            dissolveVal = 0;
            renderers = item.renderers;
        }


        void Update()
        {
            if (cantEvanesco == false)
            {
                dissolveVal += 0.01f;
                if (dissolveVal < 1)
                {
                    foreach (var renderer in renderers)
                    {
                        foreach (Material mat in renderer.materials)
                        {
                            mat.SetFloat("_dissolve", dissolveVal);
                        }
                    }
                }

                else if (dissolveVal >= 1f)
                {
                    dissolveVal = 0;
                    cantEvanesco = true;
                    Destroy(item.gameObject);
                }
            }
        }
    }
}