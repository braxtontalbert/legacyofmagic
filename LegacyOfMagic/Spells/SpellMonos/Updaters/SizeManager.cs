using System;
using UnityEngine;
using ThunderRoad;
namespace LegacyOfMagic.Spells.SpellMonos.Updaters
{
    public class SizeManager : MonoBehaviour
    {
        private Item item;
        private float currentSizeValue;
        private Vector3 lastSizeVector;
        private readonly float ratio = 2f;
        public bool changeSize;
        public bool direction;
        private float elapsedTime = 0f;
        private bool setLastSizeVector = false;
        public void Start()
        {
            item = GetComponent<Item>();
        }

        private void Update()
        {
            if (changeSize)
            {
                if (!setLastSizeVector)
                {
                    lastSizeVector = item.transform.localScale;
                    setLastSizeVector = true;
                }
                if (!direction)
                {
                    elapsedTime += Time.deltaTime;
                    float percentageComplete = elapsedTime / 0.2f;
                    
                    item.transform.localScale = Vector3.Lerp(lastSizeVector, lastSizeVector / ratio, Mathf.SmoothStep(0, 1, percentageComplete));

                    if (item.transform.localScale == lastSizeVector / ratio)
                    {
                        elapsedTime = 0f;
                        changeSize = false;
                        setLastSizeVector = false;
                    }
                }
                else
                {
                    elapsedTime += Time.deltaTime;
                    float percentageComplete = elapsedTime / 0.2f;
                    
                    item.transform.localScale = Vector3.Lerp(lastSizeVector, lastSizeVector * ratio, Mathf.SmoothStep(0, 1, percentageComplete));

                    if (item.transform.localScale == lastSizeVector * ratio)
                    {
                        elapsedTime = 0f;
                        changeSize = false;
                        setLastSizeVector = false;
                    }
                }
            }
        }
    }
}