using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Sirenix.OdinInspector;


public class GroqTranscriber : MonoBehaviour
{
    private const string _guidance = "Dont mention button with Ask AI label. Donw mention black background. Answer in no more than 250 characters. Describe what you see on the picture by answering on this question - ";
    public string Result;
    public bool IsReady() => isTranscriptionAvailable;

    public string whisperEndpoint = "https://api.groq.com/openai/v1/audio/transcriptions";
    public string apiKey = "YOUR_API_KEY_HERE";
    public MicRecorder recorder;
    
    private bool isTranscriptionAvailable;


    [Button]
    public void TranscribeAudio()
    {
        StartCoroutine(Transcribe(recorder.GetLastFilePath(), OnTranscription));
    }

    private IEnumerator Transcribe(string filePath, System.Action<string> onResult)
    {
        byte[] audioData = File.ReadAllBytes(filePath);

        WWWForm form = new WWWForm();
        form.AddField("model", "whisper-large-v3");
        form.AddField("language", "en");
        form.AddField("prompt", "Transcribe the following audio");
        form.AddBinaryData("file", audioData, "audio.wav", "audio/wav");

        UnityWebRequest request = UnityWebRequest.Post(whisperEndpoint, form);
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Transcription failed: " + request.error);
            onResult?.Invoke(null);
        }
        else
        {
            string json = request.downloadHandler.text;
            string transcript = ParseTranscription(json);
            onResult?.Invoke(transcript);
        }
    }

    string ParseTranscription(string json)
    {
        int start = json.IndexOf("\"text\":\"") + 8;
        int end = json.IndexOf("\"", start);
        return json.Substring(start, end - start);
    }

    void OnTranscription(string text)
    {
        Result = _guidance + text;
        isTranscriptionAvailable = !string.IsNullOrEmpty(text);
        Debug.Log("<color=green>Groq Transcription: " + text + "</color>");
    }
}