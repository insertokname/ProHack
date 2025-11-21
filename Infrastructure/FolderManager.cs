using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class FolderManager
    {
        public static string LocalDownloads()
        {
            var tmp = Path.Combine(Environment.CurrentDirectory, "LocalDownloads");
            if (!Directory.Exists(tmp))
            {
                Directory.CreateDirectory(tmp);
            }
            return tmp;
        }
    }
}