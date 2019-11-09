using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using JackSParrot.Utils;

namespace JackSParrot.Services.Localization
{
    [RequireComponent(typeof(Text))]
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField]
        string _key = string.Empty;
        Text _text;

        void Awake()
        {
            _text = GetComponent<Text>();
            if(string.IsNullOrEmpty(_key))
            {
                _key = _text.text;
            }
            StartCoroutine(updateText());
        }

        IEnumerator updateText()
        {
            while(!SharedServices.GetService<ILocalizationManager>().Initialized)
            {
                yield return new WaitForSeconds(1.0f);
            }
            _text.text = SharedServices.GetService<ILocalizationManager>().GetLocalizedString(_key);
            StopAllCoroutines();
            enabled = false;
        }
    }
}