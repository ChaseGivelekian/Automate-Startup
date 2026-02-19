using System.Diagnostics;
using System.Windows.Automation;

namespace Automate_Startup.Services;

public class UIAutomationService : IUIAutomationService
{
    public AutomationElement? FindElementByName(AutomationElement parent, string name,
        TreeScope scope = TreeScope.Descendants)
    {
        return parent.FindFirst(
            scope,
            new PropertyCondition(AutomationElement.NameProperty, name));
    }

    public AutomationElement? FindElementByNameContains(AutomationElement parent, string text)
    {
        var allElements = parent.FindAll(TreeScope.Descendants, Condition.TrueCondition);

        foreach (AutomationElement element in allElements)
            try
            {
                var name = element.Current.Name;
                if (!string.IsNullOrEmpty(name) && name.Contains(text, StringComparison.OrdinalIgnoreCase))
                    return element;
            }
            catch
            {
                // Some elements may throw exceptions when accessing properties
            }

        return null;
    }

    public AutomationElement WaitForWindow(string windowName, TimeSpan timeout)
    {
        var stopwatch = Stopwatch.StartNew();
        AutomationElement? window = null;

        while (stopwatch.Elapsed < timeout && window == null)
        {
            Thread.Sleep(500);
            window = AutomationElement.RootElement.FindFirst(
                TreeScope.Children,
                new PropertyCondition(AutomationElement.NameProperty, windowName));
        }

        if (window == null)
            throw new TimeoutException($"Window '{windowName}' did not appear within {timeout.TotalSeconds} seconds.");

        return window;
    }

    public void SetFocus(AutomationElement element)
    {
        element?.SetFocus();
    }
}