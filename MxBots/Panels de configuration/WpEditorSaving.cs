using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MxBots.Panels_de_configuration
{
    public partial class WpEditorSaving : Form
    {
        public WpEditorSaving()
        {
            InitializeComponent();
        }

        private void WpEditorSaving_Load(object sender, EventArgs e)
        {

        }
        public string Reponse
        {
            get { return textBox1.Text; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
