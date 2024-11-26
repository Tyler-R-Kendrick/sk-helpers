using Castle.Core.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace SKHelpers.FunctionFilters.Identity.Tests;

public class CachedFunctionTests
{
    [Test]
    public async Task OnKernelInvokingPromptAsync_WhenPromptCached_ShouldNotInvokeFunction()
    {
        // Arrange
        Kernel? _kernel;
        Mock<ILogger> mockLogger = new();
        var kernelBuilder = Kernel.CreateBuilder();
        kernelBuilder.Plugins.AddFromObject(new TestPlugin(mockLogger.Object));
        _kernel = kernelBuilder.Build();

        // Act
        const string input = "test changing this input to lowercase";
        var executionSettings = new OpenAIPromptExecutionSettings
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };
        KernelArguments kernelArgs = new(executionSettings);
        var result1 = await _kernel.InvokePromptAsync(input, kernelArgs);
        var result2 = await _kernel.InvokePromptAsync(input, kernelArgs);

        // Assert
        mockLogger.Verify(x => x.Info(It.IsAny<string>()), Times.Once);
        Assert.That(result1, Is.EqualTo(result2));
    }

    private class TestPlugin(ILogger logger)
    {
        [Authorize]
        [KernelFunction]
        [Description("Changes the input string to lowercase.")]
        public string ToLower(
            [Description("The input string to change to lowercase.")]
            string input)
        {
            logger.Info("ToLower called");
            return input.ToLower();
        }
    }
}
