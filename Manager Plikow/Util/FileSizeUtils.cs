using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager_Plikow.Util
{
    class FileSizeUtils
    {
        public static string ByteToHumanReadable(long bytes)
        {
            if (bytes == -1)
            {
                return "";
            }
            else if (bytes < 1024)
            {
                return string.Format("{0:f2} B", bytes);
            }
            else if (bytes < 1024 * 1024)
            {
                return string.Format("{0:f2} KB", bytes / 1024);
            }
            else if (bytes < 1024 * 1024 * 1024)
            {
                return string.Format("{0:f2} MB", bytes / (1024 * 1024));
            }
            else
            {
                return string.Format("{0:f2} GB", bytes / (1024 * 1024 * 1024));
            }
        }

    }
}
