using System.Collections;
using LegacyOfMagic.Spells.SpellMonos.Updaters;
using ThunderRoad;
using UnityEngine;
namespace LegacyOfMagic.Spells.SpellMonos
{
    public class Morsmordre : Spell
    {
        private ItemData darkMark;
        public void Start() {

            spell = GetComponent<Item>();

            darkMark = Catalog.GetData<ItemData>("TheDarkMark");
            GameManager.local.StartCoroutine(Timer());
        }


        IEnumerator Timer() {
            
            yield return new WaitForSeconds(2.5f);

            darkMark.SpawnAsync(projectile => {

                projectile.gameObject.AddComponent<DarkMark>();
                projectile.transform.position = spell.transform.position;
                projectile.physicBody.rigidBody.useGravity = false;
                projectile.physicBody.rigidBody.drag = 0.0f;

                spell.Despawn();

                foreach (var creature in Creature.allActive)
                {
                    if(!creature.isPlayer && !creature.isKilled) creature.brain.instance.GetModule<BrainModuleFear>().Panic();
                }

            });

        }
    }
}