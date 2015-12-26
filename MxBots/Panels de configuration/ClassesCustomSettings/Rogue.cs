using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MxBots.Panels_de_configuration.ClassesCustomSettings
{
    public partial class Rogue : UserControl
    {
        public Rogue()
        {
            InitializeComponent();
        }

        private void Rogue_Load(object sender, EventArgs e)
        {
            //textBox1.Text = ClassSettings.Default.RogueMainDist.ToString();
            textBox1.Text = ClassSettings.Default.DistanceDuMain[(int)Classes.Voleur].ToString();
            textBox2.Text = ClassSettings.Default.MinTargetDistance[(int)Classes.Voleur].ToString();
            textBox3.Text = ClassSettings.Default.MaxTargetDistance[(int)Classes.Voleur].ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //ClassSettings.Default.RogueMainDist = Int32.Parse(textBox1.Text);
            ClassSettings.Default.DistanceDuMain[(int)Classes.Voleur] = Int32.Parse(textBox1.Text);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            ClassSettings.Default.MinTargetDistance[(int)Classes.Voleur] = Int32.Parse(textBox2.Text);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            ClassSettings.Default.MaxTargetDistance[(int)Classes.Voleur] = Int32.Parse(textBox3.Text);
        }
    }
}
