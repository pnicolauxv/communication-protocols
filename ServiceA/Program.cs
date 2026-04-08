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
app.MapGet("/test", async (string? protocol, string? size) =>
{
    protocol ??= "http";
    size ??= "1";

    if (protocol == "http")
        return await CallHttp(size);

    if (protocol == "grpc")
        return await CallGrpc(size);

    if (protocol == "mtls")
        return await CallMtls(size);

    return Results.BadRequest("Invalid protocol");
});

// app.Run("http://localhost:5000");
app.Run("http://0.0.0.0:5000");

// =============================
// HTTP
// =============================
async Task<object> CallHttp(string size)
{
    var response = await httpClient.GetFromJsonAsync<DataResponse>(
        $"http://servicebhttp-service:5000/data?size={size}"
    );

    if (response is null)
        throw new Exception("Response was null");

    return response;
}



// =============================
// gRPC
// =============================
async Task<object> CallGrpc(string size)
{
    var headers = new Metadata();
    headers.Add("size", size);

    var response = await grpcClient.GetDataAsync(new DataRequest(), headers);

    return new
    {
        Message = response.Message,
        Timestamp = response.Timestamp,
        Data = response.Data
    };
}


// =============================
// mTLS
// =============================
async Task<object> CallMtls(string size)
{
    var response = await mtlsClient.GetFromJsonAsync<DataResponse>(
        $"https://servicebmtls-service:5000/data?size={size}"
    );

    if (response is null)
        throw new Exception("Response was null");

    return response;
}

public class DataResponse
{
    public string Message { get; set; } = string.Empty;
    public long Timestamp { get; set; }
    public string Data { get; set; } = string.Empty;
}