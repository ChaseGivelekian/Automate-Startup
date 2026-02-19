using System.Windows.Automation;

namespace Automate_Startup.Services;

public interface IProcessManager
{
    AutomationElement LaunchRustDesk();
    void CloseWindow(AutomationElement window);
}