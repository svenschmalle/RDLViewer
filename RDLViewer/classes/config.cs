using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RDLViewer.classes
{

    public class config
    {
        public int FormTop { get; set; }
        public int FormLeft { get; set; }
        public int FormHeight { get; set; }
        public int FormWidth { get; set; }
        public FormWindowState FormWindowState { get; set; }

        #region Config Laden

        public config laden(string configdatei)
        {
            string ConfigDatei = File.ReadAllText(configdatei);
            return JSON.JSONDeserialise<config>(ConfigDatei);
        }

        #endregion

        #region Config Speichern
        public void speichern(string configdatei)
        {
            File.WriteAllText(configdatei, JSON.JSONSerialize<config>(this));
        }
        #endregion 
    }

    
}
