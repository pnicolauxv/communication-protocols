using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

int GetPayloadSize(string? size) => size?.ToLowerInvariant() switch
{
    "small" => 1 * 1024,     // 1 KB
    "medium" => 10 * 1024,   // 10 KB
    "large" => 100 * 1024,   // 100 KB
    _ => 1 * 1024
};

app.MapGet("/data", async (HttpResponse response, string? size) =>
{
    int payloadSize = GetPayloadSize(size);

    response.StatusCode = 200;
    response.ContentType = "application/json";

    var header = $$"""
    {"message":"ok","timestamp":{{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}},"data":"
    """;

    await response.WriteAsync(header);

    byte[] buffer = new byte[1024];
    Array.Fill(buffer, (byte)'x');

    int remaining = payloadSize;

    while (remaining > 0)
    {
        int toWrite = Math.Min(remaining, buffer.Length);
        await response.Body.WriteAsync(buffer.AsMemory(0, toWrite));
        remaining -= toWrite;
    }

    await response.WriteAsync("\"}");
});

// app.Run("http://localhost:5001");
// app.Run("http://0.0.0.0:5001");
app.Run("http://0.0.0.0:5000");