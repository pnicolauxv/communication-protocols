using Grpc.Core;
using Data;

public class DataServiceImpl : DataService.DataServiceBase
{
    private string GeneratePayload(string size) => size == "large" ? new string('x', 500) : "small";
    // private string GeneratePayload(string size) => size == "large" ? new string('x', 500_000) : "small";
    // private string GeneratePayload(string size) => size == "large" ? new string('x', 1) : "small";

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