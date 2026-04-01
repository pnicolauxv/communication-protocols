using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// string GeneratePayload(string size) => size == "large" ? new string('x', 500_000) : "small";
string GeneratePayload(string size) => size == "large" ? new string('x', 500) : "small";
// string GeneratePayload(string size) => size == "large" ? new string('x', 1) : "small";

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