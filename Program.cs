using MiseHub.Api.Interfaces;
using MiseHub.Api.Middleware;
using MiseHub.Api.Services.Fake;

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Måste ligga FÖRST — fångar fel från allt som kommer efter (CORS, auth, controllers)
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors();
app.UseAuthorization(); // förberedd för inloggning senare — gör inget än
app.MapControllers();

app.Run();