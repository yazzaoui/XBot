using System;using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Magic;
using System.Threading;

namespace GLib
{
   public  class Keys
    {
        BlackMagic Memory;
        private bool shift = false;
        private eVirtualKeys wParam;
        private eVirtualKeys wParam2;
        private eVirtualKeys bar;
        private string barstring;
        private string keyName;
        [DllImport("user32.dll", EntryPoint = "PostMessage")]
        private static extern bool _PostMessage(IntPtr hWnd, uint msg, uint wParam, uint lParam);

        //KeyName, ShiftState, BarState, Char
        public Keys(string keyName, string shiftState, string BarState, string Char, BlackMagic mem)
        {
            this.Memory = mem;
            this.keyName = keyName;
            barstring = BarState;
            //Setup shift
            if(shiftState.Equals("Ctrl"))
            {
                shift = true;
                wParam = eVirtualKeys.VK_LCONTROL;
            }
            else if (shiftState.Equals("Shift"))
            {
                shift = true;
                wParam = eVirtualKeys.VK_SHIFT;
            }
            else if (shiftState.Equals("Alt"))
            {
                shift = true;
                wParam = eVirtualKeys.Alt;
            }
            else
            {
                shift = false;
            }
            //Setup barstate
            if (BarState.Equals("Bar1"))
            {
                bar = eVirtualKeys.key1;
            } 
            else if(BarState.Equals("Bar2")) 
            {
                bar = eVirtualKeys.key2;
            }
            else if (BarState.Equals("Bar3"))
            {
                bar = eVirtualKeys.key3;
            }
            else if (BarState.Equals("Bar4"))
            {
                bar = eVirtualKeys.key4;
            }
            else if (BarState.Equals("Bar5"))
            {
                bar = eVirtualKeys.key5;
            }
            else if (BarState.Equals("Bar6"))
            {
                bar = eVirtualKeys.key6;
            }
            else 
            {
                bar = eVirtualKeys.Indifferent;
            }
            if (Char.Equals("0"))
                wParam2 = eVirtualKeys.key0;
            else if (Char.Equals("1"))
                wParam2 = eVirtualKeys.key1;
            else if (Char.Equals("2"))
                wParam2 = eVirtualKeys.key2;
            else if (Char.Equals("3"))
                wParam2 = eVirtualKeys.key3;
            else if (Char.Equals("4"))
                wParam2 = eVirtualKeys.key4;
            else if (Char.Equals("5"))
                wParam2 = eVirtualKeys.key5;
            else if (Char.Equals("6"))
                wParam2 = eVirtualKeys.key6;
            else if (Char.Equals("7"))
                wParam2 = eVirtualKeys.key7;
            else if (Char.Equals("8"))
                wParam2 = eVirtualKeys.key8;
            else if (Char.Equals("9"))
                wParam2 = eVirtualKeys.key9;
            else
            {
                wParam2 = (eVirtualKeys)Enum.Parse(typeof(eVirtualKeys), Char, true);
            }
            if (!Enum.IsDefined(typeof(eVirtualKeys), wParam2))
            {
                throw new Exception("The key send is not supported");
            }
        }

        public string name
        {
            get
            {
                return keyName;
            }
        }

        public string getbar
        {
            get
            {
                return barstring;
            }
        }

        public void changeBar()
        {
            if (bar != eVirtualKeys.Indifferent)
            {
                _PostMessage(Memory.WindowHandle, 0x100, (uint)eVirtualKeys.VK_SHIFT, 0);
                _PostMessage(Memory.WindowHandle, 0x100, (uint)bar, 0);
                _PostMessage(Memory.WindowHandle, 0x101, (uint)eVirtualKeys.VK_SHIFT, 0);
                _PostMessage(Memory.WindowHandle, 0x101, (uint)bar, 0);
                Thread.Sleep(200);
            }
        }

        public void SendKey()
        {
            if (!shift)
            {
                _PostMessage(Memory.WindowHandle, 0x100, (uint)wParam2, 0);
                _PostMessage(Memory.WindowHandle, 0x101, (uint)wParam2, 0);
            }
            if (shift)
            {
                _PostMessage(Memory.WindowHandle, 0x100, (uint)wParam, 0);
                _PostMessage(Memory.WindowHandle, 0x100, (uint)wParam2, 0);
                _PostMessage(Memory.WindowHandle, 0x101, (uint)wParam, 0);
                _PostMessage(Memory.WindowHandle, 0x101, (uint)wParam2, 0);
            }
        }

        public void PressKey()
        {
            if (!shift)
            {
                _PostMessage(Memory.WindowHandle, 0x100, (uint)wParam2, 0);
            }
            if (shift)
            {
                _PostMessage(Memory.WindowHandle, 0x100, (uint)wParam, 0);
                _PostMessage(Memory.WindowHandle, 0x100, (uint)wParam2, 0);
            }
        }

        public void ReleaseKey()
        {
            if (!shift)
            {
                _PostMessage(Memory.WindowHandle, 0x101, (uint)wParam2, 0);
            }
            if (shift)
            {
                _PostMessage(Memory.WindowHandle, 0x101, (uint)wParam, 0);
                _PostMessage(Memory.WindowHandle, 0x101, (uint)wParam2, 0);
            }
        }
    }
}
