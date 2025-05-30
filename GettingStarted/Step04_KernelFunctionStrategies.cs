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
     public class Step04_KernelFunctionStrategies
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

        public Step04_KernelFunctionStrategies() {
            this.Kernel = Kernel.CreateBuilder()
.AddAzureOpenAIChatCompletion(
"","",""
).Build();
        }

        public async Task UseKernelFunctionStrategiesWithAgentGroupChat()
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
            KernelFunction terminationFunction = AgentGroupChat.CreatePromptFunctionForStrategy(
                """
                Determine if the copy has been approved.  If so, respond with a single word: yes
                History:
                {{$history}}
                """, safeParameterNames: new[] { "history" }
                );

            KernelFunction selectionFunction  = 
            AgentGroupChat.CreatePromptFunctionForStrategy(
                $$$"""
                Determine which participant takes the next turn in a conversation based on the the most recent participant.
                State only the name of the participant to take the next turn.
                No participant should take more than one turn in a row.

                Choose only from these participants:
                - {{{ReviewerName}}}
                - {{{CopyWriterName}}}

                Always follow these rules when selecting the next participant:
                - After {{{CopyWriterName}}}, it is {{{ReviewerName}}}'s turn.
                - After {{{ReviewerName}}}, it is {{{CopyWriterName}}}'s turn.

                History:
                {{$history}}
                """
                , safeParameterNames: new[] { "history" }
                );

            // Limit history used for selection and termination to the most recent message.
            ChatHistoryTruncationReducer strategyReducer = new(1);

            // Create a chat for agent interaction.
            AgentGroupChat chat =
                new(copyWriter, reviewer)
                {
                    ExecutionSettings = new()
                    {
                        TerminationStrategy = new KernelFunctionTerminationStrategy(terminationFunction, Kernel)
                        {
                            Agents = [reviewer],
                            ResultParser = (result) =>
                            {
                                // Parse the result to determine if the copy is approved.
                                return result.GetValue<string>()?.Contains("yes", StringComparison.OrdinalIgnoreCase) ?? false;
                            },
                            HistoryVariableName = "history",
                            // Limit total number of turns
                            MaximumIterations = 10,
                            HistoryReducer = strategyReducer
                        },
                        SelectionStrategy = new KernelFunctionSelectionStrategy(selectionFunction, Kernel)
                        {
                            InitialAgent = copyWriter,
                            // Returns the entire result value as a string.
                            ResultParser = (result) => result.GetValue<string>() ?? CopyWriterName,
                            HistoryVariableName = "history",
                            // Save tokens by not including the entire history in the prompt
                            HistoryReducer = strategyReducer,
                            // Only include the agent names and not the message content
                            EvaluateNameOnly = true,

                        }
                    }
                };

            // Invoke chat and display messages.
            ChatMessageContent message = new(AuthorRole.User, "concept: maps made out of egg cartons.");
            chat.AddChatMessage(message);
            AgentMessageUtils.WriteAgentChatMessage(message);

            await foreach (ChatMessageContent response in chat.InvokeAsync())
            {
                AgentMessageUtils.WriteAgentChatMessage(response);
            }



#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        }
    }
}
