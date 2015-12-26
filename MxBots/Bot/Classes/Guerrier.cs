using System;
using System.Collections.Generic;
using System.Collections;
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
using MxBots.Properties;

namespace MxBots.Bots
{
   public class Guerrier : Bot
    {

       public Guerrier(int i)
           : base(i,Classes.Guerrier)
       {
           Dps = false;
           Tank = true;
           Healer = false;
           
       }

       protected override PrioStruct GetStruct()
       {
           return new SWarEnnemyAlgo();
       }

       protected override void CombatRoutine(GUnit Cible)
       {
           Cible.Target();
           deplacement(Cible);
           CastSpell("Attaque auto");
           int i=0;
           //SendConsole("------", ConsoleLvl.High);
         //  showTargetlist();
           while (Cible.IsAlive && localPlayer.IsAlive)
           {
               if (BreakCurActionB == true)
               {
                   BreakCurActionB = false;
                   break;
               }
               LuaVM.DoString(LuaHelp.AttackingTarget);

               Repositionnement(Cible);

               Thread.Sleep(200);
               if (!Cible.inCombat || !localPlayer.inCombat)
               {
                   Cible.Target();
                   Thread.Sleep(100);
                   CastSpell("Attaque auto");
               }
               i++;
               TargetList.Sort();
           }
           //showTargetlist();
          // SendConsole("Something stopped : " + (TargetList[0].Target == Cible).ToString() + " " + i.ToString(), ConsoleLvl.High);


       }

     
       
    }
}
