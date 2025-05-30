using GettingStarted.Plugins;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GettingStarted
{
    public class Step02_Plugins
    {
        private Kernel Kernel { get; set; } = null!;
        public Step02_Plugins()
        {
            this.Kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(
"","",""
    ).Build();


        }
        public async Task UseChatCompletionWithPlugin()
        {
            ChatCompletionAgent agent = this.createAgentWithPlugIn(
                KernelPluginFactory.CreateFromType<MenuPlugin>(),
                "Answer questions about the menu.",
                "Host"
                );

            AgentThread thread = new ChatHistoryAgentThread();

            // Start a conversation with the agent
            await InvokeAgentAsync(agent, thread, "Hello");
            await InvokeAgentAsync(agent, thread, "What is the special soup and its price?");
            await InvokeAgentAsync(agent, thread, "What is the special drink and its price?");
            await InvokeAgentAsync(agent, thread, "Thank you");


        }

        public async Task UseChatCompletionWithPluginEnumParameter()
        {
            // Define the agent
            ChatCompletionAgent agent = this.createAgentWithPlugIn(
                    KernelPluginFactory.CreateFromType<WidgetFactory>());

            AgentThread thread = new ChatHistoryAgentThread();

            // Respond to user input, invoking functions where appropriate.
            await InvokeAgentAsync(agent, thread, "Create a beautiful red colored widget for me.");

        }


        private ChatCompletionAgent createAgentWithPlugIn(
            KernelPlugin plugin,
            string? instructions = null,
            string? agentName = null
            )
        {
            ChatCompletionAgent agent = new()
            {
                Name = agentName ,
                Instructions = instructions, 
                Kernel = this.Kernel,
                Arguments = new KernelArguments(new PromptExecutionSettings() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() })
            };

            agent.Kernel
                .Plugins.Add(plugin);
            return agent;
        }

        // Local function to invoke agent and display the conversation messages.
        private async Task InvokeAgentAsync(ChatCompletionAgent agent, AgentThread thread, string input)
        {
            ChatMessageContent message = new(AuthorRole.User, input);
            AgentMessageUtils.WriteAgentChatMessage(message);

            await foreach (ChatMessageContent response in agent.InvokeAsync(message, thread))
            {
                AgentMessageUtils.WriteAgentChatMessage(response);
            }
        }

    }
}
