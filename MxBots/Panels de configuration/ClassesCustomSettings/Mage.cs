using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MxBots.Properties;

namespace MxBots
{
    public partial class MageSettings : UserControl
    {
        public Classes Classe;
        public MageSettings()
        {
            InitializeComponent();
            
        }


        private void MageSettings_Load(object sender, EventArgs e)
        {

            textBox1.Text = ClassSettings.Default.DistanceDuMain[(int)Classes.Mage].ToString();
            textBox2.Text = ClassSettings.Default.MinTargetDistance[(int)Classes.Mage].ToString();
            textBox3.Text = ClassSettings.Default.MaxTargetDistance[(int)Classes.Mage].ToString();
            checkBox1.Checked = ClassSettings.Default.MageWaitTankPull;
            textBox4.Text = ClassSettings.Default.MageTimeToWaitTankAgro.ToString();
            textBox5.Text = ClassSettings.Default.PreCombatTick[(int)Classes.Mage].ToString();
            if (!checkBox1.Checked)
            {
                textBox4.Enabled = false;
            }
            

        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            ClassSettings.Default.DistanceDuMain[(int)Classes.Mage] = Int32.Parse(textBox1.Text);


        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            ClassSettings.Default.MinTargetDistance[(int)Classes.Mage] = Int32.Parse(textBox2.Text);

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            ClassSettings.Default.MaxTargetDistance[(int)Classes.Mage] = Int32.Parse(textBox3.Text);

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ClassSettings.Default.MageWaitTankPull = checkBox1.Checked;
            if (checkBox1.Checked)
            {
                textBox4.Enabled=true;
            }
            else
            {
                textBox4.Enabled=false;
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            int n;
           bool success = int.TryParse(textBox4.Text, out  n);
           if (success)
           {
               ClassSettings.Default.MageTimeToWaitTankAgro = n;
           }
           else
           {
               MessageBox.Show("Please enter a valid number");
           }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            ClassSettings.Default.PreCombatTick[(int)Classes.Mage] = int.Parse(textBox5.Text);
        }
        
    }
}
