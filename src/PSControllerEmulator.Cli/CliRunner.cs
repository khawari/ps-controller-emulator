using PSControllerEmulator.Core.Controllers;
using PSControllerEmulator.Devices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PSControllerEmulator.Cli;

public static class CliRunner
{
    public static async Task<int> RunAsync(
        string[] args,
        TextWriter output,
        TextWriter error,
        IControllerDiscoveryService? discoveryService = null,
        CancellationToken cancellationToken = default)
    {
        if (args.Length == 0 || HasOption(args, "--help") || HasOption(args, "-h"))
        {
            WriteHelp(output);
            return 0;
        }

        var command = args[0].Trim().ToLowerInvariant();

        return command switch
        {
            "devices" => await RunDevicesAsync(args.Skip(1).ToArray(), output, discoveryService, cancellationToken),
            _ => WriteUnknownCommand(command, error)
        };
    }

    private static async Task<int> RunDevicesAsync(
        string[] args,
        TextWriter output,
        IControllerDiscoveryService? discoveryService,
        CancellationToken cancellationToken)
    {
        var useMock = HasOption(args, "--mock");
        var useJson = HasOption(args, "--json");
        var service = discoveryService ?? DeviceServiceFactory.CreateForCli(useMock);
        var controllers = await service.DiscoverAsync(cancellationToken);

        if (useJson)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            jsonOptions.Converters.Add(new JsonStringEnumConverter());

            await output.WriteLineAsync(JsonSerializer.Serialize(controllers, jsonOptions));
            return 0;
        }

        await output.WriteLineAsync("PS Controller Emulator devices");

        if (controllers.Count == 0)
        {
            await output.WriteLineAsync("No supported controllers detected.");
            return 0;
        }

        foreach (var controller in controllers)
        {
            await output.WriteLineAsync($"- {controller.DisplayName} ({controller.Model.ToDisplayName()})");
            await output.WriteLineAsync($"  Instance: {controller.InstanceId}");
            await output.WriteLineAsync($"  Connection: {controller.ConnectionType.ToDisplayText()}");
            await output.WriteLineAsync($"  Battery: {controller.Battery.ToDisplayText()}");
            await output.WriteLineAsync($"  Input: {controller.InputState.ToDisplayText()}");
        }

        return 0;
    }

    private static bool HasOption(IEnumerable<string> args, string option)
    {
        return args.Any(arg => string.Equals(arg, option, StringComparison.OrdinalIgnoreCase));
    }

    private static int WriteUnknownCommand(string command, TextWriter error)
    {
        error.WriteLine($"Unknown command '{command}'.");
        error.WriteLine("Run 'psce --help' for usage.");
        return 2;
    }

    private static void WriteHelp(TextWriter output)
    {
        output.WriteLine("PS Controller Emulator CLI");
        output.WriteLine();
        output.WriteLine("Usage:");
        output.WriteLine("  psce devices [--mock] [--json]");
        output.WriteLine();
        output.WriteLine("Commands:");
        output.WriteLine("  devices   List supported PlayStation controllers.");
    }
}
