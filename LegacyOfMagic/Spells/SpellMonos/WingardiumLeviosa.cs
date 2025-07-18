﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LegacyOfMagic.Management;
using ThunderRoad;
using UnityEngine;
using UnityEngine.VFX;
using Random = System.Random;

namespace LegacyOfMagic.Spells.SpellMonos
{
    class WingardiumLeviosa : Spell
    {
        internal bool canLift;
        Vector3 direction;
        float distance;
        Rigidbody currentRigidbody;
        Creature currentCreature;
        private Side lastSide;
        private bool executeSphereCast = false;

        private GameObject lastHit;

        private Random random;

        public override void Cast()
        {
            CastRay();
        }

        public override void Start()
        {
            base.Start();
            usedWand = GetComponent<Item>();
            usedWand.OnHeldActionEvent += Item_OnHeldActionEvent;
            Cast();
        }
        private bool usePressed;
        private bool altUsePressed;
        private void Item_OnHeldActionEvent(RagdollHand ragdollHand, Handle handle, Interactable.Action action)
        {
            if (action == Interactable.Action.AlternateUseStart && canLift)
            {
                altUsePressed = true;
            }
            if (action == Interactable.Action.AlternateUseStop && canLift)
            {
                altUsePressed = false;
            }

            if (action == Interactable.Action.UseStart && canLift)
            {
                usePressed = true;
            }
            if (action == Interactable.Action.UseStop && canLift)
            {
                usePressed = false;
            }
        }

        private void CastRay()
        {
            RaycastHit hit;
            GameObject parent;
            bool itemValid = true;
            if (Physics.Raycast(usedWand.flyDirRef.transform.position,usedWand.flyDirRef.transform.forward, out hit, 50f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                parent = hit.collider.gameObject;
                if (parent.GetComponentInParent<Item>() is Item item && !item.IsHeld() && !item.physicBody.rigidBody.isKinematic)
                {
                    if (item.holder && !item.holder.creature)
                    {
                        return;
                    }
                    followTransform = item.transform;
                    GameManager.local.StartCoroutine(ExecuteLiftSetup(parent));
                }
                else if (parent.GetComponentInParent<Creature>() is Creature creature)
                {
                    followTransform = creature.ragdoll.targetPart.transform;
                    GameManager.local.StartCoroutine(ExecuteLiftSetup(parent));
                }
                else
                {
                    ClosestItem(hit);
                }
            }
            else
            {
                ClosestItem(hit);
            }
        }

        void ClosestItem(RaycastHit hit)
        {
            float lastShortestDistance = 0f;
            Item closest = null;
            Collider[] colliders = Physics.OverlapSphere(hit.point, 2f);
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.GetComponentInParent<Item>() is Item item  && !item.IsHeld() && !item.physicBody.rigidBody.isKinematic)
                {
                    if (item.holder && !item.holder.creature)
                    {
                        return;
                    }
                    if (closest == null)
                    {
                        closest = item;
                        lastShortestDistance = Vector3.Distance(closest.transform.position, hit.point);
                    }
                    else
                    {
                        float distance = Vector3.Distance(item.transform.position, hit.point);
                        if (distance < lastShortestDistance)
                        {
                            closest = item;
                            lastShortestDistance = distance;
                        }
                    }
                }
            }

            if (closest && closest.holder && !closest.holder.creature && !closest.IsHeld() && closest.physicBody.rigidBody.isKinematic)
            {
                GameManager.local.StartCoroutine(ExecuteLiftSetup(closest.gameObject));
            }
            else
            {
                executeSphereCast = true;
            }
        }


        private IEnumerator CastSpellEffect(VisualEffect vfx, GameObject hit)
        {
            yield return base.CastSpellEffect(vfx);
            if (hit.GetComponentInParent<Item>() is Item item2 && !item2.IsHeld() && !item2.physicBody.rigidBody.isKinematic)
            {
                if (item2.holder && !item2.holder.creature)
                {
                    yield break;
                }
                currentRigidbody = item2.physicBody.rigidBody;
                canLift = true;
                distance = Math.Abs(Vector3.Distance(currentRigidbody.transform.position, usedWand.flyDirRef.position));
                item2.OnBreakStart += ItemsBrokeStart;

                executeSphereCast = false;
                reset = false;
            }
            else if (hit.GetComponentInParent<Creature>() is Creature creature)
            {
                currentCreature = creature;
                if(currentCreature.ragdoll.state != Ragdoll.State.Frozen) currentCreature.ragdoll.SetState(Ragdoll.State.Destabilized);

                currentRigidbody = currentCreature.ragdoll.targetPart.physicBody.rigidBody;
                canLift = true;
                distance = Math.Abs(Vector3.Distance(currentRigidbody.transform.position, usedWand.flyDirRef.position));
                foreach (var part in currentCreature.ragdoll.parts)
                {
                    if (!part.isSliced)
                    {
                        part.physicBody.useGravity = false;
                    }
                }
                executeSphereCast = false;
                reset = false;
            }
        }
        
