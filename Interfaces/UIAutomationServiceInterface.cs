using System.Windows.Automation;

namespace Automate_Startup.Services;

public interface IUIAutomationService
{
    AutomationElement? FindElementByName(AutomationElement parent, string name,
        TreeScope scope = TreeScope.Descendants);

    AutomationElement? FindElementByNameContains(AutomationElement parent, string text);
    AutomationElement WaitForWindow(string windowName, TimeSpan timeout);
    void SetFocus(AutomationElement element);
}