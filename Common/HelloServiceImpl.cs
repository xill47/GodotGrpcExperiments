using GodotGrpcExperiment.Common.Proto;

using Grpc.Core;

namespace GodotGrpcExperiment.Common;

public class HelloServiceCore : HelloService.HelloServiceBase
{
    public override Task<HelloResponse> Hello(HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HelloResponse
        {
            Greeting = $"Hello {request.Name}!",
            UnixTime = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        });
    }
}