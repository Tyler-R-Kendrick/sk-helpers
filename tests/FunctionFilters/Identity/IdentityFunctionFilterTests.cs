// using System.Security.Principal;
// using Castle.Core.Logging;
// using Microsoft.AspNetCore.Authentication;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Http;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.VisualStudio.TestTools.UnitTesting;
// using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

// namespace SKHelpers.FunctionFilters.Identity.Tests;

// [TestClass]
// public class IdentityFunctionFilterTests
// {
//     private Mock<IAuthenticationService>? _authenticationService;
//     private Mock<IHttpContextAccessor>? _httpContextAccessor;
//     private Kernel? _kernel;

//     [TestMethod]
//     public async Task OnFunctionInvokingAsync_WhenPromptCached_ShouldNotInvoke()
//     {
//         // Arrange
//         _authenticationService = new Mock<IAuthenticationService>();
//         _httpContextAccessor = new Mock<IHttpContextAccessor>();
//         Mock<ILogger> mockLogger = new();
//         var kernelBuilder = Kernel.CreateBuilder();
//         kernelBuilder.Services.AddSingleton(_authenticationService.Object);
//         kernelBuilder.Services.AddSingleton(_httpContextAccessor.Object);
//         kernelBuilder.Services.AddSingleton((p) => _kernel!);
//         kernelBuilder.Services.AddSingleton<IFunctionInvocationFilter, AuthFunctionFilter>();
//         kernelBuilder.Plugins.AddFromObject(new TestPlugin(mockLogger.Object));
//         _kernel = kernelBuilder.Build();
//         AuthFunctionFilter filter = new(_authenticationService.Object, _httpContextAccessor.Object, _kernel);
//         _kernel.Data["User"] = new GenericPrincipal(new GenericIdentity("test"), []);

//         // Act
//         var result = await _kernel.InvokePromptAsync("test changing this input to lowercase");

//         // Assert
//         _authenticationService.Verify(x => x.AuthenticateAsync(It.IsAny<HttpContext>(), It.IsAny<string>()), Times.Never);
//         mockLogger.Verify(x => x.Info(It.IsAny<string>()), Times.Never);
//     }

//     private class TestPlugin(ILogger logger)
//     {
//         [Authorize]
//         [KernelFunction]
//         [Description("Changes the input string to lowercase.")]
//         public string ToLower(
//             [Description("The input string to change to lowercase.")]
//             string input)
//         {
//             logger.Info("ToLower called");
//             return input.ToLower();
//         }
//     }
// }
