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
    public static class Hook
    {
        private const int WM_LBUTTONDOWN = 0x201;
        private const int WH_MOUSE_LL = 14;
        private const int WM_RBUTTONDOWN = 0x204;
        private static IntPtr hookz= IntPtr.Zero;
        private static MouseLLProc _proc = MouseHookProc;
         public delegate void MouseTransfertEvent(MouseEventArgs e);
         public static event MouseTransfertEvent cliked;
        [StructLayout(LayoutKind.Sequential)]
        private class POINT
        {
            /// <summary>
            /// Coordonnée X. 
            /// </summary>
            public int x;
            /// <summary>
            /// Coordonnée Y.
            /// </summary>
            public int y;
        }


        [StructLayout(LayoutKind.Sequential)]
        private class MouseHookStruct
        {
            /// <summary>
            /// Structure POINT pour les coordonnée 
            /// </summary>
            public POINT pt;
            /// <summary>
            /// Handle de la window
            /// </summary>
            public int hwnd;
            /// <summary>
            /// Specifies the hit-test value. For a list of hit-test values, see the description of the WM_NCHITTEST message. 
            /// </summary>
            public int wHitTestCode;
            /// <summary>
            /// Specifies extra information associated with the message. 
            /// </summary>
            public int dwExtraInfo;
        }




        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int SetWindowsHookEx(int idHook, MouseLLProc lpfn, IntPtr hMod, int dwThreadId);


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        public static void StartMouseHook()
        {
            // Faire une instance de HookProc.
            
            //installer le hook
             hookz = (IntPtr)SetWindowsHookEx(
                   WH_MOUSE_LL,
                    _proc,
                    GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName),
                    0);

        }
        public static void StopMouseHook()
        {
            // Faire une instance de HookProc.

            //installer le hook
           UnhookWindowsHookEx(hookz);

        }

        private delegate int MouseLLProc(int nCode, int wParam, IntPtr lParam);
        private static int MouseHookProc(int nCode, int wParam, IntPtr lParam)
        {
            bool processNextHook = true;

            // Verifions si nCode est different de 0 et que nos evenements sont bien attachés
            if ((nCode >= 0) && (cliked != null))
            {
                //Remplissage de la structure MouseLLHookStruct a partir d'un pointeur
                MouseHookStruct mouseHookStruct = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));

                //Detection du bouton clicker
                MouseButtons button = MouseButtons.None;
                switch (wParam)
                {
                    case WM_LBUTTONDOWN:
                        button = MouseButtons.Left;
                        break;
                    case WM_RBUTTONDOWN:
                        button = MouseButtons.Right;
                        break;
                }

                //parametre de notre event 
                MouseEventArgs e = new MouseEventArgs(
                                                   button,
                                                   1,
                                                   mouseHookStruct.pt.x,
                                                   mouseHookStruct.pt.y,
                                                   0);
                //On appelle notre event
                cliked(e);
            }
            //Si processNextHook == true alors on transmet le click au destinataire, sinon, on le garde pour nous (
            if (processNextHook == true)
            {
               

                return CallNextHookEx(hookz, nCode, (IntPtr)wParam, lParam).ToInt32();
            }
            else
                return 1;
        }



    }

  
}
