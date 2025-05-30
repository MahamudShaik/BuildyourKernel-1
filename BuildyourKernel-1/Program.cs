using BuildyourKernel_1;
using Microsoft.SemanticKernel;

var builder = Kernel.CreateBuilder();

builder.AddAzureOpenAIChatCompletion(
    "","",""
    );

//// Suppress the diagnostic warning for TimePlugin
//#pragma warning disable SKEXP0050
//builder.Plugins.AddFromType<ConversationSummaryPlugin>();
////builder.Plugins.AddFromType<TimePlugin>();
//#pragma warning restore SKEXP0050

var kernel = builder.Build();

Console.WriteLine("Welcome to the Build Your Kernel demo!");
Console.WriteLine("This demo will show you how to build a kernel using the Semantic Kernel library.");
Console.WriteLine("You can use this kernel to create and run functions, and to interact with the OpenAI API.");
Console.WriteLine("You can also use the TimePlugin to get the current time.");
Console.WriteLine("You can use the ConversationSummaryPlugin to get a summary of your conversation.");

//Console.WriteLine("Enter your current career or area of expertise: ");
//var carrerHistory = Console.ReadLine();
//string prompt = @"This is some information about user background:
//        {{$carrerHistory}}
//           Given this user's background, provide a list of relevant career choices.";
//var result = await kernel.InvokePromptAsync(prompt, new()
//{
//    {"carrerHistory",carrerHistory}
//});


//#pragma warning disable SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
////var result = await kernel.InvokeAsync(nameof(TimePlugin), "DayOfWeek");
//var result = await kernel.InvokeAsync("ConversationSummaryPlugin", "SummarizeConversation", new()
//{
//    { "input", prompt }
//});
//#pragma warning restore SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

//var result = await kernel.InvokePromptAsync(prompt);

//Console.WriteLine("Enter your current carrer or area of expertise: ");
//var carrerFocus = Console.ReadLine();

//string prompt = @$"The following is a conversation with an AI carrer coaching assistant.
//                   The Assistant is helpful, creative and very friendly.
//                    <message role=""user"">Can you give me some carrer suggestions, since I want to travel more?
//                    </message>
//                    <message role=""assistant"">Of Course! What is your current career or area of expertise?.</message>
//                    <message role=""user"">${carrerFocus}</message>";
//var result = await kernel.InvokePromptAsync(prompt);
//Console.WriteLine(result);

//await new semantickernalAssistantPlugin().run(kernel);

//await new SKNativeFunction().run(kernel);

//await new UsingFunctionInPrompt().run(kernel);

await new AutomaticFunctionInvoke().run(kernel);