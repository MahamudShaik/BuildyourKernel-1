using Azure.AI.Agents.Persistent;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents.OpenAI;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenAI.Assistants;
using OpenAI.Chat;
using System;
using System.Text.Json;

namespace GettingStarted
{
    public static class AgentMessageUtils
    {
        public static void WriteAgentChatMessage(Microsoft.SemanticKernel.ChatMessageContent message)
        {
            string authorExpression = message.Role == AuthorRole.User
                ? string.Empty
                : $" - {GetAuthorNameSafe(message)}";

            Console.WriteLine($"{message.Content}{authorExpression}");
            string contentExpression = string.IsNullOrWhiteSpace(message.Content) ? string.Empty : message.Content;
            bool isCode = message.Metadata?.ContainsKey(OpenAIAssistantAgent.CodeInterpreterMetadataKey) ?? false;
            string codeMarker = isCode ? "\n  [CODE]\n" : " ";
            Console.WriteLine($"\n# {message.Role}{authorExpression}:{codeMarker}{contentExpression}");

            foreach (KernelContent item in message.Items)
            {
#pragma warning disable SKEXP0110
                if (item is AnnotationContent annotation)
                {
                    if (annotation.Kind == AnnotationKind.UrlCitation)
                        Console.WriteLine($"  [{item.GetType().Name}] {annotation.Label}: {annotation.ReferenceId} - {annotation.Title}");
                    else
                        Console.WriteLine($"  [{item.GetType().Name}] {annotation.Label}: File #{annotation.ReferenceId}");
                }
                else if (item is FileReferenceContent fileReference)
                {
                    Console.WriteLine($"  [{item.GetType().Name}] File #{fileReference.FileId}");
                }
                else if (item is ImageContent image)
                {
                    Console.WriteLine($"  [{item.GetType().Name}] {image.Uri?.ToString() ?? image.DataUri ?? $"{image.Data?.Length} bytes"}");
                }
                else if (item is FunctionCallContent functionCall)
                {
                    Console.WriteLine($"  [{item.GetType().Name}] {functionCall.Id}");
                }
                else if (item is FunctionResultContent functionResult)
                {
                    Console.WriteLine($"  [{item.GetType().Name}] {functionResult.CallId} - {SerializeToJson(functionResult.Result)}");
                }
#pragma warning restore SKEXP0110
            }
            if (message.Metadata?.TryGetValue("Usage", out object? usage) ?? false)
            {
#pragma warning disable OPENAI001
                if (usage is RunStepTokenUsage assistantUsage)
                {
                    WriteUsage(assistantUsage.TotalTokenCount, assistantUsage.InputTokenCount, assistantUsage.OutputTokenCount);
                }
                else if (usage is RunStepCompletionUsage agentUsage)
                {
                    WriteUsage(agentUsage.TotalTokens, agentUsage.PromptTokens, agentUsage.CompletionTokens);
                }
                else if (usage is ChatTokenUsage chatUsage)
                {
                    WriteUsage(chatUsage.TotalTokenCount, chatUsage.InputTokenCount, chatUsage.OutputTokenCount);
                }
#pragma warning restore OPENAI001
            }

            static void WriteUsage(long totalTokens, long inputTokens, long outputTokens)
            {
                Console.WriteLine($"  [Usage] Tokens: {totalTokens}, Input: {inputTokens}, Output: {outputTokens}");
            }
        }

        public static string GetAuthorNameSafe(Microsoft.SemanticKernel.ChatMessageContent message)
        {
            return message.GetType().GetProperty("AuthorName")?.GetValue(message) as string ?? "*";
        }

        public static string SerializeToJson(object? obj)
        {
            if (obj == null) return "*";
            try { return JsonSerializer.Serialize(obj); }
            catch { return "*"; }
        }
    }
}
