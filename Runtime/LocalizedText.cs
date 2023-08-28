using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JackSParrot.Services.Localization
{
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] private string _key = string.Empty;
        private Text _text;
        private TMP_Text _tmpText;
        private ALocalizationService _service = null;

        IEnumerator Start()
        {
            _text = GetComponent<Text>();
            _tmpText = GetComponent<TextMeshProUGUI>();
            if (_text == null && _tmpText == null)
            {
                Debug.LogError("Added a LocalizedText component to a gameobject with no ui text");
                yield break;
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
            _service = ServiceLocator.GetService<ALocalizationService>();
            _service.OnLocalizationChanged += UpdateText;
            UpdateText();
        }

        private void OnDestroy()
        {
            if(_service != null)
            {
                _service.OnLocalizationChanged -= UpdateText;
            }
        }

        void UpdateText()
        {
            if (_text != null)
            {
                _text.text = _service.GetLocalizedString(_key);
            }
            else if (_tmpText != null)
            {
                _tmpText.text = _service.GetLocalizedString(_key);
            }
        }
    }
}
