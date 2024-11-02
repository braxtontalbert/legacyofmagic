using System;
using ThunderRoad;
using UnityEngine;

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

        public override void Cast()
        {
            CastRay();
        }

        public void Start()
        {
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

                itemValid = ExecuteLiftSetup(parent);

                if (!itemValid)
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
                if (collider.gameObject.GetComponentInParent<Item>() is Item item)
                {
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

            if (closest)
            {
                ExecuteLiftSetup(closest.gameObject);
            }
            else
            {
                executeSphereCast = true;
            }
        }

        private float sphereCastMax = 50f;
        bool ExecuteLiftSetup(GameObject parent)
        {
            if (parent.GetComponentInParent<Item>() is Item item2)
            {
                currentRigidbody = item2.physicBody.rigidBody;
                canLift = true;
                distance = Math.Abs(Vector3.Distance(currentRigidbody.transform.position, usedWand.flyDirRef.position));
                item2.OnBreakStart += ItemsBrokeStart;

                executeSphereCast = false;
                reset = false;
                return true;
            }
            else if (parent.GetComponentInParent<Creature>() is Creature creature1) {

                currentCreature = creature1;
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
                return true;
            }

            return false;
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

                if (altUsePressed)
                {
                    distance += 0.11f;
                }
                else if (usePressed)
                {
                    distance -= 0.1f;
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
                         ExecuteLiftSetup(hit.collider.gameObject);
                    }
                }

                executeSphereCast = false;
            }
        }
    }
}