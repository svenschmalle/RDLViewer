using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RDLViewer.classes
{
    internal static class functions
    {
        public static string AppDataPath { get; set; }
        public static string ReportPath { get; set; }

        internal static void Init()
        {
            AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FBRDLViewer");
            ReportPath = Path.Combine(Application.StartupPath,"Reports");
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
            return Path.Combine(AppDataPath, Path.GetFileNameWithoutExtension(ReportDatei) +".json");
        }

        internal static string getResourceFile(string Filename)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            using (StreamReader textStreamReader = new StreamReader(asm.GetManifestResourceStream(Filename)))
            {
                return textStreamReader.ReadToEnd();
            }
        }
    }
}
