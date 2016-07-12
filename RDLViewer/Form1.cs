using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
                DataSet ds = new DataSet();
                ds.ReadXml(_args[0]);
                ReportDataSource rds = new ReportDataSource("Kopfdaten", ds.Tables["Kopfdaten"]);
                ReportDataSource rdsPosten = new ReportDataSource("Posten", ds.Tables["Posten"]);
                this.reportViewer1.LocalReport.DataSources.Add(rds);
                this.reportViewer1.LocalReport.DataSources.Add(rdsPosten);
                this.reportViewer1.SetDisplayMode(DisplayMode.PrintLayout);
                this.reportViewer1.LocalReport.ReportPath = Path.Combine(Application.StartupPath, "Reports", Path.GetFileNameWithoutExtension(_args[0]) + ".rdl");
                this.reportViewer1.RefreshReport();
            }
        }

        private void reportViewer1_Load(object sender, EventArgs e)
        {

        }
    }
}
