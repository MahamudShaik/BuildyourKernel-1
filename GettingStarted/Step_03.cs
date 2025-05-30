using Azure;
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
     public class Step_03
    {
        private const string ReviewerName = "ArtDirector";
        private const string ReviewerInstructions = """
            You are an art Director who has opinions about copywriting born of a love for David Ogivly.
            The goal is to determine if the given copy is acceptable to print.
            If so, state that it is approved.
            If not, provide insight on how to refine suggested copy without example.
            """;
        private const string CopyWriterName = "CopyWriter";
        private const string CopyWriterInstructions = """
            You are a copywriter with ten years of experience and are known for brevity and a dry humor.
            The goal is to refine and decide on the single best copy as an expert in the field.
            Only provide a single proposal per response.
            You're laser focused on the goal at hand.
            Don't waste time with chit chat.
            Consider suggestions when refining an idea..
            """;

        private Kernel Kernel { get; set; } = null!;
        public Step_03() {
            this.Kernel = Kernel.CreateBuilder()
.AddAzureOpenAIChatCompletion(
"","",""
).Build();
        }

        public async Task UseAgentGroupChatWithTwoAgents()
        {
            ChatCompletionAgent reviewer = new()
            {
                Name = ReviewerName,
                Instructions = ReviewerInstructions,
                Kernel = this.Kernel
            };

            ChatCompletionAgent copyWriter = new()
            {
                Name = CopyWriterName,
                Instructions = CopyWriterInstructions,
                Kernel = this.Kernel
            };

#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            AgentGroupChat agentGroupChat = new(reviewer,copyWriter)
            {
               ExecutionSettings = new ()
               {     
                   TerminationStrategy = new ApprovalTerminationStrategy()
                   {
                       Agents = [reviewer],
                       MaximumIterations = 10
                   }

               }
            };
#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.


            ChatMessageContent chat = new(AuthorRole.User, "concept: maps made out of egg cartons.");
            agentGroupChat.AddChatMessage(chat);
            AgentMessageUtils.WriteAgentChatMessage(chat);

            await foreach(ChatMessageContent response in agentGroupChat.InvokeAsync())
            {
                AgentMessageUtils.WriteAgentChatMessage(response);
            }

            Console.WriteLine($"\n[IS COMPLETED: {agentGroupChat.IsComplete}]");
        }

#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        public sealed class ApprovalTerminationStrategy : TerminationStrategy
#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        {
            protected override Task<bool> ShouldAgentTerminateAsync(Agent agent, IReadOnlyList<ChatMessageContent> history, CancellationToken cancellationToken)
            {
                if(agent.Name == ReviewerName)
                {
                    // Only the reviewer can approve
                    return Task.FromResult(history[history.Count - 1].Content?.Contains("approve", StringComparison.OrdinalIgnoreCase) ?? false);
                }
                return Task.FromResult(false);
            }
        }
    }
}
