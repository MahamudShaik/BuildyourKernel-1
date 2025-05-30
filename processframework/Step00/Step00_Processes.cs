using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Process.Runtime;


namespace processframework.Step00
{
    public class Step00_Processes
    {
        public static class ProcessEvents
        {
            public const string StartProcess = nameof(StartProcess);
        }

        public Step00_Processes() { }

        public async Task UseSimplestProcessAsync()
        {
            Kernel kernel = Kernel.CreateBuilder().Build();

            // Create a process that will interact with the chat completion service
            ProcessBuilder processBuilder = kernel.CreateProcessBuilder("SimpleProcess")
                .AddEventHandler(ProcessEvents.StartProcess, async (context) =>
                {
                    // This is the handler for the StartProcess event
                    // It will be invoked when the process starts
                    Console.WriteLine("Process started!");
                    await Task.CompletedTask;
                });


        }
    }
}
