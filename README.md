# Godot gRPC Experiments

This repository contains proof of concept Godot 3 and 4 projects that can both serve and do requests defined in shared "Common" assembly.

Godot 3 requires 3.6 version, gRPC serving implemented via [gRPC C Core](https://github.com/grpc/grpc/tree/master/src/csharp)

Godot 4 requires 4.4.1 version and .NET 8 installed, gRPC serving implemented via ASP.NET Core

### Godot 4 manual configuration

This is required for debugging or otherwise running from the Godot editor itself (or IDEs that run game via it)

Until [project runtime configuration support](https://github.com/godotengine/godot/pull/72333) is merged, you have to compile `Godot4` project once, copy the `.godot/mono/temp/bin/Debug/GodotGRpcExperiment.Godot4.runtimeconfig.json` to wherever directory you have Godot 4 installed in `bin/GodotSharp/Api/Debug/GodotPlugins.runtimeconfig.json` (overwrite is safe).

For example, if you are managing Godot installations via `godotenv` and are on Windows, you can run this from `Godot4`:

```powershell
cp .\.godot\mono\temp\bin\Debug\GodotGRpcExperiment.Godot4.runtimeconfig.json $env:AppData\godotenv\godot\bin\GodotSharp\Api\Debug\GodotPlugins.runtimeconfig.json
```

