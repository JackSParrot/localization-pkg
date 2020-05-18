using UnityEngine;
using UnityEditor;
using System.IO;
using JackSParrot.JSON;
using UnityEngine.Networking;
using System;

public class UpdateLocalizationWindow : EditorWindow
{
    [MenuItem("Localization/Download Latest")]
    static void Init()
    {
        UpdateLocalizationWindow window = (UpdateLocalizationWindow)GetWindow(typeof(UpdateLocalizationWindow));
        if(PlayerPrefs.HasKey("LOCALIZATION_URL"))
        {
            window.url = PlayerPrefs.GetString("LOCALIZATION_URL");
        }
        window.Show();
    }

    string url = "";
    string status = "";
    UnityWebRequest request = null;
    void OnGUI()
    {
        GUILayout.Label("Localization Updater", EditorStyles.boldLabel);
        url = EditorGUILayout.TextField("Server URL", url);
        if(!string.IsNullOrEmpty(url))
        {
            if(!string.IsNullOrEmpty(status))
            {
                GUILayout.Label(status);
            }
            else if (GUILayout.Button("Download"))
            {
                DownloadLatest();
            }
        }
    }

    void DownloadLatest()
    {
        PlayerPrefs.SetString("LOCALIZATION_URL", url);
        status = "Connecting to server";
        var downloadHandler = new DownloadHandlerBuffer();

        request = new UnityWebRequest(url);
        request.method = UnityWebRequest.kHttpVerbGET;
        request.useHttpContinue = false;
        request.redirectLimit = 50;
        request.timeout = 60;
        request.downloadHandler = downloadHandler;

        request.SendWebRequest().completed += UpdateLocalizationWindow_completed; 
    }

    void UpdateLocalizationWindow_completed(AsyncOperation obj)
    {
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogError("Network error: " + request.downloadHandler.text);
            status = "Network error: " + request.downloadHandler.text;
        }
        else
        {
            JSONObject response = null;
            try
            {
                response = JSON.LoadString(request.downloadHandler.text);
            }
            catch (Exception e)
            {
                Debug.LogError("Localization parse error: " + e.Message);
            }
            status = "";
            if (response != null)
            {
                SaveLocalization(response["localization"].AsObject());
            }
        }
    }

    void SaveLocalization(JSONObject localizations)
    {
        string localizationFolder = Path.Combine(Application.dataPath, "Resources");
        if(!Directory.Exists(localizationFolder))
        {
            Directory.CreateDirectory(localizationFolder);
        }
        string localizationPath = Path.Combine(localizationFolder, "localization_{0}.json");
        foreach (var kvp in localizations)
        {
            var path = string.Format(localizationPath, kvp.Key);
            File.WriteAllText(path, kvp.Value.ToString());
            Debug.Log(string.Format("Updated Localization {0} at {1}", kvp.Key, path));
        }
        status = "Localization files saved to " + localizationFolder;
        AssetDatabase.Refresh();
    }
}
