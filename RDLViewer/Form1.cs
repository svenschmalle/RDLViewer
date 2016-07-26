using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RDLViewer.classes;

namespace RDLViewer
{
	public partial class Form1 : Form
	{
        private string[] _args;
        private string _ConfigDatei;
        private config _config;
        private DataSet _ReportDataSet;

        public Form1(string[] args)
		{
            InitializeComponent();
            _args = args;

            // ###### Test ######
            //_args = new string[1];
            //_args[0] = Path.Combine(Application.StartupPath, "neu.rdlv");

            functions.Init();

            if (_args.Length > 0)
            {
                functions.MakePath(functions.AppDataPath);

                _config = new config();
                _ConfigDatei = functions.GetConfigDatei(_args[0]);
                // Vorher gespeicherte Config laden, falls vorhanden 
                if (File.Exists(_ConfigDatei))
                {
                    _config = _config.laden(_ConfigDatei);
                    this.Top = _config.FormTop;
                    this.Left = _config.FormLeft;
                    this.Height = _config.FormHeight;
                    this.Width = _config.FormWidth;
                    this.WindowState = _config.FormWindowState;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
		{
            
            _ReportDataSet = new DataSet();
            string RDLBericht = "";

            if (_args.Length > 0)
            {
                _ReportDataSet.ReadXml(_args[0]);
                RDLBericht = Path.Combine(Application.StartupPath, "Reports", Path.GetFileNameWithoutExtension(_args[0]) + ".rdl");
            }

            if (File.Exists(RDLBericht))
            {
                foreach (DataTable dt in _ReportDataSet.Tables)
                {
                    ReportDataSource rds = new ReportDataSource(dt.TableName, dt);
                    this.reportViewer1.LocalReport.DataSources.Add(rds);
                }

                this.reportViewer1.LocalReport.ReportPath = RDLBericht;
                this.reportViewer1.RefreshReport();
            }
            else
            {
                // Wenn der Bericht nicht gefunden wird, dann soll die leer.rdl mit der XML-Definition der Daten angezeigt werden
                DataTable Leerdt = new DataTable("Berichtsdaten");
                Leerdt.Columns.Add("Tabellenname");
                Leerdt.Columns.Add("XMLDefinition");
                Leerdt.Columns.Add("ReportPfad");
                Leerdt.Columns.Add("ReportDatei");
                string tmpfile = Path.Combine(functions.AppDataPath, "FBRDLViewer.tmp");

                foreach (DataTable dt in _ReportDataSet.Tables)
                {
                    DataRow Leerdr = Leerdt.NewRow();
                    Leerdr["TabellenName"] = dt.TableName;
                    Leerdr["ReportPfad"] = Path.GetDirectoryName(RDLBericht);
                    Leerdr["ReportDatei"] = Path.GetFileName(RDLBericht);
                    Leerdr["XMLDefinition"] = DataTableRDLDefinition(dt);
                    Leerdt.Rows.Add(Leerdr);
                }

                ReportDataSource rdsleer = new ReportDataSource(Leerdt.TableName, Leerdt);
                this.reportViewer1.LocalReport.DataSources.Add(rdsleer);
                this.reportViewer1.LocalReport.ReportEmbeddedResource = "RDLViewer.Reports.leer.rdl";
                this.reportViewer1.LocalReport.EnableHyperlinks = true;
                this.reportViewer1.RefreshReport();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F4)
            {
                string output = "";

                foreach (DataTable dt in _ReportDataSet.Tables)
                {
                    output += "DataTable \"" + dt.TableName + "\"" + Environment.NewLine;
                    output += DataTableRDLDefinition(dt);
                    output += Environment.NewLine;
                    output += "------------------------------------------------------" + Environment.NewLine;
                    output += Environment.NewLine + Environment.NewLine;
                }
                
                FormXMLAusgabe frm_xmlausgabe = new FormXMLAusgabe(output);
                frm_xmlausgabe.Show();
            }
        }

        private void ErstelleNeuenBericht(string Berichtsname)
        {
            string BerichtRDL = functions.getResourceFile("RDLViewer.Reports.neu.rdl");
            string DatasetRDL = "";

            foreach (DataTable dt in _ReportDataSet.Tables)
            {
                string Commandtext = System.Security.SecurityElement.Escape(DataTableRDLDefinition(dt));
                DatasetRDL += "  <DataSet Name=\"" + dt.TableName + "\"><Query><DataSourceName>DataSource1</DataSourceName><CommandText>" + Commandtext + Environment.NewLine + "</CommandText></Query>" + Environment.NewLine;
                
                DatasetRDL += "<Fields>";
                foreach (DataColumn col in dt.Columns)
                {
                    DatasetRDL += "<Field Name=\"" + col.ColumnName + "\"><DataField>" + col.ColumnName + "</DataField><rd:TypeName>System.String</rd:TypeName></Field>" + Environment.NewLine;
                }
                DatasetRDL += "</Fields>";
                
                DatasetRDL += "</DataSet>";
            }

            BerichtRDL = BerichtRDL.Replace("[HEADER]", "Neuer Bericht vom "+DateTime.Now.ToShortDateString());
            BerichtRDL = BerichtRDL.Replace("<!-- DATASETRDL -->", DatasetRDL);
            File.WriteAllText(Path.Combine(functions.ReportPath,Berichtsname),BerichtRDL);
        }

        private string DataTableRDLDefinition(DataTable dt)
        {
            string tmpfile = Path.Combine(functions.AppDataPath, "FBRDLViewer.tmp");
            dt.WriteXml(tmpfile);
            string output = File.ReadAllText(tmpfile);
            output = output.Replace("<?xml version=\"1.0\" standalone=\"yes\"?>", "");
            output = output.Replace("<" + _ReportDataSet.DataSetName + ">", "<Query>" + Environment.NewLine + "<XmlData>" + Environment.NewLine + "<Root>");
            output = output.Replace("</" + _ReportDataSet.DataSetName + ">", "</Root>" + Environment.NewLine + "</XmlData>" + Environment.NewLine + "</Query>");
            return output;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_args.Length > 0)
            {
                // Config speichern
                if (this.WindowState == FormWindowState.Normal)
                {
                    _config.FormTop = this.Top;
                    _config.FormLeft = this.Left;
                    _config.FormHeight = this.Height;
                    _config.FormWidth = this.Width;
                }
                _config.FormWindowState = this.WindowState;
                _config.speichern(_ConfigDatei);
            }
        }

        private void reportViewer1_Hyperlink(object sender, HyperlinkEventArgs e)
        {
            if (e.Hyperlink.StartsWith("copy2clipboard:"))
            {
                string output = e.Hyperlink.Replace("copy2clipboard:", "");
                output = System.Uri.UnescapeDataString(output);
                Clipboard.SetText(output);
                MessageBox.Show("Die Datendefinition wurde in die Zwischenablage eingefügt.", "RDLViewer", MessageBoxButtons.OK);
            }

            if (e.Hyperlink.StartsWith("newreport:"))
            {
                string ReportDateiname = e.Hyperlink.Replace("newreport:", "");
                ReportDateiname = System.Uri.UnescapeDataString(ReportDateiname);
                ErstelleNeuenBericht(ReportDateiname);

                DialogResult dlrslt = MessageBox.Show("Der Report " + ReportDateiname + " wurde in das Verzeichnis \"" + functions.ReportPath + "\" hinzugefügt."+
                    Environment.NewLine+
                    "Soll dieser Bericht mit der Standard-Anwendung für die Dateiendung \"."+Path.GetExtension(ReportDateiname)+"\" geöffnet werden?", 
                    "RDLViewer", MessageBoxButtons.YesNoCancel);

                if (dlrslt == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(Path.Combine(functions.ReportPath,ReportDateiname));
                }
            }

        }
    }
}
