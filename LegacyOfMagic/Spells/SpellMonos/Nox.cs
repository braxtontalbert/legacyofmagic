using LegacyOfMagic.Management;
using ThunderRoad;
using UnityEngine;
namespace LegacyOfMagic.Spells.SpellMonos
{
    class Nox : Spell
    {
        void Start()
        {
            usedWand = GetComponent<Item>();
            Cast();
        }

        public override void Cast()
        {
            Lumos active = usedWand.GetComponent<Lumos>();

            if (active && active.spell)
            {
                foreach (AudioSource c in usedWand.GetComponentsInChildren<AudioSource>())
                {
                    switch (c.name)
                    {
                        case "NoxSound":
                            c.Play();
                            break;
                    }
                }
                PlayerControl.GetHand(usedWand.mainHandler.side).StopHapticLoop(ModEntry.local.activeHandlers[usedWand]);
                ModEntry.local.activeHandlers.Remove(usedWand);
                active.spell.Despawn();
            }
        }
    }
}