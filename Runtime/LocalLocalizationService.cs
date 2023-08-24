using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace JackSParrot.Services.Localization
{
    public abstract class ALocalizationService : AService
    {
        public Action OnLocalizationChanged;
        
        public bool Initialized { get; protected set; }
        public abstract void   SetLanguage(SystemLanguage newLanguage);
        public abstract string GetLocalizedString(string  key);
    }

    public class LocalALocalizationService : ALocalizationService
    {
        private Dictionary<SystemLanguage, string> LocalizationFiles = new Dictionary<SystemLanguage, string>
        {
            {SystemLanguage.Spanish, "es"},
            {SystemLanguage.English, "en"},
            {SystemLanguage.French,  "fr"},
            {SystemLanguage.German,  "de"},
            {SystemLanguage.Italian, "it"}
        };

        private SystemLanguage _defaultLanguage;
        private SystemLanguage _chosenLanguage;
        private Localization _currentLocalization;
        

        public LocalALocalizationService(SystemLanguage defaultLanguage = SystemLanguage.English)
        {
            _defaultLanguage = defaultLanguage;
            _chosenLanguage = Application.systemLanguage;
            _currentLocalization = new Localization();
            Initialized = false;
        }

        public override void SetLanguage(SystemLanguage newLanguage)
        {
            _chosenLanguage = newLanguage;
            Initialize();
        }

        public override List<Type> GetDependencies()
        {
            return new List<Type>();
        }

        public override IEnumerator Initialize()
        {
            Initialized = false;
            var language = _chosenLanguage;
            if(!LocalizationFiles.ContainsKey(language))
            {
                language = _defaultLanguage;
                Debug.LogError("Chosen language is not available in localization files");
            }
            string fileName = "localization_" + LocalizationFiles[language];
            TextAsset content = Resources.Load<TextAsset>(fileName);
            if(content == null)
            {
                Debug.LogError("Can not load the localization file. Trying to load default localization file.");
                fileName = "localization_" + LocalizationFiles[_defaultLanguage];
                content = Resources.Load<TextAsset>(fileName);
                if(content == null)
                {
                    Debug.LogError("Can not load the default localization file.");
                    yield break;
                }
            }
            try
            {
                _currentLocalization.FromJSON(JSON.JSON.LoadString(content.text));
            }
            catch(System.Exception)
            {
                Debug.LogError("Error parsing localization file: " + fileName);
            }
            Initialized = true;
            OnLocalizationChanged();
        }

        public override string GetLocalizedString(string key)
        {
            return _currentLocalization.GetString(key);
        }

        public override void Cleanup()
        {
            _defaultLanguage = SystemLanguage.English;
            _currentLocalization = new Localization();
            Initialized = false;
        }
    }
}
