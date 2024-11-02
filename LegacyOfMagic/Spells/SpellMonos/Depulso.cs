using System;
using System.Collections.Generic;
using LegacyOfMagic.Management;
using ThunderRoad;
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
                Functionality();
            }
            catch (NullReferenceException e)
            {
                Debug.Log("Depulso Exception Handled with message: + " + e);
            }
        }

        void Functionality()
        {
            try
            {
                if (!cachedDepulso)
                {
                    cachedDepulso = Instantiate(ModEntry.local.depulsoEffect);
                }

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
                        if (collider.GetComponentInParent<Creature>() is Creature creature && !creature.isPlayer &&
                            !destabilizedList.Contains(creature))
                        {
                            creature.ragdoll.SetState(Ragdoll.State.Destabilized);
                            destabilizedList.Add(creature);
                        }

                        if (collider.attachedRigidbody is Rigidbody rigidbody && !rigidbody.isKinematic)
                        {
                            if (collider.attachedRigidbody.gameObject.layer == GameManager.GetLayer(LayerName.Ragdoll))
                            {

                                if (collider.attachedRigidbody.gameObject.GetComponent<RagdollPart>() is RagdollPart
                                        ragdollPart && destabilizedList.Contains(ragdollPart.ragdoll.creature))
                                {
                                    Vector3 direction = ragdollPart.transform.position - currentItemPos;
                                    float distance = Vector3.Distance(usedWand.flyDirRef.position,
                                        ragdollPart.transform.position);
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
        }
    }
}