// See https://aka.ms/new-console-template for more information
using GettingStarted;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.OpenAI;

Console.WriteLine("Hello, World!");


//var kernel = Kernel.CreateBuilder()
//                .AddAzureOpenAIChatCompletion(
//"","",""
//                ).Build();

//// Example 1. Invoke the kernel with a prompt and display the result
//    var result = await kernel.InvokePromptAsync("What is the capital of France?");
//    Console.WriteLine($"Result from kernel: {result}");

//// Example 2. Invoke the kernel with a templated prompt and display the result
//       KernelArguments arguments = new () { { "country" , "India" } };
//    result = await kernel.InvokePromptAsync("What is the capital of {{$country}}?",arguments);
//Console.WriteLine($"Result from kernel: {result}");

//// Example 3. Invoke the kernel with a templated prompt and stream the results to the display
//await foreach ( var update in kernel.InvokePromptStreamingAsync("What is the capital of {{$country}}? Provide a detailed Explanation",arguments))
//{
//    Console.Write(update);
//}

//// Example 4. Invoke the kernel with a templated prompt and execution settings
//arguments = new(new OpenAIPromptExecutionSettings { MaxTokens = 500, Temperature = 0.5 }) { { "topic", "dogs" } };
//Console.WriteLine(await kernel.InvokePromptAsync("Tell me a story about {{$topic}}", arguments));

//arguments = new(new OpenAIPromptExecutionSettings { ResponseFormat = "json_object" }) { { "topic", "chocolate" } };
//Console.WriteLine(await kernel.InvokePromptAsync("Create a recipe for a {{$topic}} cake in JSON format", arguments));


//var result = new agents();
//await result.UseSingleChatCompletionAgent();
//await result.UseSingleChatCompletionAgentWithConversation();
//await result.UseSingleChatCompletionAgentWithManuallyCreatedThread();

//var res = new Step02_Plugins();
//await res.UseChatCompletionWithPlugin();
//await res.UseChatCompletionWithPluginEnumParameter();

//var res = new Step_03();
//await res.UseAgentGroupChatWithTwoAgents();

//var res = new Step04_KernelFunctionStrategies();
//await res.UseKernelFunctionStrategiesWithAgentGroupChat();

var res = new Step05_JsonResult();
await res.UseKernelFunctionStrategiesWithJsonResult();


Console.ReadKey();
