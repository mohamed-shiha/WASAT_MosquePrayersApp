using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using MudBlazor.Services;
using MudBlazorPages.Data;
using MudBlazorPages.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<MudBlazorPages.App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddMudServices();
builder.Services.AddSingleton<PrayerTimeService>();
builder.Services.AddSingleton<AuthStateProvider>();
builder.Services.AddScoped<FirebaseAuthService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();
