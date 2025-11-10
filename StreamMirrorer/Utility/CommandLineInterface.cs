using System.Diagnostics;
using System.Runtime.InteropServices;
using StreamMirrorer.Interfaces;

namespace StreamMirrorer.Utility;

public class CommandLineInterface : ICommandLineInterface
{
    public async Task<string> Execute(string command)
    {
        ProcessStartInfo processInfo = GetOsPlatformShell(command);

        processInfo.CreateNoWindow = true;
        processInfo.UseShellExecute = false;
        processInfo.RedirectStandardOutput = true;

        Process? process = Process.Start(processInfo);


        if (process == null)
        {
            throw new InvalidOperationException("Failed to start process.");
        }

        await process.WaitForExitAsync();

        return await process.StandardOutput.ReadToEndAsync();
    }

    private static ProcessStartInfo GetOsPlatformShell(string command)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return new ProcessStartInfo(
                "cmd.exe",
                "/C " + command
            );
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return new ProcessStartInfo(
                "/bin/bash",
                $"-c \"{command}\""
            );
        }
        else
        {
            throw new PlatformNotSupportedException("Unsupported operating system.");
        }
    }
}