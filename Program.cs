using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MiseHub.Api.Interfaces;
using MiseHub.Api.Middleware;
using MiseHub.Api.Services.Fake;
using Microsoft.AspNetCore.Authorization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// AddJsonOptions + JsonStringEnumConverter → OrderStatus blir "Pending" i
// JSON istället för siffra (0), mycket lättare att läsa i React/Swagger.
builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // genererar Swagger-UI på /swagger

// Webbläsaren blockerar som standard att localhost:5173 (React) anropar
// localhost:5080 (detta API) — CORS-policyn nedan tillåter det explicit.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Registrerar VILKEN klass som används för VILKET interface.
// DETTA BYTS UT när riktig databas kopplas på senare, t.ex.:
//   AddSingleton<ISupplierService, FakeSupplierService>()
//   → AddScoped<ISupplierService, DbSupplierService>()
// Inget annat i appen behöver ändras.
builder.Services.AddSingleton<ISupplierService, FakeSupplierService>();
builder.Services.AddSingleton<IProductService, FakeProductService>();
builder.Services.AddSingleton<IOrderService, FakeOrderService>();
builder.Services.AddSingleton<IAuthService, FakeAuthService>();

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key missing in config.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Måste ligga FÖRST — fångar fel från allt som kommer efter (CORS, auth, controllers)
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();