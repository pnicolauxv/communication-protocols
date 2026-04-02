using Grpc.Core;
using Data;

public class DataServiceImpl : DataService.DataServiceBase
{
    string GeneratePayload(string size) => size switch
        {
            "small" => new string('x', 1 * 1024),       // 1 KB
            "medium" => new string('x', 10 * 1024),     // 10 KB
            "large" => new string('x', 100 * 1024),     // 100 KB
            _ => new string('x', 1 * 1024)              // default: small
        };

    public override Task<DataResponse> GetData(DataRequest request, ServerCallContext context)
    {
        var size = context.RequestHeaders.GetValue("size") ?? "small";

        var response = new DataResponse
        {
            Message = "grpc ok",
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Data = GeneratePayload(size)
        };

        return Task.FromResult(response);
    }
}