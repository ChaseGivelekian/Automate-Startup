using System.Windows.Automation;

namespace Automate_Startup.Services;

public interface IInputService
{
    void ClickBottomRight(AutomationElement element);
    void ClickCenter(AutomationElement element);
    void SendKeys(string keys);
}