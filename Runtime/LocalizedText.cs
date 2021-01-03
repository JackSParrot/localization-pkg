using UnityEngine;
using UnityEngine.UI;
using JackSParrot.Utils;
using TMPro;

namespace JackSParrot.Services.Localization
{
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] private string _key = string.Empty;
        private Text _text;
        private TMP_Text _tmpText;
        private ILocalizationService _service = null;

        void Start()
        {
            _text = GetComponent<Text>();
            _tmpText = GetComponent<TextMeshProUGUI>();
            if (_text == null && _tmpText == null)
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
            }
            _service = SharedServices.GetService<ILocalizationService>();
            if (_service == null)
            {
                _service = new LocalLocalizationService();
                _service.Initialize(UpdateText);
                SharedServices.RegisterService<ILocalizationService>(_service);
            }
            _service.OnLocalizationChanged += UpdateText;
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
