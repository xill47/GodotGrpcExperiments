using System;
using Godot;
using GodotGrpcExperiment.Common;
using GodotGrpcExperiment.Common.Proto;
using Google.Protobuf;
using Grpc.Core;
using Lavabird.SceneOnReady;

namespace GodotGrpcExperiment.Godot3;

internal sealed class Server : Control
{
    [OnReady]
    private readonly LineEdit listenURIEdit = null!;

    [OnReady]
    private readonly Button startServerButton = null!;

    [OnReady]
    private readonly Button stopServerButton = null!;

    [OnReady]
    private readonly RichTextLabel requestLabel = null!;

    private IProgress<HelloRequest> helloRequestProgress = null!;

    private Grpc.Core.Server? server;

    public override void _Ready()
    {
        ShowServerState(false);

        helloRequestProgress = new Progress<HelloRequest>(HelloServiceCoreOnHelloRequest);

        startServerButton.Connect("pressed", this, nameof(OnStartServerButtonPressed));
        stopServerButton.Connect("pressed", this, nameof(OnStopServerButtonPressed));
    }

    private void HelloServiceCoreOnHelloRequest(HelloRequest request)
    {
        requestLabel.BbcodeText =
            $"Request: \n[code]{JsonFormatter.Default.Format(request)}[/code]";
    }

    private void OnStartServerButtonPressed()
    {
        var listenUri = new Uri(listenURIEdit.Text);
        server = new Grpc.Core.Server
        {
            Services = { HelloService.BindService(new HelloServiceCore(helloRequestProgress)) },
            Ports = { { listenUri.Host, listenUri.Port, ServerCredentials.Insecure } },
        };
        server.Start();
        ShowServerState(true);
    }

    private async void OnStopServerButtonPressed()
    {
        if (server == null)
        {
            ShowServerState(false);
            return;
        }
        try
        {
            await server.ShutdownAsync();
            ShowServerState(false);
        }
        catch (Exception e)
        {
            requestLabel.BbcodeText = $"Error stopping server: {e.Message}";
        }
    }

    private void ShowServerState(bool isStarted)
    {
        startServerButton.Disabled = isStarted;
        stopServerButton.Disabled = !isStarted;
        listenURIEdit.Editable = !isStarted;
    }
}
