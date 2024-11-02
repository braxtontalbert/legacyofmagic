using LegacyOfMagic.Management;
using LegacyOfMagic.Spells;
using ThunderRoad;

namespace LegacyOfMagic.Modules
{
    public class WandModule : ItemModule
    {
        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.gameObject.AddComponent<CastSpell>();
            Item.OnItemGrab += ( arg1,  arg2,  arg3) =>
            {
                if(arg1.Equals(item) && !ModEntry.local.currentlyHeldWands.Contains(arg1)) ModEntry.local.currentlyHeldWands.Add(arg1);
            };
            Item.OnItemUngrab += (arg1, arg2, arg3) =>
            {
                ModEntry.local.currentlyHeldWands.Remove(arg1);
            };
        }
    }
}