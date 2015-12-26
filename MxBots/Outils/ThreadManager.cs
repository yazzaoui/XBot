using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Magic;


namespace GLib
{
    class ThreadManager : Form
    {
        public static int ProcessId;
        public static void suspendMainThread(int dwProcessId)
        {
            ProcessId = dwProcessId;
            ProcessThread wowMainThread = SThread.GetMainThread(ProcessId);
            IntPtr hThread = SThread.OpenThread(wowMainThread.Id);

            SThread.SuspendThread(hThread);
        }

        public static void resumeMainThread(int dwProcessId)
        {
            ProcessId = dwProcessId;
            ProcessThread wowMainThread = SThread.GetMainThread(ProcessId);
            IntPtr hThread = SThread.OpenThread(wowMainThread.Id);

            SThread.ResumeThread(hThread);
        }

        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0xBEEF)
            {
                Console.WriteLine("0xBEEF message recieved, resuming main thread!");

                ProcessThread wowMainThread = SThread.GetMainThread(ProcessId);
                IntPtr hThread = SThread.OpenThread(wowMainThread.Id);

                SThread.ResumeThread(hThread);
            }
        }

    }
}
