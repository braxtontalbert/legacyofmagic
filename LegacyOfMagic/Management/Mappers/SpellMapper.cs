using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ThunderRoad;

namespace LegacyOfMagic.Management.Mappers
{
    public class SpellMapper
    {
        public string characterID;
        public  List<String> spellTypes;
        public List<String> availableSpellsPerType;
        public string ToJson() => JsonConvert.SerializeObject(this, Catalog.GetJsonNetSerializerSettings());
    }
}