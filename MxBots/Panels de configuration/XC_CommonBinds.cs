using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;

namespace MxBots.Panels_de_configuration
{
    public partial class XC_CommonBinds : UserControl
    {
        public XC_CommonBinds()
        {
            InitializeComponent();
          
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            textBox1.Text = HKMDB.KeyToString(e);

            Form1.ModifyFaction(e, "Common.Assist");
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            textBox2.Text = HKMDB.KeyToString(e);
            Form1.ModifyFaction(e, "Common.StrafeLeft");
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            textBox3.Text = HKMDB.KeyToString(e);
            Form1.ModifyFaction(e, "Common.StrafeRight");
        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            textBox4.Text = HKMDB.KeyToString(e);
            Form1.ModifyFaction(e, "Common.Jump");
          
        }




        private void XC_CommonBinds_Load(object sender, EventArgs e)
        {
       //Chargement();
        }

        //private void Chargement()
        //{
           
        //        XmlNodeList Liste = SomeSettings.Faction;
        //        foreach (XmlNode faction in Liste)
        //        {
        //            string mod = "";
        //            if (faction.Attributes[1].InnerText != "None")
        //            {
        //                mod = faction.Attributes[1].InnerText + " + ";
        //            }

        //            switch (faction.Attributes[0].InnerText)
        //            {

        //                case "Common.Assist":
        //                    textBox1.Text = mod + faction.Attributes[3].InnerText;
        //                    break;
        //                case "Common.StrafeLeft":
        //                    textBox2.Text = mod + faction.Attributes[3].InnerText;
        //                    break;
        //                case "Common.StrafeRight":
        //                    textBox3.Text = mod + faction.Attributes[3].InnerText;
        //                    break;
        //                case "Common.Jump":
        //                    textBox4.Text = mod + faction.Attributes[3].InnerText;
        //                    break;
        //                case "Common.PvP":
        //                    textBox5.Text = mod + faction.Attributes[3].InnerText;
        //                    break;
        //            }
        //        }
            
         
        //}

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_KeyDown(object sender, KeyEventArgs e)
        {
            textBox5.Text = HKMDB.KeyToString(e);
            Form1.ModifyFaction(e, "Common.PvP");
        }

    }
}
