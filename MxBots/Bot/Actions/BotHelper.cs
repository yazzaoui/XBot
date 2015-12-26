using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using System.Runtime.InteropServices;
using MxBots.Bots.Actions;
using GLib;
using Magic;
using LuaInterface;
using System.Reflection;
using System.Windows.Forms;

namespace MxBots.Bots.Actions
{
    public class BotHelper
    {
        Bot[] Bots;
        public int Main{get;set;}
        public event StringStatutTransfertEventHandler sendtext;
        public event AggroTransfertEventHandler GotAggro;
        public int maxCombatTimeTick{get;set;}
        public BotHelper(Bot[] Bots)
        {
            this.Bots = Bots;
            this.Main = 0;
            maxCombatTimeTick = 0;
            foreach (Bot b in Bots)
            {
                if (b.PreCombatTick > maxCombatTimeTick)
                {
                    maxCombatTimeTick = b.PreCombatTick;
                }
            }
        }
        public bool isUnitTargetingSomeTank(GUnit Cible)
        {
            foreach (Bot b in Bots)
            {
                if (Cible.TargetGUID == b.GUID)
                {
                    if (b.Tank == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
       
        public TimeSpan CombatTime(GUnit Cible)
        {
            if (Cible.StartedCombat == false)
            {
                return TimeSpan.Zero;
            }
            if (Cible.StartCombat != null)
            {
                DateTime startTime = Cible.StartCombat;

                DateTime stopTime = DateTime.Now;
                TimeSpan duration = stopTime - startTime;
                return duration;
            }
            else
            {
                new System.ArgumentException("Ben On est pas dans la merde :(", "Cible.startTime");
                return TimeSpan.Zero;
            }

        }
        public void StartAttack(GUnit Cible)
        {
            

        }
        public void WatchHealth()
        {
            while (true)
            {

                foreach (Bot b in Bots)
                {
                    if (b.Objectlist.LocalPlayer.IsDead)
                    {
                        addRez(b.Objectlist.LocalPlayer,b);
                    }

                    else if ((b.Objectlist.LocalPlayer.Health) < 0.9)
                    {
                       
                        addHeal(b.Objectlist.LocalPlayer);
                    }
                    Thread.Sleep(100);
                }

                

            }
        }
        private void addHeal(GPlayerSelf cible)
        {
            foreach (Bot b in Bots)
            {
                if (b.Healer)
                {
                    b.addHealTarget(cible.GUID);
                }
               
            }
        }
        private void addRez(GPlayerSelf cible,Bot b2)
        {
            foreach (Bot b in Bots)
            {
                if (b.Healer)
                {
                    b.addRezTarget(cible.GUID,b2);
                }

            }
        }
        public void WatcherOoO()
        {
            while(true)
            {

                    int counter = 0;
                    List<ulong> GuidList = Bots[Main].Objectlist.GuidList;
                   
                    IDictionary<ulong,GObject> ObjectList = Bots[Main].Objectlist.ObjectList ;
                    IDictionary<ulong,GUnit> GUnitList = Bots[Main].Objectlist.GUnitList ;
                    IDictionary<ulong, GPlayer> GPlayerList = Bots[Main].Objectlist.GPlayerList;
                    int main = Main;
                    while (Bots[main].Objectlist.GuidList.Count > counter)
                    {
                        
                            if (ObjectList[GuidList[counter]].Type.Equals((int)eObjectType.G_UNIT))
                            {
                                if (GUnitList.ContainsKey(GuidList[counter]))
                                {
                                    WatchTarget(GUnitList[GuidList[counter]]);
                                }
                                   
                            }
                            else if (ObjectList[GuidList[counter]].Type.Equals((int)eObjectType.G_PLAYER))
                            {
                                if (GPlayerList.ContainsKey(GuidList[counter]))
                                {
                                    WatchTarget(GPlayerList[GuidList[counter]]);
                                }
                                   
                            }
                            if (Main != main)
                            {
                                break;
                            }
                            counter++;
                            Thread.Sleep(10);
                        }
                        
                    
                    
                
            }
        }
        private void WatchTarget(GUnit unit)
        {
            
            if (unit.inCombat)
            {
                if (unit.StartedCombat == false)
                {
                    SendConsole("Unit Agressive: " + unit.Name, ConsoleLvl.High);
                    unit.StartCombat = DateTime.Now;
                    unit.StartedCombat = true;
                    foreach (Bot b in Bots)
                    {
                        if (unit.TargetGUID == b.GUID)
                        {
                            GetAggro(unit.GUID,b.ID);
                        }
                    }
                }
                unit.TargetingSomeTank = isUnitTargetingSomeTank(unit);
              
            }
            else if (unit.StartedCombat == true)
            {

                    SendConsole("Unit Stopped Combat: " + unit.Name, ConsoleLvl.High);
                    unit.StartedCombat = false;
                

            }
 
        }
        public void CentralizeOoO()
        {
            while (true)
            {
                foreach (Bot b in Bots)
                {
                    if (b.ActionList.Count != 0)
                    {
                        SAction CurAction = b.ActionList[0];
                        
                        b.ActionList.Sort();
                        b.TargetList.Sort();
                        b.HealList.Sort();
                        if (b.ActionList[0] != CurAction || (b.TargetList.Count != 0 && b.ActionList[0].Priority < BotHub.AgroPriority) || (b.HealList.Count != 0 && b.ActionList[0].Priority < BotHub.HealPriority))
                        {
                            SendConsole("Changing current action", ConsoleLvl.BotHub,b);
                            b.breakCurAction();

                        }
                        else if (CurAction.Action == BotAction.Attack)
                        {

                            if (b.TargetList[0].Target != b.Cible)
                            {
                                SendConsole("Changing current target", ConsoleLvl.BotHub,b);
                                b.breakCurAction();
                            }
                        }
                        else if (CurAction.Action == BotAction.Heal)
                        {

                            if (b.HealList[0].Target != b.Cible)
                            {
                                SendConsole("Changing current heal target", ConsoleLvl.BotHub,b);
                                b.breakCurAction();
                            }
                        }
                        else if (CurAction.Action == BotAction.AttackMain)
                        {

                            if (b.MainTargetList.Peek().Target != b.Cible)
                            {
                                SendConsole("Changing current target", ConsoleLvl.BotHub,b);
                                b.breakCurAction();
                            }
                        }
                        else if (CurAction.Action == BotAction.GoToLocation)
                        {

                            if (!CurAction.Location.Equals(b.ActionList[0].Location))
                            {
                                SendConsole("Changing current location", ConsoleLvl.BotHub, b);
                                b.breakCurAction();
                            }
                        }
                        Thread.Sleep(10);
                    }
                }
                Thread.Sleep(150);
            }
        }
        private void GetAggro(ulong guid,int id)
        {
            if (GotAggro != null)
            {
                GotAggro(new AggroTransfertEventArg(guid,id));
            }
        }
        protected void SendConsole(string what, ConsoleLvl cl, Bot b)
        {
            if (sendtext != null)
            {
                sendtext(new StringStatutTransfertEventArg(b.Nom + " ("+ b.ID +"):" + what, cl));
            }
        }


        protected void SendConsole(string what, ConsoleLvl cl)
        {
            if (sendtext != null)
            {
                sendtext(new StringStatutTransfertEventArg(what, cl));
            }
        }
    }
}
