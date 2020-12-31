using UnityEngine;
using System.Collections.Generic;
using JackSParrot.Utils;

namespace JackSParrot.Services.Localization
{
    public interface ILocalizationService : System.IDisposable
    {
        event System.Action OnLocalizationChanged;
        bool Initialized { get; }
        void Initialize(System.Action onFinished);
        void SetLanguage(SystemLanguage newLanguage);
        string GetLocalizedString(string key);
    }

    public class LocalLocalizationService : ILocalizationService
    {
        public event System.Action OnLocalizationChanged = delegate() { };
        private Dictionary<SystemLanguage, string> LocalizationFiles = new Dictionary<SystemLanguage, string>
        {
            {SystemLanguage.Spanish, "es"},
            {SystemLanguage.English, "en"},
            {SystemLanguage.French,  "fr"},
            {SystemLanguage.German,  "ge"},
            {SystemLanguage.Italian, "it"}
        };

        private SystemLanguage _defaultLanguage;
        private SystemLanguage _chosenLanguage;
        private Localization _currentLocalization;
        public bool Initialized { get; private set; }

        public LocalLocalizationService(SystemLanguage defaultLanguage = SystemLanguage.English)
        {
            _defaultLanguage = defaultLanguage;
            _chosenLanguage = Application.systemLanguage;
            _currentLocalization = new Localization();
            Initialized = false;
        }

        public void SetLanguage(SystemLanguage newLanguage)
        {
            _chosenLanguage = newLanguage;
            Initialize();
        }

        public void Initialize(System.Action onFinishedLoading = null)
        {
            Initialized = false;
            var language = _chosenLanguage;
            if(!LocalizationFiles.ContainsKey(language))
            {
                language = _defaultLanguage;
                SharedServices.GetService<ICustomLogger>()?.LogError("Chosen language is not available in localization files");
            }
            string fileName = "localization_" + LocalizationFiles[language];
            TextAsset content = Resources.Load<TextAsset>(fileName);
            if(content == null)
            {
                SharedServices.GetService<ICustomLogger>()?.LogError("Can not load the localization file. Trying to load default localization file.");
                fileName = "localization_" + LocalizationFiles[_defaultLanguage];
                content = Resources.Load<TextAsset>(fileName);
                if(content == null)
                {
                    SharedServices.GetService<ICustomLogger>()?.LogError("Can not load the default localization file.");
                    onFinishedLoading?.Invoke();
                    return;
                }
            }
            try
            {
                _currentLocalization.FromJSON(JSON.JSON.LoadString(content.text));
            }
            catch(System.Exception)
            {
                SharedServices.GetService<ICustomLogger>()?.LogError("Error parsing localization file: " + fileName);
            }
            Initialized = true;
            OnLocalizationChanged();
            onFinishedLoading?.Invoke();
        }

        public string GetLocalizedString(string key)
        {
            return _currentLocalization.GetString(key);
        }

        public void Dispose()
        {
            _defaultLanguage = SystemLanguage.English;
            _currentLocalization = new Localization();
            Initialized = false;
        }
    }
}
