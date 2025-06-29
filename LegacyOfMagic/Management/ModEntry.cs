using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Speech.Recognition;
using System.Threading.Tasks;
using LegacyOfMagic.Spells;
using LegacyOfMagic.Spells.SpellMonos;
using ThunderRoad;
using ThunderRoad.DebugViz;
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
        public GameObject incendioHiddenEffect;
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
        public GameObject depulsoEffect;
        public GameObject explosion;
        public GameObject avadaTest;
        public GameObject crucioEffect;

        public const String wingardiumCastEffect = "apoz123Wand.SpellEffect.CastSpell.WingardiumLeviosa";
        public const String engorgioCastEffect = "apoz123Wand.SpellEffect.CastSpell.Engorgio";
        public const String imperioEffect = "apoz123Wand.SpellEffect.Cast.Imperio";
        public const String evanescoCastEffect = "apoz123Wand.SpellEffect.CastSpell.Evanesco";
        public const String crucioCastEffect = "apoz123Wand.SpellEffect.CastSpell.Crucio";
        public const String geminioCastEffect = "apoz123Wand.SpellEffect.CastSpell.Geminio";
        public const String flagratePointEffect = "apoz123Wand.SpellEffect.Flagrate";
        public const String flagrateWritingEffect = "apoz123Wand.SpellEffect.FlagrateWriting";


        //DADABook textures
        public Texture stupefyText;
        public Texture expelliarmusTexture;
        public Texture petrificusTotalusTexture;
        public Texture confringoTexture;
        public Texture tarantallegraTexture;
        public Texture impedimentaTexture;
        public Texture incendioTexture;
        public Texture everteStatumTexture;
        public Texture flipendoTexture;
        public Texture protegoTexture;
        
        //MMTBook textures
        public Texture AccioTexture;
        public Texture ArrestoMomentumTexture;
        public Texture AscendioTexture;
        public Texture DissiumlareTexture;
        public Texture DissimuloTexture;
        public Texture EngorgioTexture;
        public Texture EvanescoTexture;
        public Texture GeminioTexture;
        public Texture WingardiumLeviosaTexture;
        public Texture LeviosoTexture;
        public Texture LumosTexture;
        public Texture NoxTexture;
        public Texture ReducioTexture;
        public Texture AvadaTexture;
        public Texture CrucioTexture;
        public Texture ImperioTexture;
        
        //crucio sfx
        public GameObject crucioStartEffect;
        public GameObject crucioLoopEffect;
        public GameObject crucioEndEffect;
        
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

        public IEnumerator SetupRecognizer(Choices choices)
        {
            if (recognizer != null)
            {
                recognizer.SpeechRecognized -= Recognizer_SpeechRecognized;
                recognizer = null;
            }
            try
            {
                recognizer = new SpeechRecognitionEngine();
                Grammar servicesGrammar = new Grammar(new GrammarBuilder(choices));
                recognizer.RequestRecognizerUpdate();
                recognizer.LoadGrammarAsync(servicesGrammar);
                recognizer.SetInputToDefaultAudioDevice();
                recognizer.RecognizeAsync(RecognizeMode.Multiple);
                recognizer.SpeechRecognized += Recognizer_SpeechRecognized;
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

            yield return null;
        }
        async void AsyncSetup() {

            await Task.Run(() =>
            {
                //List<ItemData> itemDatas = Catalog.GetDataList<ItemData>();
                //parseItemWeapons(itemDatas);
                Application.quitting += () => Process.GetCurrentProcess().Kill();
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
            Catalog.LoadAssetAsync<GameObject>("apoz123Wand.SpellEffect.SFX.GeminioPop",callback => { geminioPop = callback;}, "GeminioPopSfx");
            Catalog.LoadAssetAsync<GameObject>("apoz123Wand.SpellEffect.Incendio", callback =>{incendioEffect = callback;}, "IncendioEffect");
            Catalog.LoadAssetAsync<GameObject>("apoz123Wand.SpellEffect.IncendioHidden", callback =>{incendioHiddenEffect = callback;}, "IncendioHiddenEffect");
            Catalog.LoadAssetAsync<GameObject>("apoz123Wand.SpellEffect.CastSpell.Crucio.CrucioStartSFX", callback =>{crucioStartEffect = callback;}, "CrucioStartSFX");
            Catalog.LoadAssetAsync<GameObject>("apoz123Wand.SpellEffect.CastSpell.Crucio.CrucioLoopSFX", callback =>{crucioLoopEffect = callback;}, "CrucioLoopSFX");
            Catalog.LoadAssetAsync<GameObject>("apoz123Wand.SpellEffect.CastSpell.Crucio.CrucioEndSFX", callback =>{crucioEndEffect = callback;}, "CrucioEndSFX");
            Catalog.LoadAssetAsync<Texture>("stupefyTextSDF", callback =>{stupefyText = callback;}, "StupefySDF");
            Catalog.LoadAssetAsync<Texture>("expelliarmusTextSDF", callback =>{expelliarmusTexture = callback;}, "ExpelliarmusSDF");
            Catalog.LoadAssetAsync<Texture>("petrificusTotalusTextSDF", callback =>{petrificusTotalusTexture = callback;}, "PetrificusTotalusSDF");
            Catalog.LoadAssetAsync<Texture>("everteStatumTextSDF", callback =>{everteStatumTexture = callback;}, "EverteStatumSDF");
            Catalog.LoadAssetAsync<Texture>("flipendoTextSDF", callback =>{flipendoTexture = callback;}, "FlipendoSDF");
            Catalog.LoadAssetAsync<Texture>("confringoTextSDF", callback =>{confringoTexture = callback;}, "ConfringoSDF");
            Catalog.LoadAssetAsync<Texture>("protegoTextSDF", callback =>{protegoTexture = callback;}, "ProtegoSDF");
            Catalog.LoadAssetAsync<Texture>("incendioTextSDF", callback =>{incendioTexture = callback;}, "IncendioSDF");
            Catalog.LoadAssetAsync<Texture>("impedimentaTextSDF", callback =>{impedimentaTexture = callback;}, "ImpedimentaSDF");
            Catalog.LoadAssetAsync<Texture>("tarantallegraTextSDF", callback =>{tarantallegraTexture = callback;}, "TarantallegraSDF");
            Catalog.LoadAssetAsync<Texture>("accioTextSDF", callback =>{AccioTexture = callback;}, "AccioSDF");
            Catalog.LoadAssetAsync<Texture>("arrestoMomentumTextSDF", callback =>{ArrestoMomentumTexture = callback;}, "ArrestoMomentumSDF");
            Catalog.LoadAssetAsync<Texture>("ascendioTextSDF", callback =>{AscendioTexture = callback;}, "AscendioSDF");
            Catalog.LoadAssetAsync<Texture>("dissimulareTextSDF", callback => { DissiumlareTexture = callback; }, "DissimulareSDF");
            Catalog.LoadAssetAsync<Texture>("dissimuloTextSDF", callback =>{DissimuloTexture = callback;}, "DissimuloSDF");
            Catalog.LoadAssetAsync<Texture>("engorgioTextSDF", callback =>{EngorgioTexture = callback;}, "EngorgioSDF");
            Catalog.LoadAssetAsync<Texture>("evanescoTextSDF", callback =>{EvanescoTexture = callback;}, "EvanescoSDF");
            Catalog.LoadAssetAsync<Texture>("geminioTextSDF", callback =>{GeminioTexture = callback;}, "GeminioSDF");
            Catalog.LoadAssetAsync<Texture>("wingardiumLeviosaTextSDF", callback =>{WingardiumLeviosaTexture = callback;}, "WingardiumLeviosaSDF");
            Catalog.LoadAssetAsync<Texture>("leviosoTextSDF", callback =>{LeviosoTexture = callback;}, "LeviosoSDF");
            Catalog.LoadAssetAsync<Texture>("lumosTextSDF", callback =>{LumosTexture = callback;}, "LumosSDF");
            Catalog.LoadAssetAsync<Texture>("noxTextSDF", callback =>{NoxTexture = callback;}, "NoxSDF");
            Catalog.LoadAssetAsync<Texture>("reducioTextSDF", callback =>{ReducioTexture = callback;}, "ReducioSDF");
            Catalog.LoadAssetAsync<Texture>("avadaKedavraSDF", callback =>{AvadaTexture = callback;}, "AvadaKedavraSDF");
            Catalog.LoadAssetAsync<Texture>("crucioSDF", callback =>{CrucioTexture = callback;}, "CrucioSDF");
            Catalog.LoadAssetAsync<Texture>("imperioSDF", callback =>{ImperioTexture = callback;}, "ImperioSDF");
            
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
                    String extraData = null;
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
                            castSpell.Activate(resultParsed, wand, extraData);
                        }
                        else
                        {
                            CastSpell spellCastInstance = wand.gameObject.AddComponent<CastSpell>();
                            spellCastInstance.Activate(resultParsed, wand, extraData);
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


        public (string value, Texture stupefyText) ReturnSpellTextureByName(string value)
        {
            switch (value)
            {
                case "Stupefy":
                    return (value,stupefyText);
                case "Expelliarmus":
                    return (value,expelliarmusTexture);
                case "PetrificusTotalus":
                    return (value,petrificusTotalusTexture);
                case "Impedimenta":
                    return (value,impedimentaTexture);
                case "Incendio":
                    return (value,incendioTexture);
                case "Protego":
                    return (value,protegoTexture);
                case "EverteStatum":
                    return (value,everteStatumTexture);
                case "Flipendo":
                    return (value,flipendoTexture);
                case "Confringo":
                    return (value,confringoTexture);
                case "Tarantallegra":
                    return (value,tarantallegraTexture);
                case "Accio":
                    return (value,AccioTexture);
                case "ArrestoMomentum":
                    return (value, ArrestoMomentumTexture);
                case "Ascendio":
                    return (value, AscendioTexture);
                case "Dissimulare":
                    return (value, DissiumlareTexture);
                case "Dissimulo":
                    return (value, DissimuloTexture);
                case "Engorgio":
                    return (value, EngorgioTexture);
                case "Evanesco":
                    return (value, EvanescoTexture);
                case "Geminio":
                    return (value, GeminioTexture);
                case "WingardiumLeviosa":
                    return (value, WingardiumLeviosaTexture);
                case "Levioso":
                    return (value, LeviosoTexture);
                case "Lumos":
                    return (value, LumosTexture);
                case "Nox":
                    return (value, NoxTexture);
                case "Reducio":
                    return (value, ReducioTexture);
                case "AvadaKedavra":
                    return (value, AvadaTexture);
                case "Crucio":
                    return (value, CrucioTexture);
                case "Imperio":
                    return (value, ImperioTexture);
                default:
                    return ("Null",null);
            }
        }
        
        /*private void parseItemWeapons(List<ItemData> itemDatas)
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
        }*/
    }
}