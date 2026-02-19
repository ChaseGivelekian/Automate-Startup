using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Automation;

namespace Automate_Startup.Services;

public class InputService : IInputService
{
    private const uint INPUT_MOUSE = 0;
    private const uint MOUSEEVENTF_ABSOLUTE = 0x8000;
    private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const uint MOUSEEVENTF_LEFTUP = 0x0004;
    private const uint MOUSEEVENTF_MOVE = 0x0001;

    public void ClickBottomRight(AutomationElement element)
    {
        if (element == null) return;

        var rect = (Rect)element.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);
        var clickX = (int)(rect.Right - 26);
        var clickY = (int)(rect.Bottom - 15);

        ClickAtScreenPosition(clickX, clickY);
        ClickAtScreenPosition(clickX, clickY); // Double click
    }

    public void ClickCenter(AutomationElement element)
    {
        if (element == null) return;

        var rect = (Rect)element.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);
        var clickX = (int)(rect.Left + rect.Width / 2);
        var clickY = (int)(rect.Top + rect.Height / 2);

        ClickAtScreenPosition(clickX, clickY);
    }

    public void SendKeys(string keys)
    {
        System.Windows.Forms.SendKeys.SendWait(keys);
    }

    [DllImport("user32.dll")]
    private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    private static void ClickAtScreenPosition(int clickX, int clickY)
    {
        var screenWidth = Screen.PrimaryScreen!.Bounds.Width;
        var screenHeight = Screen.PrimaryScreen.Bounds.Height;

        var normalizedX = clickX * 65535 / screenWidth;
        var normalizedY = clickY * 65535 / screenHeight;

        var inputs = new INPUT[3];

        inputs[0].type = INPUT_MOUSE;
        inputs[0].mi.dx = normalizedX;
        inputs[0].mi.dy = normalizedY;
        inputs[0].mi.dwFlags = MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE;

        inputs[1].type = INPUT_MOUSE;
        inputs[1].mi.dx = normalizedX;
        inputs[1].mi.dy = normalizedY;
        inputs[1].mi.dwFlags = MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_ABSOLUTE;

        inputs[2].type = INPUT_MOUSE;
        inputs[2].mi.dx = normalizedX;
        inputs[2].mi.dy = normalizedY;
        inputs[2].mi.dwFlags = MOUSEEVENTF_LEFTUP | MOUSEEVENTF_ABSOLUTE;

        SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct INPUT
    {
        public uint type;
        public MOUSEINPUT mi;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }
}