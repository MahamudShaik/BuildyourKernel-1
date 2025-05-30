using BuildyourKernel_1.Plugins.CareerHistory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildyourKernel_1
{
     public class UsingFunctionInPrompt
    {

        public async Task run(Kernel kernel)
        {
            kernel.ImportPluginFromType<CareerTrackerPlugin>();
#pragma warning disable SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            kernel.ImportPluginFromType<ConversationSummaryPlugin>();
#pragma warning restore SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.


            var history = await kernel.InvokeAsync("CareerTrackerPlugin", "GetCareerhistory");

            var prompt = @"User Career History:
                            {{ConversationSummaryPlugin.SummarizeConversation $history}}
                            Based on this user's career information, provide a list of alternate fields";

            var result = await kernel.InvokePromptAsync(prompt, new()
            {
                { "history", history }
            });

            //var prompt = @" This is a list of  the user's job history:
            //                {{CareerTrackerPlugin.GetCareerhistory}}
            //            Based on their career history , suggest a job role for them to pursue next and explain why";

            //var result = await kernel.InvokePromptAsync(prompt);
            Console.WriteLine(result);
        }
        }
}
