using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Collections.Specialized;
using System.Linq;
using System.IO;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using MxBots.Properties;

namespace MxBots
{
    public partial class XC_XbConfig : UserControl
    {
        public XC_XbConfig()
        {
            InitializeComponent();
            if (Settings.Default.showconsole == false)
            {
                checkBox1.Checked = false;
            }
            if (Settings.Default.showballoon == false)
            {
                checkBox2.Checked = false;
            }
            checkBox3.Checked = Settings.Default.music;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.showconsole = checkBox1.Checked;
            
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.showballoon = checkBox2.Checked;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.music = checkBox3.Checked;
         
        }
    }
}
