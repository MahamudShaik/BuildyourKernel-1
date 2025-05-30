using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GettingStarted
{
    public class Step05_JsonResult
    {
        private Kernel Kernel { get; set; } = null!;

        private const int ScoreCompletionThreshold = 70;

        private const string TutorName = "Tutor";
        private const string TutorInstructions =
            """
        Think step-by-step and rate the user input on creativity and expressiveness from 1-100.

        Respond in JSON format with the following JSON schema:

        {
            "score": "integer (1-100)",
            "notes": "the reason for your score"
        }
        """;

        public Step05_JsonResult() {
            this.Kernel = Kernel.CreateBuilder()
.AddAzureOpenAIChatCompletion(
"","",""
).Build();
        }

        public async Task UseKernelFunctionStrategiesWithJsonResult()
        {
            ChatCompletionAgent agent = new()
            {
                Name = TutorName,
                Instructions = TutorInstructions,
                Kernel = this.Kernel
            };


#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            AgentGroupChat chat = new AgentGroupChat()
            {
                ExecutionSettings = new Microsoft.SemanticKernel.Agents.Chat.AgentGroupChatSettings()
                {
                    TerminationStrategy = new ThresholdTerminationStrategy()
                }
            };
#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            // Respond to user input
            await InvokeAgentAsync("The sunset is very colorful.");
            await InvokeAgentAsync("The sunset is setting over the mountains.");
            await InvokeAgentAsync("The sunset is setting over the mountains and filled the sky with a deep red flame, setting the clouds ablaze.");

            // Local function to invoke agent and display the conversation messages.
            async Task InvokeAgentAsync(string input)
            {
                ChatMessageContent message = new(AuthorRole.User, input);
                chat.AddChatMessage(message);
                AgentMessageUtils.WriteAgentChatMessage(message);

                await foreach (ChatMessageContent response in chat.InvokeAsync(agent))
                {
                    AgentMessageUtils.WriteAgentChatMessage(response);

                    Console.WriteLine($"[IS COMPLETED: {chat.IsComplete}]");
                }
            }
        }

    }
#pragma warning disable IDE1006 // Naming Styles
    public record struct WritingScore(int score, string notes);
    // Change the access modifier of the class from `private` to `internal` to resolve CS1527.
    // Classes defined in a namespace cannot be explicitly declared as private.

#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    internal sealed class ThresholdTerminationStrategy : TerminationStrategy
#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    {
        protected override Task<bool> ShouldAgentTerminateAsync(Agent agent, IReadOnlyList<ChatMessageContent> history, CancellationToken cancellationToken)
        {
            string lastMessageContent = history[history.Count - 1].Content ?? string.Empty;

            WritingScore? result = JsonResultTranslator.Translate<WritingScore>(lastMessageContent);

            return Task.FromResult((result?.score ?? 0) >= 70);
        }
    }
}
