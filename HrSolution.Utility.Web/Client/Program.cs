using HrSolution.Utility.Web.Client;
using HrSolution.Utility.Web.Client.Helpers;
using HrSolution.Utility.Web.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Syncfusion.Blazor;

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBMAY9C3t2VFhhQlJDfV5AQmBIYVp/TGpJfl96cVxMZVVBJAtUQF1hSn5QdURiWX9dcXJTR2Za;MTU3MzE4NkAzMjMxMmUzMTJlMzMzN2F6bFJpRC93RS9DQVNmZWh3S0IwQktDUEVEMkh1b1JtU1RtUWVBYTdXaHc9");

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddScoped<IAuthenticationService, AuthenticationService>()
    .AddScoped<IUserService, UserService>()
    .AddScoped<IHttpService, HttpService>()
    .AddScoped<ILocalStorageService, LocalStorageService>()
    .AddScoped<ITimekeepService, TimekeepService>();
    //.AddScoped<BlazorServiceAccessor>();

// configure http client
//builder.Services.AddScoped(x => {
//    //var apiUrl = new Uri(builder.Configuration["apiUrl"]);

//    // use fake backend if "fakeBackend" is "true" in appsettings.json
//    if (builder.Configuration["fakeBackend"] == "true")
//        return new HttpClient(new FakeBackendHandler()) { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };

//    return new HttpClient() { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
//});

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSyncfusionBlazor();

builder.Logging.SetMinimumLevel(LogLevel.Warning);

var host = builder.Build();

var logger = host.Services.GetRequiredService<ILoggerFactory>()
    .CreateLogger<Program>();

logger.LogInformation("Logged after the app is built in Program.cs.");
  
var authenticationService = host.Services.GetRequiredService<IAuthenticationService>();
await authenticationService.Initialize();

await host.RunAsync();