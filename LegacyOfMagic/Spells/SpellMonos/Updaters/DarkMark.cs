using System.Collections;
using UnityEngine;
using ThunderRoad;
namespace LegacyOfMagic.Spells.SpellMonos.Updaters
{
    public class DarkMark : MonoBehaviour
    {
        Item item;
        float dissolveVal;
        public void Start() {

            item = GetComponent<Item>();
            dissolveVal = 1;

            foreach (Renderer renderer in item.gameObject.GetComponentsInChildren<Renderer>()) {
                foreach (Material mat in renderer.materials) {
                    mat.SetFloat("_dissolve", dissolveVal);
                }
            
            }

            GameManager.local.StartCoroutine(Timer());

        }

        IEnumerator Timer()
        {
            yield return new WaitForSeconds(20f);
            end = true;
        }

        private bool end = false;
        void Update() {
            item.gameObject.transform.LookAt(Player.local.transform);
            if (dissolveVal > 0 && !end)
            {
                dissolveVal -= 0.01f;

                foreach (Renderer renderer in item.gameObject.GetComponentsInChildren<Renderer>())
                {
                    foreach (Material mat in renderer.materials) {

                        mat.SetFloat("_dissolve",dissolveVal);
                    
                    
                    }
                }
            }
            else if (end)
            {
                if (dissolveVal <= 1)
                {
                    dissolveVal += 0.01f;

                    foreach (Renderer renderer in item.gameObject.GetComponentsInChildren<Renderer>())
                    {
                        foreach (Material mat in renderer.materials)
                        {

                            mat.SetFloat("_dissolve", dissolveVal);


                        }
                    }
                }
                else
                {
                    item.Despawn();
                }
            }
            
        }
    }
}