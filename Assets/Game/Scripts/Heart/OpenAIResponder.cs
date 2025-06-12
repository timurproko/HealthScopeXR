using System.Collections.Generic;
using OpenAI;
using OpenAI.Chat;
using Sirenix.OdinInspector;
using UnityEngine;

public class OpenAIResponder : MonoBehaviour
{
    public bool IsReady() => isResultAvailable;
    public string Result;

    [SerializeField] private OpenAITTS _openAITTS;
    [SerializeField] private GroqTranscriber _transcriber;
    [SerializeField] private GroqVision _vision;
    
    private OpenAIClient client;
    private bool isResultAvailable;

    private void Start()
    {
        client = _openAITTS._api;
    }

    [Button]
    public async void AskChatGPT()
    {
        isResultAvailable = false;

        var chatRequest = new ChatRequest(new List<Message>
        {
            new Message(Role.System, "You are medical specialist who specializes on human body, you are assisting the user who ask questions."),
            new Message(Role.System, "Respond with no more than 250 characters."),
            new Message(Role.System, $"This is description of what user is looking at: {_vision.Result}"),
            new Message(Role.System, $"Answer directly to user question and use other information as supportive to understand the context"),
            new Message(Role.User, $"My question is: {_transcriber.Result}")
        });

        var response = await client.ChatEndpoint.GetCompletionAsync(chatRequest);
        Result = response.FirstChoice.Message.Content.ToString();
        isResultAvailable = true;
        Debug.Log("<color=green>ChatGPT: " + response.FirstChoice.Message.Content + "</color>");
    }
}