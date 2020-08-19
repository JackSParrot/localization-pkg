using UnityEngine;
using System.Collections.Generic;
using JackSParrot.Utils;

namespace JackSParrot.Services.Localization
{
    public interface ILocalizationService : System.IDisposable
    {
        bool Initialized { get; }
        void Initialize(System.Action onFinished);
        string GetLocalizedString(string key);
    }

    public class LocalLocalizationService : ILocalizationService
    {
        SystemLanguage _defaultLanguage;
        Dictionary<SystemLanguage, string> LocalizationFiles = new Dictionary<SystemLanguage, string>
        {
            {SystemLanguage.Spanish, "es"},
            {SystemLanguage.English, "en"},
            {SystemLanguage.French,  "fr"},
            {SystemLanguage.German,  "ge"},
            {SystemLanguage.Italian, "it"}
        };

        Localization _currentLocalization;
        public bool Initialized { get; private set; }

        public LocalLocalizationService(SystemLanguage defaultLanguage = SystemLanguage.English)
        {
            _defaultLanguage = defaultLanguage;
            _currentLocalization = new Localization();
            Initialized = false;
        }

        public void Initialize(System.Action onFinishedLoading)
        {
            Initialized = false;
            var language = Application.systemLanguage;
            if(!LocalizationFiles.ContainsKey(language))
            {
                language = _defaultLanguage;
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
                    onFinishedLoading();
                    return;
                }
            }
            _currentLocalization.FromJSON(JSON.JSON.LoadString(content.text));
            Initialized = true;
            onFinishedLoading();
        }

        public string GetLocalizedString(string key)
        {
            return _currentLocalization.GetString(key);
        }

        public void Dispose()
        {
            _defaultLanguage = defaultLanguage;
            _currentLocalization = new Localization();
            Initialized = false;
        }
    }
}
