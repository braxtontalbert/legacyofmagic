using System.Collections;
using System.Collections.Generic;
using LegacyOfMagic.Management;
using LegacyOfMagic.Spells.VFX;
using ThunderRoad;
using UnityEngine;
using UnityEngine.VFX;

namespace LegacyOfMagic.Spells.SpellMonos
{
    public class Flagrate : Spell
    {
        private List<List<Vector3>> points = new List<List<Vector3>>();
        private bool recordPoints;
        private int iteration = 0;
        private int lastIteration = 0;
        private bool updatedIteration;
        private float maxDistance = 0.05f;
        private GameObject writing;
        private GameObject flagratePoint;
        public override void Start()
        {
            usedWand = GetComponent<Item>();
            usedWand.OnHeldActionEvent += HeldActionEvent;
            Cast();
            base.Start();
        }
        
        IEnumerator AddWritingVFXToPool()
        {
            yield return new WaitForSeconds(0.1f);
            Catalog.InstantiateAsync(ModEntry.flagrateWritingEffect,
                usedWand.flyDirRef.transform.position, usedWand.flyDirRef.transform.rotation, null,
                callback =>
                {
                    writing = callback;
                    VFXPool.local.flagrateWritingVfxPool.Enqueue(writing);
                }, "FlagrateWrite");
        }
        IEnumerator AddPointVFXToPool()
        {
            yield return new WaitForSeconds(0.1f);
            Catalog.InstantiateAsync(ModEntry.flagratePointEffect,
                usedWand.flyDirRef.transform.position, usedWand.flyDirRef.transform.rotation, null,
                callback =>
                {
                    flagratePoint = callback;
                    VFXPool.local.flagratePointVfxPool.Enqueue(flagratePoint);
                }, "FlagratePoint");
        }
        public override void Cast()
        {
            GameManager.local.StartCoroutine(AddPointVFXToPool());
            base.Cast();
        }

        private void HeldActionEvent(RagdollHand ragdollhand, Handle handle, Interactable.Action action)
        {
            if (action == Interactable.Action.AlternateUseStart)
            {
                recordPoints = true;
            }
        }

        new void Update()
        {
            if (flagratePoint != null)
            {
                flagratePoint.transform.position = usedWand.transform.position;
            }
            if (recordPoints)
            {
                if (points.IsNullOrEmpty())
                {
                    points.Add(new List<Vector3>());
                    if (iteration == lastIteration)
                    {
                        
                    }
                }
                else if (lastIteration != iteration)
                {
                    points.Add(new List<Vector3>());
                    lastIteration = iteration;
                }

                List<Vector3> pointList = points[iteration];
                if (ShouldAddPoint(usedWand.flyDirRef.transform.position))
                {
                    pointList.Add(usedWand.flyDirRef.transform.position);
                    
                }
            }
            else
            {
                if (lastIteration == iteration) iteration+=1;
            }
        }
        
        private bool ShouldAddPoint(Vector3 point)
        {
            return points.Count == 0 || Vector3.Distance(points[iteration][points.Count - 1], point) <= maxDistance;
        }
    }
}