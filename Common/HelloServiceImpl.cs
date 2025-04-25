using GodotGrpcExperiment.Common.Proto;
using Grpc.Core;

namespace GodotGrpcExperiment.Common;

public class HelloServiceCore : HelloService.HelloServiceBase
{
    private readonly IProgress<HelloRequest> requestReporter;

    public HelloServiceCore(IProgress<HelloRequest> requestReporter)
    {
        this.requestReporter = requestReporter;
    }

    public override Task<HelloResponse> Hello(HelloRequest request, ServerCallContext context)
    {
        requestReporter.Report(request);
        return Task.FromResult(
            new HelloResponse
            {
                Greeting = $"Hello {request.Name}!",
                UnixTime = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            }
        );
    }
}
