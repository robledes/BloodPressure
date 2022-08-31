global using BloodPressure.Shared;
global using System.Data;
global using Dapper;
global using BloodPressure.Server.Services.AuthService;
global using BloodPressure.Server.Services.MeasurementService;

using System.Data.SqlClient;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

//----------------------------------------------------------------------------------------
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddSingleton<IDbConnection>(sp => new SqlConnection(connectionString));
//----------------------------------------------------------------------------------------
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMeasurementService, MeasurementService>();
//----------------------------------------------------------------------------------------
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
//----------------------------------------------------------------------------------------

WebApplication? app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseWebAssemblyDebugging();
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();

//----------------------
app.UseAuthentication();
app.UseAuthorization();
//----------------------

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.Run();