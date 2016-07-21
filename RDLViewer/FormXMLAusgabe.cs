using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RDLViewer
{
    public partial class FormXMLAusgabe : Form
    {
        private string _XMLText;
        public FormXMLAusgabe(string XMLText)
        {
            InitializeComponent();
            _XMLText = XMLText;
        }

        private void FormXMLAusgabe_Load(object sender, EventArgs e)
        {
            textBox1.Text = _XMLText;
        }
    }
}
