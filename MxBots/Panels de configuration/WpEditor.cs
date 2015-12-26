using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Linq;
using System.Collections;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Specialized;
using System.Drawing.Drawing2D;
using MxBots.Properties;
using GLib;
using System.Xml.Serialization;
using System.IO;

using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;


namespace MxBots.Panels_de_configuration
{
    public partial class WpEditor : Form
    {
        int Pid;
        GObjectList Player;
        NavMesh NavWesh;
        public WpEditor()
        {
            InitializeComponent();
            label1.Text = "";
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            th = new Thread(new ThreadStart(NodeThread));

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Are you sure ?", "reset",MessageBoxButtons.OKCancel);
            if(dr == DialogResult.OK)
            {
                NavWesh = new NavMesh();
            }
        }

        private void WpEditor_Load(object sender, EventArgs e)
        {

        }

        private void WpEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
           
        }
        Thread th;
        private void button3_Click(object sender, EventArgs e)
        {
            
           Pid= Magic.SProcess.GetProcessFromProcessName("WoW");
           Player = new GObjectList(Pid);
           label1.Text = "Zone ID " + Player.LocalPlayer.ZoneID.ToString();
           NavWesh = new NavMesh();
           if (!th.IsAlive)
           {
               th.Start();
           }
        }
        private void addC(Image m)
        {
            pictureBox1.Image = m;
        }
        private delegate void MaMethodeCallBack(string text);
        private delegate void Calltwo(Image c);
        bool pause = false;
        private void NodeThread()
        {
            Bitmap dota = Resources.dot1;
            Bitmap dotb = Resources.dot2;
            GLocation CurPos;
            NavWesh.addNode(Player.LocalPlayer.Location);

            
            while (true )
            {
                if (this.Visible == false || this.Enabled == false)
                {
                    Thread.CurrentThread.Abort();
                }
                while (pause)
                {
                    Thread.Sleep(100);
                }
                try
                {
                    CurPos = Player.LocalPlayer.Location;

                    this.Invoke(new MaMethodeCallBack(changetext), NavWesh.count.ToString());

                    Bitmap bm = new Bitmap(pictureBox1.Width, pictureBox1.Height);




                    Graphics g = Graphics.FromImage(bm);

                    g.FillRectangle(Brushes.White, this.ClientRectangle);

                    Pen p = new Pen(Color.Black);


                    g.TranslateTransform(bm.Width / 2, bm.Height / 2);


                    foreach (Node n in NavWesh.Nodes)
                    {

                        //g.DrawImageUnscaled(dotb, (int)Math.Truncate((n.Loc.X - CurPos.X) * 10) - 7, (int)Math.Truncate((n.Loc.Y - CurPos.Y) * 10) - 7);
                        g.DrawImage(dotb, (int)Math.Truncate((n.Loc.X - CurPos.X) ) , (int)Math.Truncate((n.Loc.Y - CurPos.Y) ), 5, 5);
                        // g.DrawEllipse(p, (int)Math.Truncate((n.Loc.X - CurPos.X) * 10), (int)Math.Truncate((n.Loc.Y - CurPos.Y) * 10), 5, 5);
                    //foreach (Nodacoter c in n.NodeProches)
                    //{
                    //   // g.DrawLine(p, getPoint(n, CurPos), getPoint(NavWesh.Nodes[c.id], CurPos));
                    //}
                    }
                    g.DrawImage(dota, 0, 0, 10, 10);

                    bm.RotateFlip(RotateFlipType.Rotate90FlipY);

                    this.Invoke(new Calltwo(addC), bm);

                    RefreshNode();
                    Thread.Sleep(35);
                }
                catch
                {
                }
            }
        }
        private Point getPoint(Node n,GLocation CurPos)
        {
            Point p = new Point();
            p.X = (int)Math.Truncate((n.Loc.X - CurPos.X) * 10);
            p.Y = (int)Math.Truncate((n.Loc.Y - CurPos.Y) * 10);
            return p;
        }
        int LastMesh=0;
        private void RefreshNode()
        {
            NavMesh procheNod = NavWesh.CheckProche(0.0f, 1.0f, Player.LocalPlayer.Location);
            foreach (Node n in procheNod.Nodes)
            {
                if (LastMesh != n.id)
                {
                    NavWesh.AddWay(LastMesh, n.id);
                    LastMesh = n.id;
                }
            }
            NavMesh liste = NavWesh.CheckProche(0.0f,5.0f, Player.LocalPlayer.Location);
           
            if (liste.count == 0 )
            {
                NavWesh.addNode(Player.LocalPlayer.Location,liste);
                NavWesh.AddWay(LastMesh, NavWesh.count - 1);
                LastMesh = NavWesh.count - 1;
            }

        }
        private void changetext(string text)
        {
            label1.Text = text;
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void WpEditor_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                pause = true;
            }
            else
            {
                pause = false;
            }
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
            WpEditorSaving WpSavin = new WpEditorSaving();
            WpSavin.ShowDialog();
            WpSavin.TopMost = true;
            string save = WpSavin.Reponse;
            string FILE = "Waypoints\\"+save+".xnav"; 
            
            IFormatter formatter = new BinaryFormatter();

            using (FileStream strm = new FileStream(FILE, FileMode.Create, FileAccess.Write))
            {
                
                formatter.Serialize(strm, NavWesh);
            }
            this.TopMost = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Pid = Magic.SProcess.GetProcessFromProcessName("WoW");
            Player = new GObjectList(Pid);
            Stream myStream;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory()+"\\Waypoints";
            openFileDialog1.Filter = "nav files (*.xnav)|*.xnav";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = false;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = openFileDialog1.OpenFile()) != null)
                {
                    // Insert code to read the stream here.
                    IFormatter formatter = new BinaryFormatter();
                   
                    NavWesh =  formatter.Deserialize(myStream) as NavMesh;
                   
                    myStream.Close();
                }
                if (!th.IsAlive)
                {
                    th.Start();
                }
            }


        }
    }
}
