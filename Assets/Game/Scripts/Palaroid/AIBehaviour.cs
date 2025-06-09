using System;
using System.Collections.Generic;
using Atomic.Entities;
using Cysharp.Threading.Tasks;
using Game;
using Oculus.Interaction;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;
using UnityEngine;
using UnityEngine.UI;

public class AIBehaviour : IInit, IUpdate
{
    private static readonly Content Request =
        "What do you see in this image? Give answer in no more than 80 characters.";

    private OpenAIClient _api;
    private InteractableUnityEventWrapper _AIButton;
    private IEntity _entity;
    private Text _text;

    public void Init(in IEntity entity)
    {
        _entity = entity;
        _text = entity.GetDescription();
        _AIButton = entity.GetButtonAI();
        _AIButton.WhenSelect.AddListener(SendSnapshotAsync);
        InitAsync().Forget();
    }

    private async UniTask InitAsync()
    {
        await Authenticate();
    }

    private void SendSnapshotAsync()
    {
        SendSnapshot(_entity.GetSnapshot()).Forget();
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

    private async UniTask ListModels()
    {
        var models = await _api.ModelsEndpoint.GetModelsAsync();

        foreach (var model in models)
        {
            Debug.Log(model.ToString());
        }
    }

    private async UniTask SendSnapshot(Texture2D snapshot)
    {
        if (snapshot == null) return;
        _entity.SetIsAIWorking(true);
        
        var messages = new List<Message>
        {
            new Message(Role.User, new List<Content>
            {
                Request,
                snapshot
            })
        };

        var chatRequest = new ChatRequest(messages, model: Model.GPT4o);
        var result = await _api.ChatEndpoint.GetCompletionAsync(chatRequest);
        object message =
            $"GPT Response: {result.FirstChoice.Message.Content} | Finish Reason: {result.FirstChoice.FinishDetails}";

        _entity.GetDescription().text = result.FirstChoice.Message.Content.ToString();
        // Debug.Log(message);
        _entity.SetIsAIWorking(false);
        _entity.GetFinishedEvent().Invoke();
    }

    private async UniTask SendHelloWorld()
    {
        try
        {
            var messages = new[]
            {
                new Message(Role.User, "Say hello to the world.")
            };

            var chatRequest = new ChatRequest(
                messages: messages,
                model: "gpt-3.5-turbo"
            );

            var response = await _api.ChatEndpoint.GetCompletionAsync(chatRequest);

            Debug.Log("GPT Response: " + response.FirstChoice.Message.Content);
        }
        catch (Exception ex)
        {
            Debug.LogError("Chat request failed: " + ex);
        }
    }

    public void OnUpdate(in IEntity entity, in float deltaTime)
    {
        if (!entity.GetIsCaptured())
        {
            _text.text = "";
        }
    }
}