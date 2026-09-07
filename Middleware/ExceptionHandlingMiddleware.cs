namespace MiseHub.Api.Middleware;

// Fångar oväntade fel innan de kraschar appen. Ligger först i pipelinen
// (se Program.cs) så den kan fånga fel från allt som kommer efter.
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next; // "resten av pipelinen"
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // kör resten av kedjan (auth, controller osv)
        }
        catch (Exception ex)
        {
            // Logga hela felet själva, skicka aldrig detaljer till klienten
            _logger.LogError(ex, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;

            var payload = new
            {
                error = "Something went wrong. Please try again.",
                traceId = context.TraceIdentifier, // för att hitta rätt loggpost vid felsökning
            };

            await context.Response.WriteAsJsonAsync(payload);
        }
    }
}