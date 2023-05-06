using HrSolution.Common.Shared;
using HrSolution.Data;
using HrSolution.Utility.Web.Server.Helpers; 
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer; 
using Microsoft.IdentityModel.Tokens; 

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = true,
        ValidAudience = LocalConfig.JwtAudience,
        ValidateIssuer = true,
        ValidIssuer = LocalConfig.JwtIssuer,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(LocalConfig.JwtSecretKey))  
    };
});



builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var configuration = builder.Configuration;

var connection = configuration.GetConnectionString("HrisDefaultConnection");
builder.Services.AddDbContextFactory<UixeDbContext>(options => options.UseMySql(connection, ServerVersion.AutoDetect(connection), optionBuilder => optionBuilder.EnableRetryOnFailure()), lifetime: ServiceLifetime.Singleton);
builder.Services.AddSingleton<IHrDataConnection, MySqlConnection>();
builder.Services.AddSingleton<IAppHostEnvironment, AppHostEnvironment>();
HrSolution.Domain.Bootstrap.RegisterType(builder.Services);
HrSolution.Core.Bootstrap.RegisterType(builder.Services);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePages();
app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseAuthentication(); 
app.UseRouting(); 
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
