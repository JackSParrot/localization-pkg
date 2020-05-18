using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using JackSParrot.Utils;
using TMPro;

namespace JackSParrot.Services.Localization
{
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField]
        string _key = string.Empty;
        Text _text;
        TextMeshProUGUI _tmpText;
        TextMeshPro _tmpTextPro;

        void Start()
        {
            _text = GetComponent<Text>();
            _tmpText = GetComponent<TextMeshProUGUI>();
            _tmpTextPro = GetComponent<TextMeshPro>();
            if (_text == null && _tmpText == null && _tmpTextPro == null)
            {
                Debug.LogError("Added a LocalizedText component to a gameobject with no ui text");
                return;
            }
            if(string.IsNullOrEmpty(_key))
            {
                if(_text != null)
                {
                    _key = _text.text;
                }
                else if(_tmpText != null)
                {
                    _key = _tmpText.text;
                }
                else if(_tmpTextPro != null)
                {
                    _key = _tmpTextPro.text;
                }
            }
            StartCoroutine(updateText());
        }

        IEnumerator updateText()
        {
            var service = SharedServices.GetService<ILocalizationService>();
            if(service == null)
            {
                service = new LocalLocalizationService();
                service.Initialize(() => Debug.Log("LocalizationService Initialized"));
                SharedServices.RegisterService<ILocalizationService>(service);
            }
            while (!service.Initialized)
            {
                yield return new WaitForSeconds(1.0f);
            }
            if(_text != null)
            {
                _text.text = service.GetLocalizedString(_key);
            }
            else if(_tmpText != null)
            {
                _tmpText.text = service.GetLocalizedString(_key);
            }
            else if(_tmpTextPro != null)
            {
                _tmpTextPro.text = service.GetLocalizedString(_key);
            }
            StopAllCoroutines();
            enabled = false;
        }
    }
}
