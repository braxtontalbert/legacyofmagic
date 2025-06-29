using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using LegacyOfMagic.Management.Mappers;
using Newtonsoft.Json;
using ThunderRoad;
using ThunderRoad.AI.Decorator;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LegacyOfMagic.Management
{
    public class GameModeSpellManager : ThunderScript
    {
        public static GameModeSpellManager local;
        public List<String> currentSpellsForCharacter = new List<string>();
        public List<String> availableSpells = new List<string>();
        private List<String> allSpells = new List<string>();
        private string crystalHuntId = "CrystalHunt";
        private bool waitingForData = false;
        public override void ScriptLoaded(ModManager.ModData modData)
        {
            if (local == null) local = this;
            GameManager.local.StartCoroutine(GetJsonData());
            EventManager.onLevelLoad += LevelLoadEvent;
            EventManager.onLevelUnload += LevelUnloadEvent;
            Application.quitting += OnApplicationQuit;
            AddAllToList(allSpells);
            base.ScriptLoaded(modData);
        }

        private void LevelUnloadEvent(LevelData leveldata, LevelData.Mode mode, EventTime eventtime)
        {
            if (eventtime == EventTime.OnStart)
            {
                GameManager.local.StartCoroutine(this.SaveJsonData(Player.characterData.ID, currentSpellsForCharacter));
            }
        }

        private void OnApplicationQuit()
        {
            GameManager.local.StartCoroutine(this.SaveJsonData(Player.characterData.ID, currentSpellsForCharacter));
        }

        private void LevelLoadEvent(LevelData levelData, LevelData.Mode mode, EventTime eventTime)
        {
            if (eventTime == EventTime.OnStart)
            {
                if(!waitingForData) GameManager.local.StartCoroutine(GetJsonData());
            }
        }

        public IEnumerator SaveJsonData(String characterID, List<String> currentSpells)
        {
            SpellMapper mapper = new SpellMapper();
            mapper.characterID = characterID;
            mapper.spellTypes = currentSpells;

            var json = mapper.ToJson();
            PlatformBase.Save save = new PlatformBase.Save(mapper.characterID, "LOMSpells", json);
            yield return GameManager.platform.WriteSaveCoroutine(save);
        }
        public IEnumerator GetJsonData()
        {
            Debug.Log("waiting until character data exists");
            waitingForData = true;
            yield return new WaitUntil(() => Player.characterData != null);
            waitingForData = false;
            Debug.Log("after character data exists");
            List<PlatformBase.Save> saves = null;
            List<PlatformBase.Save> characterSaves = null;
            yield return GameManager.platform.ReadSavesCoroutine("LOMSpells", value =>
            {
                saves = value;
            });

            yield return GameManager.platform.ReadSavesCoroutine("chr", value => { characterSaves = value; });

            yield return GameManager.local.StartCoroutine(RemoveUnusedCharacterData(characterSaves, saves));

            yield return GameManager.platform.ReadSavesCoroutine("LOMSpells", value =>
            {
                saves = value;
            });

            Debug.Log(saves);
            if (saves.IsNullOrEmpty())
            {
                if (Player.characterData.mode.data.GetGameModeSaveData().gameModeId == crystalHuntId)
                {
                    currentSpellsForCharacter.Add("Revelio");
                    currentSpellsForCharacter.Add("Deletrius");
                }
                else 
                {
                   AddAllToList(currentSpellsForCharacter);
                }
                ModifyChoices();
                yield return SaveJsonData(Player.characterData.ID, currentSpellsForCharacter);
                yield break;
            }
            if (!saves.Select(save => save.id).Contains(Player.characterData.ID)) {
                    if (Player.characterData.mode.data.GetGameModeSaveData().gameModeId == crystalHuntId)
                    {
                        currentSpellsForCharacter.Add("Revelio");
                        currentSpellsForCharacter.Add("Deletrius");
                    }
                    else 
                    {
                        AddAllToList(currentSpellsForCharacter);
                    }
                    ModifyChoices();
                    yield return SaveJsonData(Player.characterData.ID, currentSpellsForCharacter);
                    yield break;
            }
            foreach (var save in saves)
            {
                if (save.id.Equals(Player.characterData.ID))
                {
                    SpellMapper mapper = JsonConvert.DeserializeObject<SpellMapper>(save.data);
                    if (mapper != null)
                    {
                        currentSpellsForCharacter = mapper.spellTypes;
                        availableSpells = mapper.availableSpellsPerType;
                        ModifyChoices();
                        break;
                    }
                }
            }
        }

        public void CrossCheckAvailableAndCurrentAndRemoveFromAvailable(List<string> availableSpellsGiven, List<string> currentSpells)
        {
            for (var i = 0; i < availableSpellsGiven.Count; i++)
            {
                string spellName = availableSpellsGiven[i];
                if (currentSpells.Contains(spellName))
                {
                    availableSpellsGiven.Remove(spellName);
                }
            }
            availableSpellsGiven.TrimExcess();
        }
        void AddAllToList(List<String> listToAdd)
        {
            listToAdd.Add("Stupefy");
            listToAdd.Add("Expelliarmus");
            listToAdd.Add("Avada Kedavra");
            listToAdd.Add("Everte Statum"); 
            listToAdd.Add("Flipendo"); 
            listToAdd.Add("Levicorpus"); 
            listToAdd.Add("Levioso"); 
            listToAdd.Add("Confringo"); 
            listToAdd.Add("Morsmordre"); 
            listToAdd.Add("Petrificus Totalus"); 
            listToAdd.Add("Tarantallegra"); 
            listToAdd.Add("Ascendio"); 
            listToAdd.Add("ArrestoMomentum"); 
            listToAdd.Add("Depulso"); 
            listToAdd.Add("Dissimulo"); 
            listToAdd.Add("Dissimulare"); 
            listToAdd.Add("Accio"); 
            listToAdd.Add("Wingardium Leviosa"); 
            listToAdd.Add("Lumos");
            listToAdd.Add("Nox");
            listToAdd.Add("Protego"); 
            listToAdd.Add("Sectumsempra"); 
            listToAdd.Add("Liberacorpus"); 
            listToAdd.Add("Impedimenta"); 
            listToAdd.Add("Evanesco"); 
            listToAdd.Add("Engorgio"); 
            listToAdd.Add("Reducio"); 
            listToAdd.Add("Imperio"); 
            listToAdd.Add("Geminio"); 
            listToAdd.Add("Incendio"); 
            listToAdd.Add("Crucio");
            listToAdd.Add("Revelio");
            listToAdd.Add("Flagrate");
            listToAdd.Add("Deletrius");
        }

        public void ModifyChoices()
        {
            Choices choices = new Choices();
            foreach (var spell in currentSpellsForCharacter)
            {
                choices.Add(spell);
            }

            GameManager.local.StartCoroutine(ModEntry.local.SetupRecognizer(choices));
        }
        
        IEnumerator RemoveUnusedCharacterData(List<PlatformBase.Save> characterSave, List<PlatformBase.Save> spellSaves)
        {
            var matchingSaves = characterSave.Select(i => i.id).Intersect(spellSaves.Select(i => i.id)).ToList();

            var nonMatchingSaves = new List<PlatformBase.Save>();

            foreach (var spellSave in spellSaves)
            {
                if (!matchingSaves.Contains(spellSave.id))
                {
                    nonMatchingSaves.Add(spellSave);
                }
            }

            yield return GameManager.platform.DeleteSaveCoroutine(nonMatchingSaves.ToArray());
        }
    }
}