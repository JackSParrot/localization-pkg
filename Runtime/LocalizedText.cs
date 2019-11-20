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

        void Awake()
        {
            _text = GetComponent<Text>();
            _tmpText = GetComponent<TextMeshProUGUI>();
            if(_text == null && _tmpText == null)
            {
                SharedServices.GetService<ICustomLogger>()?.LogError("Added a LocalizedText component to a gameobject with no ui text");
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
            }
            StartCoroutine(updateText());
        }

        IEnumerator updateText()
        {
            var service = SharedServices.GetService<ILocalizationService>();
            while (service == null || !service.Initialized)
            {
                service = SharedServices.GetService<ILocalizationService>();
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
            StopAllCoroutines();
            enabled = false;
        }
    }
}