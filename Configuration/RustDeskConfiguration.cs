using Automate_Startup.Services;

namespace Automate_Startup.Configuration;

public class RustDeskConfig
{
    public string ExecutablePath { get; set; } = DotEnvService.GetValue("RUSTDESK_PATH")!;
    public string ConnectionName { get; set; } = DotEnvService.GetValue("CONNECTION_NAME")!;
    public string ConnectionId { get; set; } = DotEnvService.GetValue("CONNECTION_ID")!;
    public TimeSpan WindowTimeout { get; set; } = TimeSpan.FromSeconds(15);
    public string TerminalWindowTitle { get; set; } = DotEnvService.GetValue("TERMINAL_WINDOW_TITLE")!;
}