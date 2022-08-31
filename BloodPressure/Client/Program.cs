global using BloodPressure.Shared;
global using BloodPressure.Client.Services.AuthService;
global using BloodPressure.Client.Services.MeasurementService;
global using Microsoft.AspNetCore.Components.Authorization;
global using Blazored.LocalStorage;
global using Microsoft.AspNetCore.Components;
global using MudBlazor;
global using BloodPressure.Client.Pages;

using BloodPressure.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

WebAssemblyHostBuilder? builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//-----------------------------------------
builder.Services.AddBlazoredLocalStorage();
//-----------------------------------------

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomEnd;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 5000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

//--------------------------------------------------------------------
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMeasurementService, MeasurementService>();
//---------------------------------------------------------------------------------
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
//---------------------------------------------------------------------------------

await builder.Build().RunAsync();