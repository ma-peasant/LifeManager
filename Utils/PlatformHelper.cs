using System.Runtime.InteropServices;

namespace LifeManager.Utils
{
    public  class PlatformHelper
    {
        public static string GetPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "Windows";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "Linux";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "macOS";
            }
            else
            {
                return "Unknown";
            }
        }
    }
}
