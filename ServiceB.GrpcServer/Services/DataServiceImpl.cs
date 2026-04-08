using Grpc.Core;
using Data;

public class DataServiceImpl : DataService.DataServiceBase
{
    int GetPayloadSize(string size)
    {
        if (int.TryParse(size, out int multiplier) && multiplier > 0)
        {
            multiplier = Math.Min(multiplier, 1024);
            return multiplier * 1024;
        }

        return 1 * 1024;
    }

    public override Task<DataResponse> GetData(DataRequest request, ServerCallContext context)
    {
        var sizeHeader = context.RequestHeaders.GetValue("size") ?? "1";
        int totalSize = GetPayloadSize(sizeHeader);

        string payload = new string('x', totalSize);

        var response = new DataResponse
        {
            Message = "grpc ok",
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Data = payload
        };

        return Task.FromResult(response);
    }
}