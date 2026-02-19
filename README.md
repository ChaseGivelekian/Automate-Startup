# Automate Startup
This is a small program that I created to automate some of the startup tasks that I do every time I start my computer. These include running the last command on a remote desktop using RustDesk and running the winget upgrade command.

## Features
- Connects to the desired remote desktop on RustDesk, opens the terminal, and runs the last command.
- Utilizes [System.Windows.Automation](https://learn.microsoft.com/en-us/dotnet/api/system.windows.automation?view=windowsdesktop-10.0) in C# to automate the UI interactions.
- Opens the terminal and runs the `winget upgrade` command to update installed applications.

## Usage
1. Clone the repository.
2. Create a `.env` file in the root directory with same structure as `.env.example` and fill in the required values.
3. Build and run the project.