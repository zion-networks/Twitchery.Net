using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TwitcheryNet.Misc;

public static class WebTools
{
    public static void OpenUrl(string url)
    {
        try
        {
            Process.Start(url);
        }
        catch
        {
            // hack because of this: https://github.com/dotnet/corefx/issues/10361
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Related to https://github.com/zion-networks/BetterRaid/issues/4
                // Won't work for Opera GX
                // url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                Console.WriteLine("Failed to start your browser.");
                Console.WriteLine("Please open the following URL in your browser: {0}", url);
            }
        }
    }
}