        public override void ExecuteAfterInstantiate()
        {
            GameManager.local.StartCoroutine(CastSpellEffect(activeCast, lastHit));
        }

        public override void ExecuteIfCached()
        {
            GameManager.local.StartCoroutine(CastSpellEffect(activeCast, lastHit));
        }
        
        private float sphereCastMax = 50f;
        IEnumerator ExecuteLiftSetup(GameObject hit)
        {
            followTransform = hit.gameObject.transform;
            lastHit = hit.gameObject;
            GameManager.local.StartCoroutine(SetupCast(followTransform.position, ModEntry.wingardiumCastEffect));
            yield return null;
        }

        private void ItemsBrokeStart(Breakable breakable)
        {
            canLift = false;
            currentRigidbody = null;
            PlayerControl.GetHand(lastSide).StopHapticLoop(this);
        }

        private bool reset = true;
        void Update()
        {
            direction = usedWand.flyDirRef.forward;

            if (canLift)
            {
                if (usedWand.mainHandler != null && currentRigidbody)
                {
                    if (lastSide != usedWand.mainHandler.side)
                    {
                        PlayerControl.GetHand(lastSide).StopHapticLoop(this);
                    }
                    lastSide = usedWand.mainHandler.side;
                    
                    currentRigidbody.velocity = ((usedWand.flyDirRef.position + (direction * distance)) - currentRigidbody.position) * (3f);
                    float velocity = currentRigidbody.velocity.magnitude;
                    float ratio = Utils.CalculateRatio(velocity, 0f, 5f,
                        Catalog.gameData.haptics.telekinesisIntensity.x,
                        Catalog.gameData.haptics.telekinesisIntensity.y);
                    float period = Utils.CalculateRatio(velocity, 0f, 5f, Catalog.gameData.haptics.telekinesisPeriod.x,
                        Catalog.gameData.haptics.telekinesisPeriod.y);
                    float intensity =
                        (ratio * Catalog.gameData.haptics.telekinesisMassIntensity.Evaluate(currentRigidbody.mass))
                        .Clamp(0.0f, 1f);
                    PlayerControl.GetHand(lastSide).HapticLoop(this, intensity, period);
                }
                else
                {
                    PlayerControl.GetHand(lastSide).StopHapticLoop(this);
                }
                if (usePressed && altUsePressed && !reset)
                {
                    followTransform = null;
                    canLift = false;
                    currentRigidbody = null;
                    if (currentCreature)
                    {
                        foreach (var part in currentCreature.ragdoll.parts)
                        {
                            part.physicBody.useGravity = true;
                        }

                        currentCreature = null;
                    }
                    PlayerControl.GetHand(lastSide).StopHapticLoop(this);
                    reset = true;
                    executeSphereCast = false;
                    usePressed = false;
                    altUsePressed = false;
                }

                else if (altUsePressed)
                {
                   distance =  Mathf.Clamp(distance + 0.1f, 1f, 30f);
                }
                else if (usePressed)
                {
                    distance = Mathf.Clamp(distance - 0.1f, 1f, 30f);
                }
            }

            if (executeSphereCast)
            {
                for (float distance = sphereCastMax; distance >= 0; distance -= 1.5f)
                {
                    var transform1 = usedWand.flyDirRef.transform;
                    Vector3 checkPoint = transform1.position + transform1.forward * distance;

                    RaycastHit hit;
                    if (Physics.SphereCast(checkPoint, 1.5f, transform1.forward, out hit, 3))
                    {
                        if (hit.collider.gameObject.GetComponentInParent<Item>() is Item item && !item.IsHeld() && !item.physicBody.rigidBody.isKinematic)
                        {
                            if (item.holder && !item.holder.creature)
                            {
                                return;
                            }
                            GameManager.local.StartCoroutine(ExecuteLiftSetup(hit.collider.gameObject));
                        }
                        else if (hit.collider.gameObject.GetComponentInChildren<Creature>())
                        {
                            GameManager.local.StartCoroutine(ExecuteLiftSetup(hit.collider.gameObject));
                        }
                    }
                }
                executeSphereCast = false;
            }
            
            base.Update();
        }
    }
}