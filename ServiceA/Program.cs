using Grpc.Core;
using Grpc.Net.Client;
using Data;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Net.Http.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


// HTTP
var httpClient = new HttpClient();

// var grpcChannel = GrpcChannel.ForAddress("http://localhost:5002");
// var grpcChannel = GrpcChannel.ForAddress("http://host.docker.internal:5002");
var grpcChannel = GrpcChannel.ForAddress("http://servicebgrpc-service:5000");
var grpcClient = new DataService.DataServiceClient(grpcChannel);

// mTLS
var mtlsHandler = new HttpClientHandler();

var clientCert = new X509Certificate2(
    // "../certs/client-cert.pfx",
    // "certs/client-cert.pfx",
    "/app/certs/client-cert.pfx",
    "test123",
    X509KeyStorageFlags.UserKeySet |
    X509KeyStorageFlags.PersistKeySet |
    X509KeyStorageFlags.Exportable
);

mtlsHandler.ClientCertificates.Add(clientCert);

// SOLO desarrollo
mtlsHandler.ServerCertificateCustomValidationCallback =
    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

var mtlsClient = new HttpClient(mtlsHandler);


// =============================
// ENDPOINT
// =============================
app.MapGet("/test", async (string? protocol) =>
{
    protocol ??= "http";

    if (protocol == "http")
        return await CallHttp();

    if (protocol == "grpc")
        return await CallGrpc();

    if (protocol == "mtls")
        return await CallMtls();

    return Results.BadRequest("Invalid protocol");
});

// app.Run("http://localhost:5000");
app.Run("http://0.0.0.0:5000");

// =============================
// HTTP
// =============================
async Task<object> CallHttp()
{
    var response = await httpClient.GetFromJsonAsync<DataResponse>(
        // "http://localhost:5001/data?size=large"
        // "http://host.docker.internal:5001/data?size=large"
        "http://servicebhttp-service:5000/data?size=large"
    );

    return response!;
}


// =============================
// gRPC
// =============================
async Task<object> CallGrpc()
{
    var headers = new Metadata();
    headers.Add("size", "large");

    var reply = await grpcClient.GetDataAsync(new DataRequest(), headers);
    return reply;
}


// =============================
// mTLS
// =============================
async Task<object> CallMtls()
{
    var response = await mtlsClient.GetFromJsonAsync<DataResponse>(
        // "https://localhost:5003/data?size=large"
        // "https://host.docker.internal:5003/data?size=large"
        "https://servicebmtls-service:5000/data?size=large"
    );

    return response!;
}