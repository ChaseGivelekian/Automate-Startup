using System.Diagnostics;
using System.Windows.Automation;
using Automate_Startup.Configuration;

namespace Automate_Startup.Services;

public class ProcessManager(RustDeskConfig config, IUIAutomationService uiAutomation) : IProcessManager
{
    public AutomationElement LaunchRustDesk()
    {
        Process.Start(config.ExecutablePath);
        Process.Start(config.ExecutablePath);

        var mainWindow = uiAutomation.WaitForWindow("RustDesk", config.WindowTimeout);
        uiAutomation.SetFocus(mainWindow);

        return mainWindow;
    }

    public void CloseWindow(AutomationElement window)
    {
        if (window == null) return;

        var windowPattern = window.GetCurrentPattern(WindowPattern.Pattern) as WindowPattern;
        windowPattern?.Close();
    }
}