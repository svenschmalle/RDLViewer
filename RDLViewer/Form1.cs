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

namespace RDLViewer
{
	public partial class Form1 : Form
	{
        private string[] _args;
        private DataSet _ReportDataSet;

        public Form1(string[] args)
		{
            InitializeComponent();
            _args = args;
		}

        private void Form1_Load(object sender, EventArgs e)
		{
            if (_args.Length == 0)
            {
                MessageBox.Show("Kein Druckbericht angegeben","Fehler!",MessageBoxButtons.OK,MessageBoxIcon.Stop);
                Application.Exit();
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                _ReportDataSet = new DataSet();
                _ReportDataSet.ReadXml(_args[0]);

                foreach (DataTable dt in _ReportDataSet.Tables)
                {
                    ReportDataSource rds = new ReportDataSource(dt.TableName, dt);
                    this.reportViewer1.LocalReport.DataSources.Add(rds);
                }

                this.reportViewer1.SetDisplayMode(DisplayMode.PrintLayout);
                this.reportViewer1.LocalReport.ReportPath = Path.Combine(Application.StartupPath, "Reports", Path.GetFileNameWithoutExtension(_args[0]) + ".rdl");
                this.reportViewer1.RefreshReport();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F4)
            {
                string tmpfile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FBRDLViewer.tmp");
                string output = "";

                foreach (DataTable dt in _ReportDataSet.Tables)
                {
                    output += "DataTable \"" + dt.TableName + "\"" + Environment.NewLine;
                    dt.WriteXml(tmpfile);
                    output += File.ReadAllText(tmpfile);
                    output = output.Replace("<?xml version=\"1.0\" standalone=\"yes\"?>", "------------------------------------------------------");
                    output = output.Replace("<" + _ReportDataSet.DataSetName + ">", "<Query>" + Environment.NewLine + "<XmlData>" + Environment.NewLine + "<Root>");
                    output = output.Replace("</" + _ReportDataSet.DataSetName + ">", "</Root>" + Environment.NewLine + "</XmlData>" + Environment.NewLine + "</Query>");
                    output += Environment.NewLine;
                    output += "------------------------------------------------------" + Environment.NewLine;
                    output += Environment.NewLine + Environment.NewLine;
                }
                
                FormXMLAusgabe frm_xmlausgabe = new FormXMLAusgabe(output);
                frm_xmlausgabe.Show();
            }
        }
    }
}
