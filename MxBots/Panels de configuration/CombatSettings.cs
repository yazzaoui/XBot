using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MxBots
{
    public partial class CombatSettings : UserControl
    {
        
        public CombatSettings()
        {
            InitializeComponent();
            foreach (TreeNode n in treeView1.Nodes)
            {
                n.Nodes.Add("General Configuration");
               // n.Nodes.Add("Keyboard Configuration");
                n.Nodes.Add("Routines Configuration");
            }
   
                if (ClassSettings.Default.MaxTargetDistance == null || ClassSettings.Default.PreCombatTick == null)
                {
                    ClassSettings.Default.MaxTargetDistance = new System.Collections.ArrayList();
                    ClassSettings.Default.MinTargetDistance = new System.Collections.ArrayList();
                    ClassSettings.Default.DistanceDuMain = new System.Collections.ArrayList();
                    ClassSettings.Default.PreCombatTick = new System.Collections.ArrayList();
             
                for (int i = 0; i < 10; i++)
                {

                    BotConfig SConf = new BotConfig();
                    int precombatTick = 0;
                    switch (i)
                    {
                        case (int)Classes.Mage:
                            SConf.FollowDistance = 7;
                            SConf.maxDistance = 30;
                            SConf.minDistance = 15;
                            precombatTick = 2000;
                            break;
                        case (int)Classes.Chasseur:
                            SConf.FollowDistance = 1;
                            SConf.maxDistance = 3;
                            SConf.minDistance = 1;
                            break;
                        case (int)Classes.Démoniste:
                            SConf.FollowDistance = 1;
                            SConf.maxDistance = 3;
                            SConf.minDistance = 1;
                            break;
                        case (int)Classes.DK:
                            SConf.FollowDistance = 1;
                            SConf.maxDistance = 3;
                            SConf.minDistance = 1;
                            break;
                        case (int)Classes.Druide:
                            SConf.FollowDistance = 1;
                            SConf.maxDistance = 3;
                            SConf.minDistance = 1;
                            break;
                        case (int)Classes.Guerrier:
                            SConf.FollowDistance = 1;
                            SConf.maxDistance = 3;
                            SConf.minDistance = 1;
                            break;
                        case (int)Classes.Paladin:
                            SConf.FollowDistance = 1;
                            SConf.maxDistance = 3;
                            SConf.minDistance = 1;
                            break;
                        case (int)Classes.Prêtre:
                            SConf.FollowDistance = 1;
                            SConf.maxDistance = 3;
                            SConf.minDistance = 1;
                            break;

                        case (int)Classes.Shaman:
                            SConf.FollowDistance = 1;
                            SConf.maxDistance = 3;
                            SConf.minDistance = 1;
                            break;
                        case (int)Classes.Voleur:
                            SConf.FollowDistance = 1;
                            SConf.maxDistance = 3;
                            SConf.minDistance = 1;
                            break;
                    }
                 
                    ClassSettings.Default.DistanceDuMain.Add(SConf.FollowDistance);
                    ClassSettings.Default.MaxTargetDistance.Add(SConf.maxDistance);
                    ClassSettings.Default.MinTargetDistance.Add(SConf.minDistance);
                    ClassSettings.Default.PreCombatTick.Add(precombatTick);
                }

            }


        }

        private void CombatSettings_Load(object sender, EventArgs e)
        {
            
           // treeView1.SelectedNode = treeView1.Nodes[0];
            AllInvisisible();
            
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ClickSurArbre(treeView1.SelectedNode.Parent.Text);

         
        }

        private void ClickSurArbre(string texte)
        {
            AllInvisisible();
            if (treeView1.SelectedNode.Text == "Routines Configuration")
            {
                userconfig_routines1.Visible = true;
                userconfig_routines1.SwicthClass(texte);
            }
            else
            {
                if (texte == "Mage")
                {
                    mageSettings1.Visible = true;
                }
                else if (texte == "Warrior")
                {
                    warrior1.Visible = true;
                }
                else if (texte == "Warlock")
                {
                    warlock1.Visible = true;
                }
                else if (texte == "Hunter")
                {
                    hunter1.Visible = true;
                }
                else if (texte == "Druid")
                {
                    druid1.Visible = true;
                }
                else if (texte == "Shaman")
                {
                    shaman1.Visible = true;
                }
                else if (texte == "Rogue")
                {
                    rogue1.Visible = true;
                }
                else if (texte == "Death Knight")
                {
                    deathKnight1.Visible = true;
                }
                else if (texte == "Priest")
                {
                    priest1.Visible = true;
                }
                else if (texte == "Paladin")
                {
                    paladin1.Visible = true;
                }


            }
        }
        private void AllInvisisible()
        {
            foreach (UserControl c in splitContainer1.Panel2.Controls)
            {
                c.Visible = false;
            }
        }

        private void userconfig_routines1_Load(object sender, EventArgs e)
        {

        }
        public void save()
        {
            this.userconfig_routines1.SaveRoutines();
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {

        }
    }
}
