using System;
using System.Threading.Tasks;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;

public class ChatGptService
{
    private readonly OpenAIAPI _openAiApi;

    public ChatGptService(string apiKey)
    {
        _openAiApi = new OpenAIAPI(apiKey);
    }

    public async Task<string> GetResponseFromChatGpt(string userMessage)
    {
        var systemMessage = "You are a helpful assistant that provides search results.";
        var userPrompt = $"User: {userMessage}\nAssistant: MAXTokens을 200으로 제한했으니까 간단하고 명확하게 설명해줘.";

        var messages = new[]
        {
            new ChatMessage(ChatMessageRole.System, systemMessage),
            new ChatMessage(ChatMessageRole.User, userPrompt)
        };

        var chatRequest = new ChatRequest
        {
            Model = Model.ChatGPTTurbo, // GPT-3.5 터보 모델 사용
            Messages = messages,
            Temperature = 0.7,
            MaxTokens = 200 // MaxTokens 값을 200으로 증가
        };

        var response = await _openAiApi.Chat.CreateChatCompletionAsync(chatRequest);

        if (response != null && response.Choices.Count > 0)
        {
            return response.Choices[0].Message.Content.Trim();
        }
        else
        {
            throw new Exception("Error performing search.");
        }
    }
}
