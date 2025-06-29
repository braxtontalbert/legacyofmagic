using System;
using System.Collections.Generic;
using System.Linq;
using LegacyOfMagic.Management;
using LegacyOfMagic.Modules.ContainerContents;
using ThunderRoad;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

namespace LegacyOfMagic.Modules
{
    public enum SpellBook
    {
        DADA,
        MMT,
        APM,
        TDA
    }
    public class SpellBookInstance : MonoBehaviour
    {
        public List<String> availableSpells;
        public bool hasSpellBeenRevealed;
        public string spellNameAssigned;
        private Item item;
        private SpellBookContainerContent content;
        public SpellBook spellBookType;
        public Dictionary<string, string> counterCursesToLearnWithSpell = new Dictionary<string, string>();

        public void AddByType()
        {
            switch (spellBookType)
            {
                case SpellBook.DADA:
                    AddAllDADASpells(availableSpells);
                    break;
                case SpellBook.MMT:
                    AddAllMagicalTheorySpells(availableSpells);
                    break;
                case SpellBook.TDA:
                    AddAllDarkArtsSpells(availableSpells);
                    break;
            }
        }
        public void AddAllDADASpells(List<String> listToAdd)
        {
            listToAdd.Add("Stupefy");
            listToAdd.Add("Expelliarmus");
            listToAdd.Add("PetrificusTotalus"); 
            listToAdd.Add("EverteStatum"); 
            listToAdd.Add("Flipendo"); 
            listToAdd.Add("Confringo");; 
            listToAdd.Add("Tarantallegra");
            listToAdd.Add("Protego"); 
            listToAdd.Add("Impedimenta"); 
            listToAdd.Add("Incendio");
        }
        public void AddAllMagicalTheorySpells(List<String> listToAdd)
        { 
            listToAdd.Add("Levioso"); 
            listToAdd.Add("Ascendio"); 
            listToAdd.Add("ArrestoMomentum"); 
            listToAdd.Add("Depulso"); 
            listToAdd.Add("Dissimulo");
            listToAdd.Add("Accio"); 
            listToAdd.Add("WingardiumLeviosa"); 
            listToAdd.Add("Lumos");
            listToAdd.Add("Evanesco"); 
            listToAdd.Add("Engorgio"); 
            listToAdd.Add("Reducio"); 
            listToAdd.Add("Geminio"); 
        }

        public void AddAllDarkArtsSpells(List<String> listToAdd)
        {
            listToAdd.Add("AvadaKedavra");
            listToAdd.Add("Crucio");
            listToAdd.Add("Imperio");
        }

        public void Start()
        {
            item = GetComponent<Item>();
            counterCursesToLearnWithSpell.Add("Lumos", "Nox");
            counterCursesToLearnWithSpell.Add("Levicorpus", "Liberacorpus");
            counterCursesToLearnWithSpell.Add("Dissimulo", "Dissimulare");
            EventManager.onLevelUnload += Unload;

            var vfxs = item.gameObject.GetComponentsInChildren<VisualEffect>().ToList();
            Debug.Log(vfxs.Count);
            foreach (var VARIABLE in vfxs)
            {
                VARIABLE.Stop();
            }
            if (item.TryGetCustomData(out SpellBookContainerContent spellBookContainerContent))
            {
                Debug.Log("On Instance start content container: " + spellBookContainerContent.revealedSpellName);
                hasSpellBeenRevealed = spellBookContainerContent.hasBeenRevealed;
                spellNameAssigned = spellBookContainerContent.revealedSpellName;
                content = spellBookContainerContent;
            }
        }

        private void Unload(LevelData leveldata, LevelData.Mode mode, EventTime eventtime)
        {
            if (eventtime == EventTime.OnStart && this.content != null)
            {
                this.content.active = false;
            }
        }

        public void SetContent()
        {
            if (content == null) content = new SpellBookContainerContent();
            this.content.hasBeenRevealed = this.hasSpellBeenRevealed;
            this.content.revealedSpellName = this.spellNameAssigned;
            this.content.type = this.spellBookType;
            if (item.contentCustomData == null) item.contentCustomData = new List<ContentCustomData>();
            item.contentCustomData.Add(content);
        }

        public String ReturnRandomFromAvailable()
        {
            if (availableSpells.IsNullOrEmpty())
            {
                availableSpells = new List<string>();
                AddByType();
                GameModeSpellManager.local.CrossCheckAvailableAndCurrentAndRemoveFromAvailable(availableSpells, GameModeSpellManager.local.currentSpellsForCharacter);
            }
            else GameModeSpellManager.local.CrossCheckAvailableAndCurrentAndRemoveFromAvailable(availableSpells, GameModeSpellManager.local.currentSpellsForCharacter);
            var randValue = Random.Range(0, availableSpells.Count - 1);
            
            var valueToReturn = availableSpells[randValue];
            availableSpells.Remove(valueToReturn);
            Debug.Log(valueToReturn);
            return valueToReturn;
        }
        
    }
}