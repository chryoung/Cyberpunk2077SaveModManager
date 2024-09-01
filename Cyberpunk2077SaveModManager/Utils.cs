using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyberpunk2077SaveModManager
{
    public class Utils
    {
        public static string FormatSize(long size) =>
            size switch
            {
                < 0 => string.Empty,
                < 1024 => $"{size}B",
                < 1048576 => $"{size / 1024.0:F2}KB",
                < 1073741824L => $"{size / 1048576.0:F2}MB",
                < 1099511627776L => $"{size / 1073741824.0:F2}GB",
                _ => $"{size}B"
            };
    }
}
