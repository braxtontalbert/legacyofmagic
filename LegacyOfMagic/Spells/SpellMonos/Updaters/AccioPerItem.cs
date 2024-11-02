using UnityEngine;
using ThunderRoad;
namespace LegacyOfMagic.Spells.SpellMonos.Updaters
{
    public class AccioPerItem : MonoBehaviour
    {
        public bool cantAccio;
        private float elapsedTime;
        private Vector3 endPoint;
        private RagdollHand oppositeHand;
        private Vector3 startPoint;
        private string componentLevel;
        private Item pulled;
        private Item wand;

        public void AddWand(Item wand)
        {
            this.wand = wand;
            oppositeHand = wand.mainHandler.otherHand;
        }
        void Start() {

            cantAccio = false;
            pulled = GetComponent<Item>();

        }

        void Update()
        {
            if (!cantAccio)
            {
                float distanceSqr = (pulled.gameObject.transform.position - oppositeHand.transform.position).magnitude;

                if (distanceSqr < 0.2f)
                {
                    cantAccio = true;
                    foreach (var hand in pulled.handles)
                    {
                        if (hand)
                        {
                            oppositeHand.playerHand.ragdollHand.Grab(hand);
                            break;
                        }   
                    }
                    pulled.physicBody.rigidBody.useGravity = true;
                    pulled.physicBody.rigidBody.velocity = new Vector3(0, 0, 0);
                }
                else
                {
                    pulled.physicBody.rigidBody.useGravity = false;
                    var position = oppositeHand.transform.position;
                    var position1 = pulled.transform.position;
                    pulled.physicBody.rigidBody.velocity = (position - position1).normalized * 15f;
                }
            }
        }
    }
}