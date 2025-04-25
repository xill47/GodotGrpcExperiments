using System;
using Godot;
using GodotGrpcExperiment.Common.Proto;
using Google.Protobuf;
using Grpc.Core;
using Lavabird.SceneOnReady;

namespace GodotGrpcExperiment.Godot3;

internal sealed class Client : Control
{
    [OnReady]
    private readonly LineEdit nameEdit = null!;

    [OnReady]
    private readonly LineEdit uriEdit = null!;

    [OnReady]
    private readonly Button requestButton = null!;

    [OnReady]
    private readonly RichTextLabel responseLabel = null!;

    public override void _Ready()
    {
        requestButton.Connect("pressed", this, nameof(OnRequestButtonPressed));
    }

    private async void OnRequestButtonPressed()
    {
        requestButton.Disabled = true;
        try
        {
            var uri = new Uri(uriEdit.Text);
            var grpcChannel = new Channel(uri.Host, uri.Port, ChannelCredentials.Insecure);
            var client = new HelloService.HelloServiceClient(grpcChannel);
            try
            {
                HelloResponse? helloResponse = await client.HelloAsync(
                    new HelloRequest { Name = nameEdit.Text }
                );
                responseLabel.BbcodeText =
                    $"Response:\n[code]{JsonFormatter.Default.Format(helloResponse)}[/code]";
            }
            finally
            {
                await grpcChannel.ShutdownAsync();
            }
        }
        catch (Exception e)
        {
            responseLabel.BbcodeText = $"Error: {e.Message}\n[code]{e.StackTrace}[/code]";
        }
        finally
        {
            requestButton.Disabled = false;
        }
    }
}
