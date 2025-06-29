using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using LegacyOfMagic.Management;
using LegacyOfMagic.Spells.VFX;
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
        public bool spellActive;
        private bool constantCast = false;


        public Transform pos0;
        public Transform pos1;
        public Transform pos2;
        public Transform pos3;
        public Transform followTransform;
        

        public GameObject cachedCastEffect;
        public VisualEffect activeCast;


        private void OnDestroy()
        {
            EventManager.onLevelUnload -= UnLoad;
        }

        public virtual void Start()
        {
            EventManager.onLevelUnload += UnLoad;
        }

        private void UnLoad(LevelData leveldata, LevelData.Mode mode, EventTime eventtime)
        {
            if (eventtime == EventTime.OnEnd)
            {
                VFXPool.local.ClearVfxPools();
            }
        }

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
        

        protected IEnumerator CastSpellEffect(VisualEffect vfx)
        {
            var audioSource = cachedCastEffect.GetComponentInChildren<AudioSource>();
            audioSource.Play();
            float ageOverLifeTime = 0;
            float elapsedTime = 0f;
            vfx.SetFloat("lifetime", 0f);
            while (ageOverLifeTime < 1)
            {
                elapsedTime += Time.deltaTime;

                ageOverLifeTime = Mathf.Clamp(elapsedTime / 0.2f, 0f, 1f);
                vfx.SetFloat("lifetime", ageOverLifeTime);
                yield return null;
            }
            if(!constantCast) vfx.Stop();
        }

        public virtual void ExecuteAfterInstantiate()
        {
            
        }
        public virtual void ExecuteIfCached()
        {
            
        }

        IEnumerator InstantiateVFX(String effectReference, Vector3 position)
        {
            Catalog.InstantiateAsync(effectReference, usedWand.flyDirRef.transform.position, usedWand.flyDirRef.transform.rotation, null, callback =>
            {
                cachedCastEffect = callback;
                VFXPool.local.GetPoolByReference(effectReference).Enqueue(cachedCastEffect);
                Transform[] transforms = cachedCastEffect.gameObject.GetComponentsInChildren<Transform>();

                pos0 = transforms[2];
                pos1 = transforms[3];
                pos2 = transforms[4];
                pos3 = transforms[5];

                var transform1 = usedWand.flyDirRef.transform;
                var position1 = transform1.position;
                pos0.transform.position = position1;
                var forward = transform1.forward;
                pos1.transform.position = position1 + (forward * (position - position1).magnitude * 0.33f);
                pos2.transform.position = position1 + (forward * (position - position1).magnitude * 0.66f);
                pos3.transform.position = position;
            
                activeCast = cachedCastEffect.gameObject.GetComponentInChildren<VisualEffect>();
                    
            }, effectReference+"Handler");
            yield return new WaitUntil(() => activeCast != null);
            ExecuteAfterInstantiate();
        }
        protected IEnumerator SetupCast(Vector3 position, [CanBeNull] String effectReference)
        {
            if (VFXPool.local.GetPoolByReference(effectReference).IsNullOrEmpty() || (cachedCastEffect && cachedCastEffect.GetComponentInChildren<VisualEffect>().aliveParticleCount > 0))
            {
                yield return new WaitForSeconds(0.1f);
                GameManager.local.StartCoroutine(InstantiateVFX(effectReference, position));
            }
            else
            {
                if (!VFXPool.local.GetPoolByReference(effectReference).IsNullOrEmpty())
                {
                    if (VFXPool.local.GetPoolByReference(effectReference).Dequeue() is GameObject go && go  && !go.Equals(cachedCastEffect))
                    {
                        cachedCastEffect = go;
                        activeCast = cachedCastEffect.GetComponentInChildren<VisualEffect>();
                        VFXPool.local.GetPoolByReference(effectReference).Enqueue(cachedCastEffect);
                        
                        Transform[] transforms = cachedCastEffect.gameObject.GetComponentsInChildren<Transform>();

                        pos0 = transforms[2];
                        pos1 = transforms[3];
                        pos2 = transforms[4];
                        pos3 = transforms[5];
                        
                        var transform2 = usedWand.flyDirRef.transform;
                        cachedCastEffect.transform.position = transform2.position;
                        var position1 = transform2.position;
                        pos0.transform.position = position1;
                        var forward = transform2.forward;
                        pos1.transform.position = position1 + (forward * (position - position1).magnitude * 0.33f);
                        pos2.transform.position = position1 + (forward * (position - position1).magnitude * 0.66f);
                        pos3.transform.position = position;
                        activeCast.Play();
                        ExecuteIfCached();
                        yield break;
                    }
                    VFXPool.local.GetPoolByReference(effectReference).Enqueue(cachedCastEffect);
                    yield return new WaitForSeconds(0.1f);
                    GameManager.local.StartCoroutine(InstantiateVFX(effectReference, position));
                }
            }
        }

        public virtual void Update()
        {
            if (followTransform && activeCast && activeCast.aliveParticleCount > 0)
            {
                pos0.position = usedWand.flyDirRef.transform.position;
                pos3.position = followTransform.position;
            }
        }
        
        public Creature GetCreaturesWithinAngleAndDistance(Vector3 origin, Vector3 direction, float angle, float distance)
        {
            List<Creature> detectedCreatures = new List<Creature>();

            foreach (Creature creature in Creature.allActive)
            {
                Debug.Log(creature);
                if (creature == null || creature.isPlayer || creature.isKilled) continue;

                // Calculate the vector from the origin to the creature
                Vector3 toCreature = creature.ragdoll.targetPart.transform.position - origin;

                // Check if within distance
                if (toCreature.sqrMagnitude > distance * distance)
                {
                    continue; // Skip if creature is beyond the specified distance
                }

                // Normalize direction and calculate the angle between the forward direction and the creature's direction
                Vector3 normalizedDirection = direction.normalized;
                Vector3 normalizedToCreature = toCreature.normalized;
                float angleToCreature = Vector3.Angle(normalizedDirection, normalizedToCreature);

                // Check if within the specified angle
                if (angleToCreature <= angle / 2f)
                {
                    detectedCreatures.Add(creature);
                }
            }

            Creature toReturn = null;
            foreach (var returner in detectedCreatures)
            {
                Vector3 toCreature = returner.ragdoll.targetPart.transform.position - origin;
                if (toReturn)
                {
                    if (toCreature.sqrMagnitude < (toReturn.ragdoll.targetPart.transform.position - origin).sqrMagnitude)
                    {
                        toReturn = returner;
                    }
                }
                else
                {
                    toReturn = returner;
                }
            }

            return toReturn;
        }
        
        public Item GetItemsWithinAngleAndDistance(Vector3 origin, Vector3 direction, float angle, float distance)
        {
            Item closestItem = null;
            float closestDistance = distance * distance;  // Use squared distance for efficiency
            float angleThreshold = Mathf.Cos(angle * Mathf.Deg2Rad / 2f);  // Cosine threshold for dot product

            foreach (Item item in Item.allActive)
            {
                Vector3 toItem = item.transform.position - origin;
                float sqrDistanceToItem = toItem.sqrMagnitude;

                // Check if within distance
                if (sqrDistanceToItem > closestDistance)
                {
                    continue;
                }

                // Check if within angle using dot product
                Vector3 normalizedToItem = toItem.normalized;
                float dotProduct = Vector3.Dot(direction.normalized, normalizedToItem);

                if (dotProduct >= angleThreshold)
                {
                    // If within angle, update closest item
                    if (closestItem == null || sqrDistanceToItem < closestDistance)
                    {
                        closestItem = item;
                        closestDistance = sqrDistanceToItem;  // Update closest distance
                    }
                }
            }

            return closestItem;
        }
    }
}