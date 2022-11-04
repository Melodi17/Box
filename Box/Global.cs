using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Box
{
    public static class Global
    {
        public static string DataPath;
        public static string LibDataPath;

        public static string PackageDownloadUrl;
        static Global()
        {
            PackageDownloadUrl = "https://github.com/Melodi17/Ream/raw/main/Libraries/";

            DataPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Ream");
            LibDataPath = Path.Join(DataPath, "Libraries");

            Directory.CreateDirectory(DataPath);
            Directory.CreateDirectory(LibDataPath);
        }
    }
}
