using Grpc.Core;
using Data;
using System.Text;

public class DataServiceImpl : DataService.DataServiceBase
{
    private const int ChunkSize = 1024;

    int GetPayloadSize(string size)
    {
        if (int.TryParse(size, out int multiplier) && multiplier > 0)
        {
            multiplier = Math.Min(multiplier, 1024); // max 1MB
            return multiplier * ChunkSize;
        }

        return ChunkSize;
    }

    public override async Task GetDataStream(
        DataRequest request,
        IServerStreamWriter<DataChunk> responseStream,
        ServerCallContext context)
    {
        var sizeHeader = context.RequestHeaders.GetValue("size") ?? "1";
        int totalSize = GetPayloadSize(sizeHeader);

        byte[] buffer = new byte[ChunkSize];
        Array.Fill(buffer, (byte)'x');

        int remaining = totalSize;

        while (remaining > 0)
        {
            int toWrite = Math.Min(remaining, ChunkSize);

            var chunk = new DataChunk
            {
                Data = Encoding.UTF8.GetString(buffer, 0, toWrite)
            };

            await responseStream.WriteAsync(chunk);

            remaining -= toWrite;
        }
    }
}