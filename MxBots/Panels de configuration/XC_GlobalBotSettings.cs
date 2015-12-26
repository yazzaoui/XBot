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

namespace MxBots
{
    public partial class XC_GlobalBotSettings : UserControl
    {
        FolderBrowserDialog D2;
        string DirectoryRoutines;
        public XC_GlobalBotSettings()
        {
            InitializeComponent();
            D2 = new FolderBrowserDialog();
            D2.SelectedPath = Path.GetDirectoryName(Application.ExecutablePath);
           // Settings.Default.first_use = true;
            for (int i = 1; i < 10; i++)
            {
                comboBox1.Items.Add(i);
            }
            comboBox1.SelectedItem = Settings.Default.fastmainswitch;
            if (Settings.Default.first_use == true)
            {
                DirectoryRoutines = Path.GetDirectoryName(Application.ExecutablePath) + "\\routines";
                textBox1.Text = DirectoryRoutines;
            }
            else
            {
                DirectoryRoutines = Settings.Default.routines_dir;
                textBox1.Text = DirectoryRoutines;
            }
            checkBox1.Checked = Settings.Default.Autorest;
            toolTip1.SetToolTip(textBox1,DirectoryRoutines);
            checkBox2.Checked = Settings.Default.switchloc;
            checkBox3.Checked = Settings.Default.allowstacktarget;
            checkBox4.Checked = Settings.Default.Passif;
            //toolTip1.Show
        }

        private void XC_GlobalBotSettings_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {

            D2.Description = "Select the classes routines folder : ";
            
            if(D2.ShowDialog() == DialogResult.OK)
            {
                DirectoryRoutines = D2.SelectedPath;
                textBox1.Text = DirectoryRoutines;
                Settings.Default.routines_dir = DirectoryRoutines;

                toolTip1.SetToolTip(textBox1, DirectoryRoutines);
            }



        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.Autorest = checkBox1.Checked;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Default.fastmainswitch = (int)comboBox1.SelectedItem;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.switchloc = checkBox2.Checked;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.allowstacktarget = checkBox3.Checked;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.Passif = checkBox4.Checked;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        MxBots.Panels_de_configuration.WpEditor wp;
        private void button2_Click(object sender, EventArgs e)
        {
            wp = new MxBots.Panels_de_configuration.WpEditor();
            wp.Show();
            
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            wp = new MxBots.Panels_de_configuration.WpEditor();
            wp.Show();
        }
    }
}
