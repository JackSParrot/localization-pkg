using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace JackSParrot.Services.Localization
{
    public abstract class ALocalizationService : AService
    {
        public Action OnLocalizationChanged;
        public abstract void   SetLanguage(SystemLanguage newLanguage);
        public abstract string GetLocalizedString(string  key);
    }

    [CreateAssetMenu(fileName = "LocalLocalizationService", menuName = "JackSParrot/Services/LocalLocalizationService")]
    public class LocalLocalizationService : ALocalizationService
    {
        private Dictionary<SystemLanguage, string> LocalizationFiles = new Dictionary<SystemLanguage, string>
        {
            {SystemLanguage.Spanish, "es"},
            {SystemLanguage.English, "en"}
        };

        [SerializeField]
        private SystemLanguage _defaultLanguage;
        private Localization _currentLocalization = new Localization();

        public override void SetLanguage(SystemLanguage newLanguage)
        {
            PlayerPrefs.SetInt("locale", (int)newLanguage);
            Initialize();
        }

        public override List<Type> GetDependencies()
        {
            return new List<Type>();
        }

        public override IEnumerator Initialize()
        {
            SystemLanguage language = (SystemLanguage)PlayerPrefs.GetInt("locale", (int)_defaultLanguage);
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
            Status = EServiceStatus.Initialized;
            OnLocalizationChanged?.Invoke();
        }

        public override string GetLocalizedString(string key)
        {
            return _currentLocalization.GetString(key);
        }

        public override void Cleanup()
        {
            _defaultLanguage     = SystemLanguage.English;
            _currentLocalization = new Localization();
            Status               = EServiceStatus.NotInitialized;
        }
    }
}
