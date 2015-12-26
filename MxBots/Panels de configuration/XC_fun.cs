using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MxBots.Properties;

namespace MxBots.Panels_de_configuration.ClassesCustomSettings
{
    public partial class XC_fun : UserControl
    {
        public XC_fun()
        {
            InitializeComponent();
            checkBox1.Checked = Settings.Default.FunSounds;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.FunSounds = checkBox1.Checked;
        }
    }
}
