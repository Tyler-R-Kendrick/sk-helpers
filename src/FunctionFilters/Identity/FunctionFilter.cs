using System.Security.Principal;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace SKHelpers.FunctionFilters.Identity;
using Common;

public static class UnauthorizedFunctionResult
{
    public static FunctionResult CreateUnauthorizedResult(
        FunctionInvocationContext context,
        AuthenticateResult? authenticateResult = null) => new(
        context.Function,
        authenticateResult,
        context.Kernel.Culture,
        new Dictionary<string, object?>
        {
            { "StatusCode", StatusCodes.Status401Unauthorized },
            { "AuthenticateResult", authenticateResult }
        });
}
public sealed class AuthFunctionFilter(
    IAuthenticationService authenticationService,
    IHttpContextAccessor httpContextAccessor,
    Kernel kernel)
    : BaseFunctionFilter
{
    private const string
        AuthScheme = "AuthScheme",
        UserPrincipal = "User";

    protected sealed override async Task OnFunctionInvokingAsync(
        FunctionInvocationContext context)
    {
        var function = context.Function;
        var httpContext = httpContextAccessor.HttpContext;
        var authScheme = function.Metadata.AdditionalProperties[AuthScheme]?.ToString();
        if(httpContext == null || string.IsNullOrWhiteSpace(authScheme))
        {
            return;
        }

        if(kernel.Data.TryGetValue(UserPrincipal, out object? value)
            && value is IPrincipal principal
            && principal.Identity?.IsAuthenticated == true)
        {
            await base.OnFunctionInvokingAsync(context);
            return;
        }

        var result = await authenticationService.AuthenticateAsync(httpContext, authScheme);
        if(result.Principal == null)
        {
            context.Result = UnauthorizedFunctionResult.CreateUnauthorizedResult(context);
            return;
        }
        
        principal = result.Principal;
        kernel.Data[UserPrincipal] = principal;
        context.Result = UnauthorizedFunctionResult.CreateUnauthorizedResult(context, result);
    }

    protected sealed override Task OnFunctionInvokedAsync(
        FunctionInvocationContext context)
    {
        return Task.CompletedTask;
    }
}
