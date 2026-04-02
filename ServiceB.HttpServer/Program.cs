using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

string GeneratePayload(string size) => size switch
{
    "small" => new string('x', 1 * 1024),       // 1 KB
    "medium" => new string('x', 10 * 1024),     // 10 KB
    "large" => new string('x', 100 * 1024),     // 100 KB
    _ => new string('x', 1 * 1024)              // default: small
};

app.MapGet("/data", (string? size) =>
{
    return Results.Json(new
    {
        message = "ok",
        timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
        data = GeneratePayload(size ?? "small")
    });
});

// app.Run("http://localhost:5001");
// app.Run("http://0.0.0.0:5001");
app.Run("http://0.0.0.0:5000");