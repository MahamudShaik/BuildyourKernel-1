using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildyourKernel_1
{
    class semantickernalAssistantPlugin
    {


        public async Task run(Kernel kernel)
        {
            var plugins = kernel.CreatePluginFromPromptDirectory("Plugins");
            Console.WriteLine("Enter your current carrer or area of expertise: ");
            var carrerFocus = Console.ReadLine();

            var result = await kernel.InvokeAsync(plugins["CareerCoach"], new()
            {
                { "carrerFocus", carrerFocus   }
            });

            Console.WriteLine(result);
        }
    }
}
