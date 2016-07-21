using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDLViewer.classes
{
    internal static class functions
    {
        internal static string GetAppPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FBRDLViewer");
        }

        internal static void MakePath(string Pfad)
        {
            if (!Directory.Exists(Pfad))
            {
                Directory.CreateDirectory(Pfad);
            }
        }

        internal static string GetConfigDatei(string ReportDatei)
        {
            return Path.Combine(GetAppPath(), Path.GetFileNameWithoutExtension(ReportDatei) +".json");
        }

    }
}
