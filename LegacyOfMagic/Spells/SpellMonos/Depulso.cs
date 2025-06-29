using System;
using System.Collections;
using System.Collections.Generic;
using LegacyOfMagic.Management;
using ThunderRoad;
using ThunderRoad.Skill.Spell;
using UnityEngine;
using UnityEngine.VFX;

namespace LegacyOfMagic.Spells.SpellMonos
{
    public class Depulso : Spell
    {
        private List<Creature> destabilizedList = new List<Creature>();
        private GameObject cachedDepulso;

        public void Start()
        {
            usedWand = GetComponent<Item>();
            Cast();
        }

        public override void Cast()
        {
            destabilizedList.Clear();
            destabilizedList.TrimExcess();
            Debug.Log("In Cast override method");
            try
            {
                GameManager.local.StartCoroutine(Functionality());
            }
            catch (NullReferenceException e)
            {
                Debug.Log("Depulso Exception Handled with message: + " + e);
            }
        }

        IEnumerator Functionality()
        {
            
            if (!cachedDepulso)
            {
                yield return new WaitForSeconds(0.1f);
                cachedDepulso = Instantiate(ModEntry.local.depulsoEffect);
            }
            try
            {
                    
                cachedDepulso.transform.position = usedWand.flyDirRef.position;
                cachedDepulso.transform.rotation = usedWand.flyDirRef.rotation;
                cachedDepulso.GetComponentInChildren<VisualEffect>().Play();
                usedWand.mainHandler.playerHand.ragdollHand.PlayHapticClipOver(new AnimationCurve(new []{new Keyframe(0,0), new Keyframe(1, 2)}), 0.3f);
                Collider[] colliderArray =
                    Physics.OverlapSphere(usedWand.flyDirRef.position + usedWand.flyDirRef.forward * 2f, 3f);
                Vector3 currentItemPos = usedWand.flyDirRef.transform.position;
                if (colliderArray.Length > 0)
                {
                    foreach (Collider collider in colliderArray)
                    {
                        if (collider.attachedRigidbody is Rigidbody rigidbody)
                        {
                            if (collider.attachedRigidbody.gameObject.layer == GameManager.GetLayer(LayerName.Ragdoll))
                            {
                                if (collider.attachedRigidbody.gameObject.GetComponent<RagdollPart>() is RagdollPart
                                        ragdollPart && !ragdollPart.ragdoll.creature.isPlayer)
                                {
                                    ragdollPart.ragdoll.SetState(Ragdoll.State.Destabilized);
                                    var position = ragdollPart.transform.position;
                                    
                                    Vector3 direction = position - currentItemPos;
                                    float distance = Vector3.Distance(usedWand.flyDirRef.position,
                                        position);
                                    ragdollPart.ragdoll.creature.TryPush(Creature.PushType.Magic,
                                        direction.normalized * 2f,
                                        (int)Mathf.Round(Mathf.Lerp(1f, 3f, Mathf.InverseLerp(2f, 0.0f, distance))));
                                    ragdollPart.physicBody.rigidBody.AddForce(
                                        direction.normalized * ragdollPart.physicBody.rigidBody.mass * 8f,
                                        ForceMode.Impulse);
                                }
                            }
                            else
                            {
                                if (collider.attachedRigidbody.GetComponentInParent<Item>())
                                {
                                    Vector3 direction = collider.attachedRigidbody.transform.position - currentItemPos;
                                    rigidbody.AddForce(direction.normalized * rigidbody.mass * 1.1f, ForceMode.Impulse);
                                }
                            }
                        }
                    }
                }

                foreach (AudioSource c in usedWand.GetComponentsInChildren<AudioSource>())
                {
                    if (c.name == this.GetType().Name)
                    {
                        c.Play();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Where game usually crashes");
            }
            yield return null;
        }
    }
}