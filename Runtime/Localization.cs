using System.Collections.Generic;

namespace JackSParrot.Services.Localization
{
    public class Localization
    {
        public Dictionary<string, string> Localized = new Dictionary<string, string>();

        public Localization()
        {

        }

        public Localization(JSON.JSONObject obj)
        {
            FromJSON(obj);
        }

        public void FromJSON(JSON.JSONObject obj)
        {
            Localized = obj.ToStringDictionary();
        }

        public JSON.JSONObject ToJSON()
        {
            return new JSON.JSONObject(Localized);
        }

        public string GetString(string key)
        {
            return Localized.TryGetValue(key, out string value) ? value : key;
        }
    }
}

