using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using ThunderRoad;

namespace LegacyOfMagic.Modules.ContainerContents
{
    public class SpellBookContainerContent : ContentCustomData
    {
        public string referenceId = "DADAContent";
        public bool hasBeenRevealed;
        public string revealedSpellName;
        public SpellBook type;
        public bool active;

        public void AddCustomContentData(Item item)
        {
            item.contentCustomData.Add(this);
        }
    }
}