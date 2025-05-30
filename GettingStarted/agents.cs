using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.OpenAI;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenAI.Assistants;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.AI.Agents.Persistent;
using ChatMessageContent = Microsoft.SemanticKernel.ChatMessageContent;
using ChatTokenUsage = OpenAI.Chat.ChatTokenUsage;
using System.Text.Json;

namespace GettingStarted
{
    public class agents
    {
        private const string ParrotName = "Parrot";
        private const string ParrotInstructions = "Repeat the user message in the voice of a pirate and then end with a parrot sound.";

        private const string JokerName = "Joker";
        private const string JokerInstructions = "You are good at telling jokes.";
        private AgentThread? thread = null;


        private Kernel Kernel { get; set; } = null!;
        public agents()
        {
            this.thread = null;
            this.Kernel = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(
"","",""
                ).Build();

        }

        public async Task UseSingleChatCompletionAgent()
        {

            ChatCompletionAgent agent = new()
            {
                Name = ParrotName,
                Instructions = ParrotInstructions,
                Kernel = this.Kernel
            };

            // Respond to user input
            await this.InvokeAgentAsync("Fortune favors the bold.", agent);
            await this.InvokeAgentAsync("I came, I saw, I conquered.", agent);
            await this.InvokeAgentAsync("Practice makes perfect.", agent);

        }


        public async Task UseSingleChatCompletionAgentWithConversation()
        {
            ChatCompletionAgent agent = new()
            {
                Name = JokerName,
                Instructions = JokerInstructions,
                Kernel = this.Kernel
            };


            // Start a conversation with the agent
            await this.InvokeAgentAsync("Tell me a joke.", agent, this.thread);
            await this.InvokeAgentAsync("Tell me another joke.", agent, this.thread);
            await this.InvokeAgentAsync("Tell me a joke about cats.", agent, this.thread);
            await this.InvokeAgentAsync("Now add some emojis to the joke.",agent,this.thread);
            this.thread = null; // Reset the thread after the conversation is done 
        }

        public async Task UseSingleChatCompletionAgentWithManuallyCreatedThread()
        {

            ChatCompletionAgent agent = new()
            {
                Name = JokerName,
                Instructions = JokerInstructions,
                Kernel = this.Kernel
            };
            // Manually create a thread
             this.thread = new ChatHistoryAgentThread([
                    new ChatMessageContent(AuthorRole.User, "Tell me a joke about a pirate."),
                    new ChatMessageContent(AuthorRole.Assistant, "Why did the pirate go to the gym? To improve his 'arrrrms'!"),
                ]);
            // Respond to user input
            await this.InvokeAgentAsync("Now add some emojis to the joke.", agent, this.thread);
            await this.InvokeAgentAsync("Now make the joke sillier.",agent,this.thread);

        }

        private async Task InvokeAgentAsync(string Input, ChatCompletionAgent agent, AgentThread? thread = null)
        {
            ChatMessageContent message = new(AuthorRole.User, Input);
            AgentMessageUtils.WriteAgentChatMessage(message);


            await foreach (AgentResponseItem<ChatMessageContent> response in agent.InvokeAsync(message, thread))
            {
                AgentMessageUtils.WriteAgentChatMessage(response);
            }

        }


    }
}
