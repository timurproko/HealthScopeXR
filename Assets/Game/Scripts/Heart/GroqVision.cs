using System;
using System.Collections;
using System.Text;
using System.Text.Json;
using UnityEngine;
using UnityEngine.Networking;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class GroqVision : MonoBehaviour
{
    public bool IsReady() => isResultAvailable;
    public string Result;

    [SerializeField] private GroqTranscriber _transcriber;
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private int captureWidth = 512;
    [SerializeField] private int captureHeight = 512;
    [SerializeField] private string apiKey = "your_api_key_here";
    [SerializeField] private string model = "gpt-4-vision-preview";
    
    private string endpoint = "https://api.groq.com/openai/v1/chat/completions";
    private readonly string _request = "Describe this image in details, be specific on what is represented, don't describe in general. Focusing on anatomical structures and any notable visual features of the human heart. Use medical terminology.";
    private bool isResultAvailable;

    [Button]
    public void RequestVision()
    {
        StartCoroutine(SendVisionRequest());
    }

    private IEnumerator SendVisionRequest()
    {
        RenderTexture renderTexture = new RenderTexture(captureWidth, captureHeight, 24);
        sceneCamera.targetTexture = renderTexture;
        sceneCamera.Render();

        RenderTexture.active = renderTexture;
        Texture2D texture2D = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
        texture2D.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
        texture2D.Apply();

        sceneCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        byte[] imageBytes = texture2D.EncodeToJPG();
        string base64Image = Convert.ToBase64String(imageBytes);

        string jsonPayload = JsonSerializer.Serialize(new
        {
            model = model,
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = new object[]
                    {
                        new { type = "text", text = _request },
                        new
                        {
                            type = "image_url",
                            image_url = new
                            {
                                url = "data:image/jpeg;base64," + base64Image
                            }
                        }
                    }
                }
            },
            max_tokens = 300
        });

        byte[] postData = Encoding.UTF8.GetBytes(jsonPayload);

        UnityWebRequest request = new UnityWebRequest(endpoint, "POST");
        request.uploadHandler = new UploadHandlerRaw(postData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Groq Vision API Error: " + request.error);
        }
        else
        {
            string json = request.downloadHandler.text;
            string content = ParseResponse(json);
            Result = content;
            isResultAvailable = !string.IsNullOrEmpty(Result);
            Debug.Log("<color=green>Groq Vision: " + content + "</color>");

        }
    }

    private string ParseResponse(string json)
    {
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;

        if (root.TryGetProperty("choices", out var choices) &&
            choices.GetArrayLength() > 0 &&
            choices[0].TryGetProperty("message", out var message) &&
            message.TryGetProperty("content", out var content))
        {
            return content.GetString();
        }

        return "No content in response.";
    }
}
