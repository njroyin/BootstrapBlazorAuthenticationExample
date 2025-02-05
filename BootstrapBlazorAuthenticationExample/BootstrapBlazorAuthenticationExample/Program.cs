using BootstrapBlazorAuthenticationExample.Components;
using BootstrapBlazorAuthenticationExample.Share.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddServerSideBlazor();
builder.Services.AddBootstrapBlazor();


// builder.Services.AddAuthenticationCore();
// builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<ProtectedSessionStorage>(); 
builder.Services.AddScoped<IAuthenticationService,AuthenticationService>(serviceProvider =>
{
  var sessionStorage = serviceProvider.GetService<ProtectedSessionStorage>();
  return new AuthenticationService(sessionStorage);
}); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStatusCodePagesWithRedirects("/StatusCode/{0}");
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BootstrapBlazorAuthenticationExample.Client._Imports).Assembly);
//
// app.UseAuthentication();
// app.UseAuthorization();
app.Run();