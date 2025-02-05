using BootstrapBlazorAuthenticationExample.Share.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

namespace BootstrapBlazorAuthenticationExample.Share.Services;

public interface IAuthenticationService
{
    public Task<AuthenticationState> GetAuthenticationStateAsync();

    public Task LoginAsync(User user);

    public Task LogoutAsync();

}


public class AuthenticationService(ProtectedBrowserStorage sessionStorage) : IAuthenticationService
{
    private Task<AuthenticationState>? authenticationState = null;
    private ClaimsPrincipal anonymous = new ClaimsPrincipal(new ClaimsIdentity());
    private readonly ProtectedBrowserStorage sessionStorage = sessionStorage;

 
    public  async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            if (authenticationState != null && !authenticationState.Result.Equals(new AuthenticationState(anonymous)))
            {
                return await authenticationState;
            }

            var userSessionStorageResult = await sessionStorage.GetAsync<User>("User");
            var userSession = userSessionStorageResult.Success ? userSessionStorageResult.Value : null;
            if (userSession == null)
            {
                return await Task.FromResult(new AuthenticationState(anonymous));
            }

            var claimsPrincipal =
                new ClaimsPrincipal(
                    new ClaimsIdentity(new List<Claim> { new Claim(ClaimTypes.Name, userSession.Username), },
                        "CustomAuth"));
            this.authenticationState = Task.FromResult(new AuthenticationState(claimsPrincipal));

        }
        catch (Exception ex)
        {
            this.authenticationState = Task.FromResult(new AuthenticationState(anonymous));
        }

        return await this.authenticationState;
    }



    public async Task LoginAsync(User user)
    {
        ClaimsPrincipal claimsPrincipal;


        await sessionStorage.SetAsync("User", user);
        claimsPrincipal =
            new ClaimsPrincipal(
                new ClaimsIdentity(new List<Claim> { new Claim(ClaimTypes.Name, user.Username), },
                    "CustomAuth"));


        var authState = Task.FromResult(new AuthenticationState(claimsPrincipal));
        this.authenticationState = authState;
    }

    public async Task LogoutAsync()
    { 
        await sessionStorage.DeleteAsync("User");
        var authState = Task.FromResult(new AuthenticationState(anonymous));
        this.authenticationState = authState;
    }
}
