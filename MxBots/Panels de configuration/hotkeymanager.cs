using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using MxBots.Properties;

namespace MxBots
{
    public partial class hotkeymanager : UserControl
    {
        #region variables
        private KeyEventArgs keyassignation;
        private int nombreditems;
        public ArrayList listOfHotkeys;
        private bool Firstuse;
        #endregion

        public hotkeymanager()
        {
            
            InitializeComponent();
            Firstuse = false;
            HKMDB lol = new HKMDB(new KeyEventArgs(Keys.Control | Keys.Enter), 1);
            nombreditems = HKMLIST.NOMBREDEHOTKEYS;
            if (Settings.Default.first_use == true)
            {
                Firstuse = true;
                listOfHotkeys = new System.Collections.ArrayList();
                listOfHotkeys.Capacity = nombreditems;
                for (int arg = 0; arg < nombreditems; arg++)
                {
                    listOfHotkeys.Add(HKMLIST.HKMLISTDEF[arg]);
                }

            }
            else
             {
                 listOfHotkeys = HKMDB.deserialize();
                if ( listOfHotkeys.Count != nombreditems)
                {
                   
                    int anciennombreditems = listOfHotkeys.Capacity;
                    listOfHotkeys.Capacity = nombreditems;
                    for (int arg = anciennombreditems; arg < listOfHotkeys.Capacity; arg++)
                    {
                        listOfHotkeys.Add(HKMLIST.HKMLISTDEF[arg]);
                    }
                }

            }
        
     
            foreach (HKMDB h in HKMLIST.HKMLISTDEF)
            {
                listBox1.Items.Add(h.DonnemoiladéfinitionConnard());
          
            }
            listBox1.SelectedIndex = 0;
      
        }
        private void hotkeymanager_Load(object sender, EventArgs e)
        {

        }
        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            textBox3.Text = HKMDB.KeyToString(e);
            keyassignation = e;
            
        }
       
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox3.Text != string.Empty)
            {
                if (keyassignation != null)
                {
                    bool accep = true;
                    for (int i = 0; i < nombreditems; i++)
                    {

                        HKMDB Truc = (HKMDB) listOfHotkeys[i];
                        if (Truc.KeyToString() == HKMDB.KeyToString(keyassignation) && i != listBox1.SelectedIndex)
                        {
                            accep = false;
                            MessageBox.Show("Already Choosen!", "Error");
                        }

                    }
                    if (accep == true)
                    {
                        listOfHotkeys[listBox1.SelectedIndex] = new HKMDB(keyassignation, listBox1.SelectedIndex);
                        
                    }
                }
                else
                {
                    MessageBox.Show("Erreur", "Erreur Impossible");
                }
            }
            
            refresh();
            
        }
        private void refresh()
        {
            keyassignation = null;
            HKMDB Truc = (HKMDB)listOfHotkeys[listBox1.SelectedIndex];
           
            textBox1.Text = Truc.KeyToString();
            textBox3.Text = string.Empty;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            refresh();
            
         
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listOfHotkeys[listBox1.SelectedIndex] = HKMLIST.HKMLISTDEF[listBox1.SelectedIndex];
            refresh();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
