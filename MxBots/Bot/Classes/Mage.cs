using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GLib;
namespace MxBots.Bots
{
   public class Mage : Bot
    {
       public bool waitTankPull { get; set; }
       private int timeToWaitPull;
       private int precombattick;
       public Mage(int i)
          : base(i,Classes.Mage)
       {

           waitTankPull = ClassSettings.Default.MageWaitTankPull;
           timeToWaitPull = ClassSettings.Default.MageTimeToWaitTankAgro;
           
           Dps = true;
           Tank = false;
           Healer = false;
           
       }




       protected override void CombatRoutine(GUnit Cible)
       {

           if (waitTankPull)
           {
               if (BotHelper.CombatTime(Cible) < TimeSpan.FromMilliseconds(1000))
               {
                   // combat vien juste de commencer
                   Thread.Sleep(timeToWaitPull);
               }
           }
            
            while (Cible.IsAlive && localPlayer.IsAlive)
            {
                if (BreakCurActionB == true)
                {
                    BreakCurActionB = false;
                    break;
                }
                LuaVM.DoString(LuaHelp.AttackingTarget);

                Repositionnement(Cible);
                if (!Cible.inCombat || !localPlayer.inCombat)
                {
                    Cible.Target();
                    Thread.Sleep(100);
                    
                }
                Thread.Sleep(200);
               
                TargetList.Sort();
            }
       }



    }
}
