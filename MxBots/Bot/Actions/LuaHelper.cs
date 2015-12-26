using System;
using System.IO;
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

namespace MxBots.Bots.Actions
{
    public class LuaHelper
    {
       private string routine;
       private Classes classe;
       string executableName;
       FileInfo executableFileInfo;
       string executableDirectoryName;
       Bot Curbot;
       public Lua LuaVm;
       public string AttackingTarget { get; set; }
       public string HealingTarget { get; set; }
       public string RezTarget { get; set; }
       public string PreCombat { get; set; }
       public string GotAggro { get; set; }
       public string RetrieveAggro { get; set; }
       public LuaHelper(Classes c,Bot cur)
        {
            executableName = Application.ExecutablePath;
            executableFileInfo = new FileInfo(executableName);
            executableDirectoryName = executableFileInfo.DirectoryName;
            Curbot = cur;
            string dir = executableDirectoryName + "\\Routines\\" + Bot.ClassToString(c) + ".xbot";

            StreamReader sr = new StreamReader(dir);

            routine = sr.ReadToEnd();
            sr.Close();
            AttackingTarget = string.Empty;
            HealingTarget = string.Empty;
            PreCombat = string.Empty;
            GotAggro = string.Empty;
            RetrieveAggro = string.Empty;
            RezTarget = string.Empty;

            getFunctions();
            CreateVm();
           
        }
       private void CreateVm()
       {

           LuaVm = new Lua();
           LuaReg("dance");
           LuaReg("CastSpell");
           LuaReg("IsTargetAlive");
           LuaReg("addConsole");
          
       }
       public void dance()
       {

           Curbot.DoString("DEFAULT_CHAT_FRAME:AddMessage(\"Inner Lua Function Called\")");

       }
       public void addConsole(string str)
       {
           Curbot.SendConsole(str, ConsoleLvl.BotStatut);
       }
       public void CastSpell(string spell)
       {
           Curbot.DoString("CastSpellByName(\"" + spell + "\")");
       }
       public bool IsTargetAlive()
       {
           return Curbot.Objectlist.LocalPlayerTarget.IsAlive;
       }
       private void LuaReg(string reg)
       {
           LuaVm.RegisterFunction(reg, this, this.GetType().GetMethod(reg));
       }
       private void getFunctions()
       {
           int i = 0;
           while (i < routine.Length)
           {
          
                   if (routine[i] == '<' && routine[i + 1] == '<')
                   {
                       string func = "";
                       string inner = "";
                       i = i + 2;

                       while (routine[i] != '>' || routine[i + 1] != '>')
                       {
                           func = func + routine[i];

                           i = i + 1;
                       }
                       i = i + 2;
                       while (routine[i] != '-' || routine[i + 1] != '>')
                       {
                           i++;
                       }
                       i = i + 2;

                       while (routine[i] != '<' || routine[i + 1] != '-')
                       {
                           inner = inner + routine[i];
                           i++;
                       }
                       GotFunc(func, inner);
                   }

                   i++;
                        
           }
       }
       private void GotFunc(string name, string func)
       {
           switch (name)
           {
               case "AttackingTarget":
                   AttackingTarget = func;
                   break;
               case "HealingTarget":
                   HealingTarget = func;
                   break;
               case "RezTarget":
                   RezTarget = func;
                   break;
               case "PreCombat":
                   PreCombat = func;
                   break;
               case "GotAggro":
                   GotAggro = func;
                   break;
               case "RetrieveAggro":
                   RetrieveAggro = func;
                   break;
           }
       }
    }
}
