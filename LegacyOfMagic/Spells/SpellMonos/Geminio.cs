using System.Collections;
using LegacyOfMagic.Management;
using ThunderRoad;
using UnityEngine;

namespace LegacyOfMagic.Spells.SpellMonos
{
    public class Geminio : Spell
    {
        ItemData copyData;
        private GameObject sfx;

        public void Start()
        {
            usedWand = GetComponent<Item>();
            Cast();
        }

        public override void Cast()
        {
            CastRay();
        }

        private void CastRay()
        {
            RaycastHit hit;
            GameObject parent;

            if (Physics.Raycast(usedWand.flyDirRef.transform.position, usedWand.flyDirRef.transform.forward, out hit, float.MaxValue, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                parent = hit.collider.gameObject;
                if (parent.GetComponentInParent<Item>() is Item item)
                {
                    usedWand.mainHandler.playerHand.ragdollHand.PlayHapticClipOver(new AnimationCurve(new []{new Keyframe(0,0), new Keyframe(1, 2)}), 0.3f);
                    copyData = item.data;
                    copyData.SpawnAsync(copy => {
                        copy.IgnoreItemCollision(item);
                        copy.transform.position = item.transform.position;
                        copy.transform.rotation = item.transform.rotation;
                        float x = Random.Range(-3f, 3f);
                        float y = Random.Range(0.01f, 3f);
                        float z = Random.Range(-3f, 3f);

                        if (!sfx)
                        {
                            sfx = Instantiate(ModEntry.local.geminioPop);
                            sfx.transform.position = copy.transform.position;
                        }
                        else
                        {
                            sfx.transform.position = copy.transform.position;
                            sfx.gameObject.GetComponent<AudioSource>().Play();
                        }

                        Vector3 randomVector = new Vector3(x, y, z);
                        copy.physicBody.rigidBody.AddForce((Vector3.up + randomVector.normalized) * 5f * copy.physicBody.mass, ForceMode.Impulse);
                        GameManager.local.StartCoroutine(Timer(copy, item));
                    });
                }
            }
        }

        IEnumerator Timer(Item copy, Item original)
        {
            yield return new WaitForSeconds(1f);
            copy.IgnoreItemCollision(original, false);
        }
    }
}