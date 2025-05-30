using ConsultantSelection;
using Microsoft.SemanticKernel;

var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(
"","",""
    );

var kernel = builder.Build();


await new Selection().run(kernel);


