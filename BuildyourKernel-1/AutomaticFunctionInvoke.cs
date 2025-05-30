using BuildyourKernel_1.Plugins.CareerHistory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Plugins.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildyourKernel_1
{
    public class AutomaticFunctionInvoke
    {
        public async Task run(Kernel kernel)
        {
            kernel.ImportPluginFromType<CareerTrackerPlugin>();
            kernel.ImportPluginFromPromptDirectory("Prompts");
#pragma warning disable SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            kernel.ImportPluginFromType<ConversationSummaryPlugin>();
#pragma warning restore SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            OpenAIPromptExecutionSettings executionSettings = new OpenAIPromptExecutionSettings()
            {

                FunctionChoiceBehavior = FunctionChoiceBehavior.
            };

            string prompt = @"Iam a 30 years old and I live in Hyderabad. given my recent career history,
                            how do you suggest that I plan to change careers and what recommendations do you have for me.";

            var result = await kernel.InvokePromptAsync(prompt, new(executionSettings));
            Console.WriteLine(result);
        }
    }
}
