using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

int GetPayloadSize(string? size)
{
    if (int.TryParse(size, out int multiplier) && multiplier > 0)
    {
        multiplier = Math.Min(multiplier, 1024);
        return multiplier * 1024;
    }

    return 1 * 1024;
}

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