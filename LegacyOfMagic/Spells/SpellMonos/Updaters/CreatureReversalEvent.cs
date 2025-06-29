using ThunderRoad;
using UnityEngine;

namespace LegacyOfMagic.Spells.SpellMonos.Updaters
{
    public class CreaturesReversalEvent : MonoBehaviour
    {
        private Creature creature;


        private void Start()
        {
            creature = GetComponentInParent<Creature>();
            creature.OnKillEvent += Target_OnKillEvent;
        }

        private void Target_OnKillEvent(CollisionInstance collisionInstance, EventTime eventTime)
        {
            creature.animator.speed = 1f;
            creature.locomotion.ClearSpeedModifiers();
            Destroy(this);
        }
        
    }
}