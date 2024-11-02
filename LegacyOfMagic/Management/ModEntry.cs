using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Speech.Recognition;
using System.Threading.Tasks;
using LegacyOfMagic.Spells;
using ThunderRoad;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace LegacyOfMagic.Management
{
    public class ModEntry : CustomData
    {
        public static ModEntry local;

        internal  Dictionary<Item, object> activeHandlers = new Dictionary<Item, object>();
        //Tracking
        public List<Creature> levicorpusedCreatures = new List<Creature>();
        public List<GameObject> floaters = new List<GameObject>();
        public List<Item> currentTippers = new List<Item>();
        public Dictionary<Creature, float[]> creaturesFOV = new Dictionary<Creature,float[]>();
        public List<Item> currentlyHeldWands = new List<Item>();
        public List<Type> spellsOnPlayer = new List<Type>();
        public List<Type> finiteSpells = new List<Type>();
        
        //vfx
        public Material evanescoDissolveMat;
        public Material dissimuloDissolveMat;
        public GameObject highlighter;
        public Material selectorMat;
        public GameObject incendioEffect;
        public GameObject bubbleHeadEffect;
        public GameObject impedimentaEffect;
        public GameObject imperioShown;
        public GameObject stupefySparks;
        public GameObject expelliarmusSparks;
        public GameObject petrificusSparks;
        public GameObject avadaSparks;
        public GameObject levicorpusSparks;
        public GameObject tarantallegraSparks;
        public GameObject flipendoSparks;
        public GameObject sectumsempraSparks;
        public GameObject leviosoSparks;
        public GameObject evertestatumSparks;
        public GameObject wingardiumLeviosaEffect;
        public GameObject imperioEffect;
        public GameObject depulsoEffect;
        public GameObject explosion;
        public GameObject avadaTest;
        public GameObject crucioEffect;

        private Choices choices;
        
        //SOUNDFX
        public GameObject impedimentaSoundFX;
        public GameObject freezeSFX;
        public GameObject geminioPop;
        
        //SPEECH RECOGNITION STUFF
        GrammarBuilder findServices;
        SpeechRecognitionEngine recognizer;
        public string knownCurrent;
        Dictionary<string, Type> spellDict = new Dictionary<string, System.Type>();
        Item paramItem;
        public bool dissimuloActive;
        public GameObject activeDisillusion;
        public List<Material[]> originalCreatureMaterial = new List<Material[]>();
        
        public override void OnCatalogRefresh()
        {
            if (local != null) return;
            local = this;
            AsyncSetup();
        }
        
        async void AsyncSetup() {

            await Task.Run(() =>
            {
                List<ItemData> itemDatas = Catalog.GetDataList<ItemData>();
                choices = new Choices();
                choices.Add("Stupefy");
                choices.Add("Expelliarmus");
                choices.Add("Avada Kedavra");
                choices.Add("Everte Statum");
                choices.Add("Flipendo");
                choices.Add("Levicorpus");
                choices.Add("Levioso");
                choices.Add("Confringo");
                choices.Add("Morsmordre");
                choices.Add("Petrificus Totalus");
                choices.Add("Tarantallegra");
                choices.Add("Ascendio");
                choices.Add("ArrestoMomentum");
                choices.Add("Depulso");
                choices.Add("Dissimulo");
                choices.Add("Dissimulare");
                choices.Add("Accio");
                choices.Add("Wingardium Leviosa");
                choices.Add("Lumos");
                choices.Add("Nox");
                choices.Add("Protego");
                choices.Add("Sectumsempra");
                choices.Add("Liberacorpus");
                choices.Add("Impedimenta");
                choices.Add("Evanesco");
                choices.Add("Engorgio");
                choices.Add("Reducio");
                choices.Add("Imperio");
                choices.Add("Geminio");
                parseItemWeapons(itemDatas);
                
                try
                {
                    recognizer = new SpeechRecognitionEngine();
                    Grammar servicesGrammar = new Grammar(new GrammarBuilder(choices));
                    recognizer.RequestRecognizerUpdate();
                    recognizer.LoadGrammarAsync(servicesGrammar);
                    recognizer.SetInputToDefaultAudioDevice();
                    recognizer.RecognizeAsync(RecognizeMode.Multiple);
                    recognizer.SpeechRecognized += Recognizer_SpeechRecognized;
                    Application.quitting += () => Process.GetCurrentProcess().Kill();
                    foreach (string micName in Microphone.devices)
                    {
                        Debug.Log("Default Microphone is: " + micName);
                    }
                    Debug.Log("HPLOM Loaded Recognition");
                }
                catch (PlatformNotSupportedException ex)
                {
                    Debug.Log("HPLOM Platform Not Supported Error is: " + ex.Message);
                }
                catch (Exception e)
                {
                    Debug.Log("All other exception: " + e);
                }
            });
        }

        public override IEnumerator LoadAddressableAssetsCoroutine()
        {
            Catalog.LoadAssetAsync<GameObject>("apoz123Wand.SpellEffect.Sparks.Stupefy", callback => { stupefySparks = callback; Debug.Log(callback);}, "StupefySparks");
            Catalog.LoadAssetAsync<GameObject>("apoz123Wand.SpellEffect.Sparks.Expelliarmus", callback => { expelliarmusSparks = callback; }, "ExpelliarmusSparks");
            Catalog.LoadAssetAsync<GameObject>("apoz123Wand.SpellEffect.Sparks.AvadaKedavra", callback => { avadaSparks = callback; }, "AvadaKedavraSparks");
            Catalog.LoadAssetAsync<GameObject>("apoz123.SpellEffect.Explosion", callback => { explosion = callback;}, "ExplosionVisualEffect");
            Catalog.LoadAssetAsync<GameObject>("apoz123Wand.SpellEffect.Sparks.Evertestatum", callback => {evertestatumSparks = callback;}, "EvertestatumSparks");
            Catalog.LoadAssetAsync<GameObject>("apoz123Wand.SpellEffect.Sparks.Flipendo", callback => { flipendoSparks = callback; }, "FlipendoSparks");
            Catalog.LoadAssetAsync<GameObject>("apoz123Wand.SpellEffect.Sparks.Levicorpus", callback => { levicorpusSparks = callback; }, "LevicorpusSparks");
            Catalog.LoadAssetAsync<GameObject>("apoz123Wand.SpellEffect.Sparks.PetrificusTotalus", callback => { petrificusSparks = callback; }, "PetrificusTotalusSparks");
            Catalog.LoadAssetAsync<GameObject>("apoz123Wand.SpellEffect.Sparks.Levioso", callback => { leviosoSparks = callback; }, "LeviosoSparks");
            Catalog.LoadAssetAsync<GameObject>("apoz123Wand.SpellEffect.Sparks.Tarantallegra", callback => { tarantallegraSparks = callback; }, "TarantallegraSparks");
            Catalog.LoadAssetAsync<GameObject>("apoz123Wand.SpellEffect.Depulso",callback => {depulsoEffect = callback; Debug.Log(callback);}, "DepulsoEffect");
            Catalog.LoadAssetAsync<Material>("apoz123Wand.SpellEffect.Evanesco.Mat", callback => { evanescoDissolveMat = callback; }, "Evanesco");
            Catalog.LoadAssetAsync<Material>("apoz123Wand.SpellEffect.Dissimulo.Mat", callback => { dissimuloDissolveMat = callback; }, "Dissimulo");
            Catalog.LoadAssetAsync<GameObject>("apoz123Wand.SpellEffect.Sparks.Sectumsempra", callback => { sectumsempraSparks = callback; }, "SectumsempraSparks");
            Catalog.LoadAssetAsync<GameObject>("apoz123Wand.SpellEffect.SFX.PetrificusFreeze", callback =>{freezeSFX = callback;}, "FreezeSFX");
            Catalog.LoadAssetAsync<GameObject>("apoz123Wand.SpellEffect.Impedimenta",callback => { impedimentaEffect = callback;}, "ImpedimentaEffect");
            Catalog.LoadAssetAsync<GameObject>("apoz123Wand.SoundEffect.Impedimenta",callback => { impedimentaSoundFX = callback;}, "ImpedimentaSoundEffect");
            Catalog.LoadAssetAsync<GameObject>("apoz123Wand.SpellEffect.ImperioHidden",callback => { imperioEffect = callback;}, "ImperioEffect");
            Catalog.LoadAssetAsync<GameObject>("apoz123Wand.SpellEffect.ImperioShown",callback => { imperioShown = callback;}, "ImperioVisibleEffect");
            Catalog.LoadAssetAsync<GameObject>("apoz123Wand.SpellEffect.SFX.GeminioPop",callback => { geminioPop = callback;}, "GeminioPopSfx");
            
            
            return base.LoadAddressableAssetsCoroutine();
        }
        
        String accioString = "Accio";
        private void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            String result = e.Result.Text;
            Debug.Log(result);
            if (e.Result.Confidence > 0.93f && currentlyHeldWands.Count > 0)
            {
                Debug.Log("HPLOM Recognized: " + result);
                foreach (var wand in currentlyHeldWands)
                {
                    String resultParsed;
                    String extraData;
                    if (!result.Contains(accioString + " "))
                    {
                        
                        resultParsed = result.Replace(" ", "");
                    }
                    else
                    {
                        string[] delimitedList = result.Split(' ');
                        resultParsed = delimitedList[0];
                        extraData = delimitedList[1];
                    }
                    try
                    {
                        if (wand.gameObject.GetComponent<CastSpell>() is CastSpell castSpell)
                        {
                            castSpell.Activate(resultParsed, wand, extraData : null);
                        }
                        else
                        {
                            CastSpell spellCastInstance = wand.gameObject.AddComponent<CastSpell>();
                            spellCastInstance.Activate(resultParsed, wand, extraData : null);
                        }
                    }
                    catch (Exception exception)
                    {

                        Debug.Log("Spell threw an error when casting: " + exception.Message);
                    }
                }
            }
            else{
                Debug.Log("HPLOM Recognition Confidence: " + e.Result.Confidence 
                                                           + " --- Confidence must be greater than 0.93 to register. " +
                                                           "Try speaking more clearly into your microphone or bringing " +
                                                           "it closer to your face.");
            }
        }
        
        private void parseItemWeapons(List<ItemData> itemDatas)
        {
            choices.Add(accioString + " Weapon");
            foreach (ItemData data in itemDatas)
            {
                if (data.type == ItemData.Type.Weapon && data.displayName != null && data.category != null)
                {
                    string displayName = data.displayName.ToLower();
                    string categoryName = "";
                    if (data.category.EndsWith("s"))
                    {
                        categoryName = data.category.Remove(data.category.Length - 1).ToLower();
                    }
                    else categoryName = data.category.ToLower();

                    if (displayName.Contains(categoryName))
                    {
                        choices.Add(accioString + " " + categoryName.ToLower());
                    }
                    else
                    {
                        string[] allNames = displayName.Split(' ');
                        for (int i = 0; i < allNames.Length; i++)
                        {
                            choices.Add(accioString + " " + allNames[i].ToLower());
                        }
                        choices.Add(accioString + " " + displayName.ToLower());
                    }
                }
            }
        }
    }
}