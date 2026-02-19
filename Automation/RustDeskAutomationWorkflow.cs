using System.Diagnostics;
using System.Windows.Automation;
using Automate_Startup.Configuration;
using Automate_Startup.Services;

namespace Automate_Startup.Automation;

public class RustDeskAutomationWorkflow(
    RustDeskConfig config,
    IProcessManager processManager,
    IUIAutomationService uiAutomation,
    IInputService inputService)
{
    public async Task RunAsync()
    {
        AutomationElement? mainWindow = null;

        try
        {
            Console.WriteLine("Launching RustDesk...");
            mainWindow = processManager.LaunchRustDesk();

            // Thread.Sleep(10000);

            Console.WriteLine("Opening connection...");
            await OpenConnectionAsync(mainWindow);

            Console.WriteLine("Running last command...");
            RunLastCommand();

            Console.WriteLine("Automation completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError: {ex.Message}");
            throw;
        }
        finally
        {
            if (mainWindow != null)
            {
                Console.WriteLine("Closing RustDesk...");
                processManager.CloseWindow(mainWindow);
            }
        }
    }

    private async Task OpenConnectionAsync(AutomationElement mainWindow)
    {
        // Find the connection entry
        var connectionFullName = $"{config.ConnectionName}\n{config.ConnectionId}. {config.ConnectionName}";
        var connection = await WaitForElementAsync(mainWindow, connectionFullName);

        if (connection == null) throw new InvalidOperationException($"Connection '{config.ConnectionName}' not found.");

        // Click the menu button
        inputService.ClickBottomRight(connection);
        await Task.Delay(500);

        // Click Terminal button
        var terminalButton = await WaitForElementAsync(mainWindow, "Terminal (beta)");
        if (terminalButton == null) throw new InvalidOperationException("Terminal button not found.");

        inputService.ClickCenter(terminalButton);

        // Wait for terminal window to be ready
        await WaitForTerminalReadyAsync();
    }

    private async Task WaitForTerminalReadyAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        AutomationElement? terminalWindow = null;

        // First, wait for the terminal window to appear
        while (stopwatch.Elapsed < config.WindowTimeout)
        {
            terminalWindow = uiAutomation.FindElementByName(
                AutomationElement.RootElement,
                config.TerminalWindowTitle);

            if (terminalWindow != null)
                break;

            await Task.Delay(500);
        }

        if (terminalWindow == null)
            throw new TimeoutException(
                $"Terminal window did not appear within {config.WindowTimeout.TotalSeconds} seconds.");

        // Now wait for the connecting dialog to disappear
        stopwatch.Restart();
        while (stopwatch.Elapsed < config.WindowTimeout)
        {
            // Refresh the terminal window reference
            terminalWindow = uiAutomation.FindElementByName(
                AutomationElement.RootElement,
                config.TerminalWindowTitle);

            if (terminalWindow == null)
                throw new InvalidOperationException("Terminal window closed unexpectedly.");

            // Check if "Connecting..." or "Cancel" button is still visible
            var connectingElement = uiAutomation.FindElementByNameContains(terminalWindow, "Connecting");
            var cancelButton = uiAutomation.FindElementByName(terminalWindow, "Cancel");

            if (connectingElement == null && cancelButton == null)
            {
                // Wait a bit more to ensure the UI animation completes
                await Task.Delay(1000);

                // Verify it's still gone (not a flicker)
                terminalWindow = uiAutomation.FindElementByName(
                    AutomationElement.RootElement,
                    config.TerminalWindowTitle);

                if (terminalWindow != null)
                {
                    connectingElement = uiAutomation.FindElementByNameContains(terminalWindow, "Connecting");
                    cancelButton = uiAutomation.FindElementByName(terminalWindow, "Cancel");

                    if (connectingElement == null && cancelButton == null)
                    {
                        // Connection dialog is gone and stayed gone, terminal is ready
                        Console.WriteLine("Terminal connection established.");
                        return;
                    }
                }
            }

            await Task.Delay(500);
        }

        throw new TimeoutException(
            $"Terminal did not finish connecting within {config.WindowTimeout.TotalSeconds} seconds.");
    }

    private async Task<AutomationElement?> WaitForElementAsync(AutomationElement mainWindow,
        string connectionFullName)
    {
        var stopwatch = Stopwatch.StartNew();

        while (stopwatch.Elapsed < config.WindowTimeout)
        {
            var connection = uiAutomation.FindElementByName(mainWindow, connectionFullName);
            if (connection != null)
                return connection;

            await Task.Delay(500);
        }

        return null;
    }

    private void RunLastCommand()
    {
        var terminalWindow = uiAutomation.FindElementByName(
            AutomationElement.RootElement,
            config.TerminalWindowTitle);

        if (terminalWindow == null) throw new InvalidOperationException("Terminal window not found.");

        uiAutomation.SetFocus(terminalWindow);

        inputService.SendKeys("{UP}");
        inputService.SendKeys("{ENTER}");

        Console.WriteLine("Command sent successfully.");
    }
}