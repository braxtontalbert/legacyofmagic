using System;
using System.Collections;
using ThunderRoad;
using UnityEngine;
using UnityEngine.VFX;

namespace LegacyOfMagic
{
    public class Spell : MonoBehaviour
    {
        public Item spell;
        public Item usedWand;
        private GameObject cachedSparksEffect;
        private String extraData;

        public void AddExtraData(String extraData)
        {
            this.extraData = extraData;
        }

        public String getExtraData()
        {
            return extraData;
        }
        public virtual void Cast(){}
        public virtual void SpawnSparks(GameObject effect, Vector3 position)
        {
            GameManager.local.StartCoroutine(SpawnSparksHidden(effect, position));
        }

        private IEnumerator SpawnSparksHidden(GameObject effect, Vector3 position)
        {
            if (!cachedSparksEffect)
            {
                effect.transform.position = position;
                cachedSparksEffect = Instantiate(effect);
            }
            else
            {
                ParticleSystem system = cachedSparksEffect.GetComponentInChildren<ParticleSystem>();
                cachedSparksEffect.transform.position = position;
                system.Play();
            }
            yield return null;
        }
    }
}