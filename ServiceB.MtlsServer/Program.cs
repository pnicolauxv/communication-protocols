using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

builder.WebHost.ConfigureKestrel(options =>
{
    // options.ListenLocalhost(5003, listenOptions =>
    // options.ListenAnyIP(5003, listenOptions =>
    options.ListenAnyIP(5000, listenOptions =>
    {
        listenOptions.UseHttps(new HttpsConnectionAdapterOptions
        {
            // ServerCertificate = new X509Certificate2(@"C:\Users\PabloNicolau\Documents\Repositories\communication-protocols\MicroservicesCSharp\certs\server-cert.pfx", "test123"),
            ServerCertificate = new X509Certificate2("/app/certs/server-cert.pfx", "test123"),
            ClientCertificateMode = ClientCertificateMode.RequireCertificate,
            ClientCertificateValidation = (cert, chain, errors) =>
            {
                if (cert == null) return false;

                if (cert.Issuer.Contains("CN=MyLocalCA") || cert.Issuer.Contains("CN=localhost"))
                    return true;

                return false;
            }
        });
    });
});

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
        message = "secure ok",
        timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
        data = GeneratePayload(size ?? "small")
    });
});

app.Run();