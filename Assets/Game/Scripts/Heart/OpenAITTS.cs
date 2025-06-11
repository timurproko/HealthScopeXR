using System;
using Cysharp.Threading.Tasks;
using OpenAI;
using OpenAI.Audio;
using Sirenix.OdinInspector;
using UnityEngine;


public class OpenAITTS : MonoBehaviour
{
    [SerializeField] private GroqVision _vision;
    [SerializeField] private AudioSource _audioSource;
    
    private OpenAIClient _api;

    private void Awake()
    {
        InitAsync().Forget();
    }

    private async UniTask InitAsync()
    {
        await Authenticate();
    }

    private async UniTask Authenticate()
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, ".openai");
        string json;

#if UNITY_ANDROID && !UNITY_EDITOR
    using var www = UnityEngine.Networking.UnityWebRequest.Get(path);
    await www.SendWebRequest();
    if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
    {
        Debug.LogError("Failed to load OpenAI config from StreamingAssets");
        return;
    }
    json = www.downloadHandler.text;
#else
        json = System.IO.File.ReadAllText(path);
#endif

        // Crude inline parsing
        string apiKey = null;
        string org = null;

        foreach (var line in json.Replace("{", "").Replace("}", "").Replace("\"", "").Split(','))
        {
            var kv = line.Split(':');
            if (kv.Length == 2)
            {
                var key = kv[0].Trim();
                var value = kv[1].Trim();

                if (key.Equals("apiKey", StringComparison.OrdinalIgnoreCase))
                    apiKey = value;
                else if (key.Equals("organization", StringComparison.OrdinalIgnoreCase))
                    org = value;
            }
        }

        if (string.IsNullOrEmpty(apiKey))
        {
            Debug.LogError("Missing apiKey in .openai config");
            return;
        }

        OpenAIAuthentication auth = string.IsNullOrEmpty(org)
            ? new OpenAIAuthentication(apiKey)
            : new OpenAIAuthentication(apiKey, org);

        _api = new OpenAIClient(auth);
    }

    [Button]
    public async UniTask GenerateSpeech()
    {
        var api = new OpenAIClient();
        var request = new SpeechRequest(_vision.Result);
        var speechClip = await api.AudioEndpoint.GetSpeechAsync(request);
        _audioSource.PlayOneShot(speechClip);
        Debug.Log(speechClip);
    }
}