namespace MxBots
{
    partial class CombatSettings
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.TreeNode treeNode91 = new System.Windows.Forms.TreeNode("Warrior");
            System.Windows.Forms.TreeNode treeNode92 = new System.Windows.Forms.TreeNode("Priest");
            System.Windows.Forms.TreeNode treeNode93 = new System.Windows.Forms.TreeNode("Mage");
            System.Windows.Forms.TreeNode treeNode94 = new System.Windows.Forms.TreeNode("Paladin");
            System.Windows.Forms.TreeNode treeNode95 = new System.Windows.Forms.TreeNode("Shaman");
            System.Windows.Forms.TreeNode treeNode96 = new System.Windows.Forms.TreeNode("Rogue");
            System.Windows.Forms.TreeNode treeNode97 = new System.Windows.Forms.TreeNode("Death Knight");
            System.Windows.Forms.TreeNode treeNode98 = new System.Windows.Forms.TreeNode("Warlock");
            System.Windows.Forms.TreeNode treeNode99 = new System.Windows.Forms.TreeNode("Druid");
            System.Windows.Forms.TreeNode treeNode100 = new System.Windows.Forms.TreeNode("Hunter");
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.paladin1 = new MxBots.Panels_de_configuration.Paladin();
            this.warrior1 = new MxBots.Panels_de_configuration.ClassesCustomSettings.Warrior();
            this.warlock1 = new MxBots.Panels_de_configuration.ClassesCustomSettings.Warlock();
            this.shaman1 = new MxBots.Panels_de_configuration.ClassesCustomSettings.Shaman();
            this.rogue1 = new MxBots.Panels_de_configuration.ClassesCustomSettings.Rogue();
            this.priest1 = new MxBots.Panels_de_configuration.ClassesCustomSettings.Priest();
            this.hunter1 = new MxBots.Panels_de_configuration.ClassesCustomSettings.Hunter();
            this.druid1 = new MxBots.Panels_de_configuration.ClassesCustomSettings.Druid();
            this.deathKnight1 = new MxBots.Panels_de_configuration.ClassesCustomSettings.DeathKnight();
            this.mageSettings1 = new MxBots.MageSettings();
            this.userconfig_routines1 = new MxBots.Userconfig_routines();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.Color.Black;
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.ForeColor = System.Drawing.SystemColors.Info;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            treeNode91.Name = "Warrior";
            treeNode91.Text = "Warrior";
            treeNode92.Name = "Priest";
            treeNode92.Text = "Priest";
            treeNode93.Name = "Mage";
            treeNode93.Text = "Mage";
            treeNode94.Name = "Paladin";
            treeNode94.Text = "Paladin";
            treeNode95.Name = "Shaman";
            treeNode95.Text = "Shaman";
            treeNode96.Name = "Rogue";
            treeNode96.Text = "Rogue";
            treeNode97.Name = "DK";
            treeNode97.Text = "Death Knight";
            treeNode98.Name = "Warlock";
            treeNode98.Text = "Warlock";
            treeNode99.Name = "Druid";
            treeNode99.Text = "Druid";
            treeNode100.Name = "Hunter";
            treeNode100.Text = "Hunter";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode91,
            treeNode92,
            treeNode93,
            treeNode94,
            treeNode95,
            treeNode96,
            treeNode97,
            treeNode98,
            treeNode99,
            treeNode100});
            this.treeView1.Size = new System.Drawing.Size(168, 328);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.userconfig_routines1);
            this.splitContainer1.Panel2.Controls.Add(this.paladin1);
            this.splitContainer1.Panel2.Controls.Add(this.warrior1);
            this.splitContainer1.Panel2.Controls.Add(this.warlock1);
            this.splitContainer1.Panel2.Controls.Add(this.shaman1);
            this.splitContainer1.Panel2.Controls.Add(this.rogue1);
            this.splitContainer1.Panel2.Controls.Add(this.priest1);
            this.splitContainer1.Panel2.Controls.Add(this.hunter1);
            this.splitContainer1.Panel2.Controls.Add(this.druid1);
            this.splitContainer1.Panel2.Controls.Add(this.deathKnight1);
            this.splitContainer1.Panel2.Controls.Add(this.mageSettings1);
            this.splitContainer1.Size = new System.Drawing.Size(504, 328);
            this.splitContainer1.SplitterDistance = 168;
            this.splitContainer1.TabIndex = 2;
            // 
            // paladin1
            // 
            this.paladin1.BackColor = System.Drawing.Color.Black;
            this.paladin1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paladin1.Location = new System.Drawing.Point(0, 0);
            this.paladin1.Name = "paladin1";
            this.paladin1.Size = new System.Drawing.Size(332, 328);
            this.paladin1.TabIndex = 11;
            // 
            // warrior1
            // 
            this.warrior1.BackColor = System.Drawing.Color.Black;
            this.warrior1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.warrior1.Location = new System.Drawing.Point(0, 0);
            this.warrior1.Name = "warrior1";
            this.warrior1.Size = new System.Drawing.Size(332, 328);
            this.warrior1.TabIndex = 10;
            // 
            // warlock1
            // 
            this.warlock1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.warlock1.Location = new System.Drawing.Point(0, 0);
            this.warlock1.Name = "warlock1";
            this.warlock1.Size = new System.Drawing.Size(332, 328);
            this.warlock1.TabIndex = 9;
            // 
            // shaman1
            // 
            this.shaman1.BackColor = System.Drawing.Color.Black;
            this.shaman1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.shaman1.Location = new System.Drawing.Point(0, 0);
            this.shaman1.Name = "shaman1";
            this.shaman1.Size = new System.Drawing.Size(332, 328);
            this.shaman1.TabIndex = 8;
            // 
            // rogue1
            // 
            this.rogue1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rogue1.Location = new System.Drawing.Point(0, 0);
            this.rogue1.Name = "rogue1";
            this.rogue1.Size = new System.Drawing.Size(332, 328);
            this.rogue1.TabIndex = 7;
            // 
            // priest1
            // 
            this.priest1.BackColor = System.Drawing.Color.Black;
            this.priest1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.priest1.Location = new System.Drawing.Point(0, 0);
            this.priest1.Name = "priest1";
            this.priest1.Size = new System.Drawing.Size(332, 328);
            this.priest1.TabIndex = 6;
            // 
            // hunter1
            // 
            this.hunter1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hunter1.Location = new System.Drawing.Point(0, 0);
            this.hunter1.Name = "hunter1";
            this.hunter1.Size = new System.Drawing.Size(332, 328);
            this.hunter1.TabIndex = 5;
            // 
            // druid1
            // 
            this.druid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.druid1.Location = new System.Drawing.Point(0, 0);
            this.druid1.Name = "druid1";
            this.druid1.Size = new System.Drawing.Size(332, 328);
            this.druid1.TabIndex = 4;
            // 
            // deathKnight1
            // 
            this.deathKnight1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deathKnight1.Location = new System.Drawing.Point(0, 0);
            this.deathKnight1.Name = "deathKnight1";
            this.deathKnight1.Size = new System.Drawing.Size(332, 328);
            this.deathKnight1.TabIndex = 3;
            // 
            // mageSettings1
            // 
            this.mageSettings1.BackColor = System.Drawing.Color.Black;
            this.mageSettings1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mageSettings1.ForeColor = System.Drawing.Color.White;
            this.mageSettings1.Location = new System.Drawing.Point(0, 0);
            this.mageSettings1.Name = "mageSettings1";
            this.mageSettings1.Size = new System.Drawing.Size(332, 328);
            this.mageSettings1.TabIndex = 2;
            // 
            // userconfig_routines1
            // 
            this.userconfig_routines1.BackColor = System.Drawing.Color.Black;
            this.userconfig_routines1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userconfig_routines1.Location = new System.Drawing.Point(0, 0);
            this.userconfig_routines1.Name = "userconfig_routines1";
            this.userconfig_routines1.Size = new System.Drawing.Size(332, 328);
            this.userconfig_routines1.TabIndex = 1;
            this.userconfig_routines1.Load += new System.EventHandler(this.userconfig_routines1_Load);
            // 
            // CombatSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.splitContainer1);
            this.Name = "CombatSettings";
            this.Size = new System.Drawing.Size(504, 328);
            this.Load += new System.EventHandler(this.CombatSettings_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private Userconfig_routines userconfig_routines1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private MageSettings mageSettings1;
        private MxBots.Panels_de_configuration.ClassesCustomSettings.Shaman shaman1;
        private MxBots.Panels_de_configuration.ClassesCustomSettings.Rogue rogue1;
        private MxBots.Panels_de_configuration.ClassesCustomSettings.Priest priest1;
        private MxBots.Panels_de_configuration.ClassesCustomSettings.Hunter hunter1;
        private MxBots.Panels_de_configuration.ClassesCustomSettings.Druid druid1;
        private MxBots.Panels_de_configuration.ClassesCustomSettings.DeathKnight deathKnight1;
        private MxBots.Panels_de_configuration.Paladin paladin1;
        private MxBots.Panels_de_configuration.ClassesCustomSettings.Warrior warrior1;
        private MxBots.Panels_de_configuration.ClassesCustomSettings.Warlock warlock1;
    }
}
