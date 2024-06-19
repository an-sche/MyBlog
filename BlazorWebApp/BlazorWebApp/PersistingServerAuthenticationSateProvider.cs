using System.Data;
using System.Diagnostics;
using BlazorWebApp.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace BlazorWebApp;

internal sealed class PersistingServerAuthenticationSateProvider : ServerAuthenticationStateProvider, IDisposable
{
    private readonly PersistentComponentState _state;
    private readonly IdentityOptions _options;
    private readonly PersistingComponentStateSubscription _subscription;

    private Task<AuthenticationState>? _authenticationStateTask;

    public PersistingServerAuthenticationSateProvider(PersistentComponentState state, IOptions<IdentityOptions> options)
    {
        _state = state;
        _options = options.Value;

        AuthenticationStateChanged += OnAuthenticationStateChanged;
        _subscription = state.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
    }

    private void OnAuthenticationStateChanged(Task<AuthenticationState> task)
    {
        _authenticationStateTask = task;
    }

    private async Task OnPersistingAsync()
    {
        if (_authenticationStateTask is null) 
        {
            throw new UnreachableException($"Authentication State not set int {nameof(OnPersistingAsync)}().");
        }

        var authenticationState = await _authenticationStateTask;
        var principal = authenticationState.User;

        if (principal.Identity?.IsAuthenticated != true) 
        {
            return;
        }

        var userId = principal.FindFirst(_options.ClaimsIdentity.UserIdClaimType)?.Value;
        var email = principal.FindFirst(_options.ClaimsIdentity.EmailClaimType)?.Value;
        var roles = principal.FindAll(_options.ClaimsIdentity.RoleClaimType);

        if (userId != null)
        {
            _state.PersistAsJson(nameof(UserInfo), new UserInfo
            {
                UserId = userId,
                Email = email ?? string.Empty,
                Roles = roles.Select(x => x.Value).ToArray()
            });
        }

    }

    public void Dispose()
    {
        _subscription.Dispose();
        AuthenticationStateChanged -= OnAuthenticationStateChanged;
    }
}
