using System;
using Godot;
using GodotGrpcExperiment.Common.Proto;
using Google.Protobuf;
using Grpc.Net.Client;

namespace GodotGRpcExperiment.Godot4;

[SceneTree]
internal sealed partial class Client : Control
{
    public override void _Ready()
    {
        RequestButton.Pressed += RequestButtonOnPressed;
    }

    private async void RequestButtonOnPressed()
    {
        RequestButton.Disabled = true;
        try
        {
            var uri = new Uri(UriEdit.Text);
            using var grpcChannel = GrpcChannel.ForAddress(uri);
            var client = new HelloService.HelloServiceClient(grpcChannel);
            var response = await client.HelloAsync(new HelloRequest { Name = NameEdit.Text });
            ResponseLabel.Text =
                $"Response:\n[code]{JsonFormatter.Default.Format(response)}[/code]";
        }
        catch (Exception e)
        {
            ResponseLabel.Text = $"Error: {e.Message}\n[code]{e.StackTrace}[/code]";
        }
        finally
        {
            RequestButton.Disabled = false;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            RequestButton.Pressed -= RequestButtonOnPressed;
        }
        base.Dispose(disposing);
    }
}
