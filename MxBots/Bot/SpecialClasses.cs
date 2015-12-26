using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GLib;
using System.Xml.Serialization;
using MxBots.Bots.Actions;

namespace MxBots
{
    public  class PrioStruct : IComparable
    {
        public GUnit Target;

        public PrioStruct()
        {
        }
        public PrioStruct(GUnit tar)
        {
            this.Target = tar;           
           
        }
         int IComparable.CompareTo(object obj)
        {
           return sort(obj);
        }
         public virtual int sort(object obj) { return 0; }


    }

    public class SWarEnnemyAlgo : PrioStruct,IComparable
    {
        public SWarEnnemyAlgo()
            : base()
        {

        }
        public SWarEnnemyAlgo(GUnit tar):base(tar)
        {

        }
       public override int sort(object obj)
        {

            PrioStruct c2 = (PrioStruct)obj;

            if (!this.Target.TargetingSomeTank && c2.Target.TargetingSomeTank)
                return -1;
            if (this.Target.TargetingSomeTank && !c2.Target.TargetingSomeTank)
                return 1;
            if (this.Target.TargetingSomeTank && c2.Target.TargetingSomeTank && this.Target.Health < c2.Target.Health)
                return -1;
            if (this.Target.TargetingSomeTank && c2.Target.TargetingSomeTank && this.Target.Health > c2.Target.Health)
                return -1;
            if (!this.Target.TargetingSomeTank && !c2.Target.TargetingSomeTank && this.Target.Health < c2.Target.Health)
                return -1;
            if (!this.Target.TargetingSomeTank && !c2.Target.TargetingSomeTank && this.Target.Health > c2.Target.Health)
                return  1;
            else
                return 0;

        }


    }

    public class HealthPrioStruct : IComparable
    {
        public GUnit Target;

       

        public HealthPrioStruct()
        {
        }
        public HealthPrioStruct(GUnit tar)
        {
            this.Target = tar;


        }
        int IComparable.CompareTo(object obj)
        {

            HealthPrioStruct c2 = (HealthPrioStruct)obj;
            double percentme = this.Target.Health;
            double percenthe = c2.Target.Health;
            if (percentme < percenthe)
                return -1;
            if (percentme > percenthe)
                return 1;
            else
                return 0;

        }

    }

    public class RezPrioStruct : IComparable
    {
        public GUnit Target;
        public Bot bot;


        public RezPrioStruct()
        {
        }
        public RezPrioStruct(GUnit tar,Bot b)
        {
            this.Target = tar;
            this.bot = b;

        }
        int IComparable.CompareTo(object obj)
        {

            RezPrioStruct face = (RezPrioStruct)obj;

            if (! this.bot.IsBeingRez && face.bot.IsBeingRez )
                return -1;
            else if (this.bot.IsBeingRez && !face.bot.IsBeingRez)
                return 1;
            else if (this.bot.Healer && !this.bot.Healer)
                return -1;
            else if (!this.bot.Healer && this.bot.Healer)
                return 1;
            else
                return 0;

        }

    }

    [Serializable]
    public class NavMesh
    {
        public List<Node> Nodes { get; set; }
        public int count { get; set; }
        public NavMesh()
        {
            count = 0;
            Nodes = new List<Node>();
        }
        public void addNode(GLocation pos)
        {
            Node n = new Node(count, pos);
            n.NodeProches = new List<Nodacoter>();
            Nodes.Add(n);

            count++;
        }
        public void addNode(GLocation pos,NavMesh w)
        {
            Node n = new Node(count, pos);
            n.NodeProches = new List<Nodacoter>();
            Nodes.Add(n);
            count++;
        }
        public void AddWay(int a, int b)
        {
            double dist = distance(Nodes[a],Nodes[b]);
            Nodacoter ab = new Nodacoter();
            ab.id=b;
            ab.distance = dist;

            Nodes[a].NodeProches.Add(ab);
            Nodacoter ba = new Nodacoter();
            ba.id=a;
            ba.distance = dist;
            Nodes[b].NodeProches.Add(ba);
        }
        public NavMesh CheckProche(float distancea,float distanceb,GLocation loc)
        {
            double disteucl;
            NavMesh list = new NavMesh();
            foreach (Node n in Nodes)
            {
                disteucl = (loc.X-n.Loc.X)*(loc.X-n.Loc.X) + (loc.Y-n.Loc.Y)*(loc.Y-n.Loc.Y) + (loc.Z-n.Loc.Z)*(loc.Z-n.Loc.Z);
                if (Math.Sqrt(disteucl) < distanceb && Math.Sqrt(disteucl) >= distancea)
                {
                    list.addNode(n);
                }
            }
            return list;
         }
        public static double distance(Node a, Node b)
        {
            return Math.Sqrt((a.Loc.X - b.Loc.X) * (a.Loc.X - b.Loc.X) + (a.Loc.Y - b.Loc.Y) * (a.Loc.Y - b.Loc.Y) + (a.Loc.Z - b.Loc.Z) * (a.Loc.Z - b.Loc.Z));
        }
        public static double distance(Node a, GLocation b)
        {
            return Math.Sqrt((a.Loc.X - b.X) * (a.Loc.X - b.X) + (a.Loc.Y - b.Y) * (a.Loc.Y - b.Y) + (a.Loc.Z - b.Z) * (a.Loc.Z - b.Z));
        }
        public void addNode(Node n)
        {
            Nodes.Add(n);
            count++;
            
        }
    }

    [Serializable]
    public class Node
    {
        public int id { get; set; }
        public List<Nodacoter> NodeProches { get; set; }
        public GLocation Loc { get; set; }
        public int DejaPasser;
        public Node()
        {
        }
        public Node(int id,GLocation loc)
        {
            this.id = id;
            Loc = loc;
            NodeProches = new List<Nodacoter>();
            DejaPasser = 0;
        }
        public void SortNodesProches()
        {
            NodeProches.Sort();
        }

    }
    [Serializable]
    public struct Nodacoter : IComparable
    {
        public int id ;
        public double distance;

        int IComparable.CompareTo(object obj)
        {
            List<Node> Nodes = SomeSettings.NavMesh.Nodes;
            Nodacoter face = (Nodacoter)obj;

            if (Nodes[id].DejaPasser < Nodes[face.id].DejaPasser)
                return -1;
            else if (Nodes[id].DejaPasser > Nodes[face.id].DejaPasser)
                return 1;
            else
                return 0;

        }

    }
}
