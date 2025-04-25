using System;
using Godot;
using GodotGrpcExperiment.Common;
using GodotGrpcExperiment.Common.Proto;
using Google.Protobuf;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GodotGRpcExperiment.Godot4;

[SceneTree(root: "Root")]
internal sealed partial class Server : Control
{
    private IProgress<HelloRequest> helloRequestReporter = null!;

    private WebApplication? webHost;

    public override void _Ready()
    {
        ShowServerState(false);

        helloRequestReporter = new Progress<HelloRequest>(HelloServiceCoreOnHelloRequest);

        StartServerButton.Pressed += StartServerButtonOnPressed;
        StopServerButton.Pressed += StopServerButtonOnPressed;
    }

    private void StartServerButtonOnPressed()
    {
        try
        {
            var uri = new Uri(ListenURIEdit.Text);
            var builder = WebApplication.CreateSlimBuilder();

            builder.WebHost.UseKestrelCore();
            builder.WebHost.ConfigureKestrel(kestrelOptions =>
            {
                kestrelOptions.ListenAnyIP(
                    uri.Port,
                    listenOptions =>
                    {
                        listenOptions.Protocols = HttpProtocols.Http2;
                    }
                );
            });
            builder.Services.AddGrpc();
            builder.Services.AddSingleton(helloRequestReporter);

            webHost = builder.Build();
            webHost.MapGrpcService<HelloServiceCore>();
            webHost.Start();

            ShowServerState(true);
        }
        catch (Exception ex)
        {
            RequestLabel.Text =
                $"Failed to start server: {ex.Message}\n[code]{ex.StackTrace}[/code]";
        }
    }

    private async void StopServerButtonOnPressed()
    {
        if (webHost == null)
        {
            return;
        }

        try
        {
            await webHost.StopAsync();
            webHost = null;

            ShowServerState(false);
        }
        catch (Exception ex)
        {
            RequestLabel.Text = $"Failed to stop server: {ex.Message}";
        }
    }

    private void HelloServiceCoreOnHelloRequest(HelloRequest request)
    {
        RequestLabel.Text = $"Request: \n[code]{JsonFormatter.Default.Format(request)}[/code]";
    }

    private void ShowServerState(bool isStarted)
    {
        StartServerButton.Disabled = isStarted;
        StopServerButton.Disabled = !isStarted;
        ListenURIEdit.Editable = !isStarted;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            StartServerButton.Pressed -= StartServerButtonOnPressed;
            StopServerButton.Pressed -= StopServerButtonOnPressed;
        }

        base.Dispose(disposing);
    }
}
