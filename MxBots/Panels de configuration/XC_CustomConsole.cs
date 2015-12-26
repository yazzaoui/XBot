using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using MxBots.Properties;

namespace MxBots.Panels_de_configuration
{
    public partial class XC_CustomConsole : UserControl
    {
        ColorDialog Colordialog;
        FontDialog Fontdialog;      
  
        public XC_CustomConsole()
        {
            InitializeComponent();
            Colordialog = new ColorDialog();
            Fontdialog = new FontDialog();
            checkBox2.Checked = Settings.Default.consolesavelocation;
            pictureBox1.BackColor = Settings.Default.consolebackgroundcolor;
            pictureBox2.BackColor = Settings.Default.consolefontcolor;
            textBox2.Text = Settings.Default.consolefont.Name + " " + Settings.Default.consolefont.Size.ToString();
            checkBox1.Checked = Settings.Default.debug_mode;
            comboBox1.SelectedIndex = Settings.Default.verbose;

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Colordialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.BackColor = Colordialog.Color;
                Settings.Default.consolebackgroundcolor = Colordialog.Color;
            }
        }

    

        private void button3_Click(object sender, EventArgs e)
        {
            if (Colordialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox2.BackColor = Colordialog.Color;
                Settings.Default.consolefontcolor = Colordialog.Color;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Fontdialog.Font = Settings.Default.consolefont;
            if (Fontdialog.ShowDialog() == DialogResult.OK)
            {
                Settings.Default.consolefont = Fontdialog.Font;
                textBox2.Text= Settings.Default.consolefont.Name + " " + Settings.Default.consolefont.Size.ToString();
                
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.consolesavelocation = checkBox2.Checked;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.debug_mode = checkBox1.Checked;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Default.verbose = comboBox1.SelectedIndex;
        }
    }
}
