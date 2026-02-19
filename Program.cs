using System.Diagnostics;
using System.IO;
using Automate_Startup.Automation;
using Automate_Startup.Configuration;
using Automate_Startup.Services;

namespace Automate_Startup;

internal class Program
{
    private static async Task<int> Main()
    {
        try
        {
            // Load .env file
            DotEnvService.Load();

            Console.WriteLine("Starting automation workflows...\n");

            // Configuration
            var rustDeskConfig = new RustDeskConfig();

            // Services (Dependency Injection by hand - could use DI container for larger apps)
            var uiAutomation = new UIAutomationService();
            var processManager = new ProcessManager(rustDeskConfig, uiAutomation);
            var inputService = new InputService();

            Console.WriteLine("=== RustDesk Automation Workflow ===\n");
            var rustDeskAutomation = new RustDeskAutomationWorkflow(
                rustDeskConfig,
                processManager,
                uiAutomation,
                inputService);

            await rustDeskAutomation.RunAsync();

            // Run winget upgrade batch file
            Console.WriteLine("\n=== Running Winget Upgrade ===\n");
            RunWingetUpgradeFileWithOutputAsync();

            Console.WriteLine("\nAll automation workflows completed successfully.");

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return 1; // Error - Task scheduler will detect it and rerun the application
        }
    }

    private static void RunWingetUpgradeFileWithOutputAsync()
    {
        var batFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "winget-upgrade.bat");

        if (!File.Exists(batFilePath))
            throw new FileNotFoundException("Winget upgrade batch file not found.", batFilePath);

        var processStartInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/k \"{batFilePath}\"",
            UseShellExecute = true
        };

        using var process = new Process();
        process.StartInfo = processStartInfo;
        process.Start();
    }
}