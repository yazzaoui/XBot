using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace GLib
{
	public static class Post
	{
		private const int WM_KEYDOWN = 0x100;
		private const int WM_KEYUP = 0x101;

		[DllImport("user32.dll", EntryPoint = "PostMessage")]
		private static extern bool _PostMessage(IntPtr hWnd, uint msg, uint wParam, uint lParam);
		public static bool PostMessage(IntPtr hWnd, uint msg, uint wParam, uint lParam)
		{ return _PostMessage(hWnd, msg, wParam, lParam); }

		public enum ArrowKeys
		{
			Up,
			Left,
			Right,
			Down
		}

		public static bool ArrowKeyDown(IntPtr hWnd, ArrowKeys key)
		{
			uint wParam, lParam;

			switch (key)
			{
				case ArrowKeys.Left:
					wParam = 0x25;
					lParam = 0x14B0001;
					break;

				case ArrowKeys.Up:
					wParam = 0x26;
					lParam = 0x1480001;
					break;

				case ArrowKeys.Right:
					wParam = 0x27;
					lParam = 0x14D0001;
					break;

				case ArrowKeys.Down:
					wParam = 0x28;
					lParam = 0x1500001;
					break;

				default:
					return false;
			}

			return PostMessage(hWnd, WM_KEYDOWN, wParam, lParam);
		}

		public static bool ArrowKeyUp(IntPtr hWnd, ArrowKeys key)
		{
			uint wParam, lParam;

			switch (key)
			{
				case ArrowKeys.Left:
					wParam = 0x25;
					lParam = 0x14B0001;
					break;

				case ArrowKeys.Up:
					wParam = 0x26;
					lParam = 0x1480001;
					break;

				case ArrowKeys.Right:
					wParam = 0x27;
					lParam = 0x14D0001;
					break;

				case ArrowKeys.Down:
					wParam = 0x28;
					lParam = 0x1500001;
					break;

				default:
					return false;
			}

			return PostMessage(hWnd, WM_KEYUP, wParam, (lParam + 0xC0000000));
		}

		public static bool ArrowKey(IntPtr hWnd, ArrowKeys key)
		{
			uint wParam, lParam;

			switch (key)
			{
				case ArrowKeys.Left:
					wParam = 0x25;
					lParam = 0x14B0001;
					break;

				case ArrowKeys.Up:
					wParam = 0x26;
					lParam = 0x1480001;
					break;

				case ArrowKeys.Right:
					wParam = 0x27;
					lParam = 0x14D0001;
					break;

				case ArrowKeys.Down:
					wParam = 0x28;
					lParam = 0x1500001;
					break;

				default:
					return false;
			}

			if (!PostMessage(hWnd, WM_KEYDOWN, wParam, lParam)) return false;
			if (!PostMessage(hWnd, WM_KEYUP, wParam, (lParam + 0xC0000000))) return false;

			return true;
		}

		public static bool ArrowKey(IntPtr hWnd, ArrowKeys key, int hold)
		{
			uint wParam, lParam;

			switch (key)
			{
				case ArrowKeys.Left:
					wParam = 0x25;
					lParam = 0x14B0001;
					break;

				case ArrowKeys.Up:
					wParam = 0x26;
					lParam = 0x1480001;
					break;

				case ArrowKeys.Right:
					wParam = 0x27;
					lParam = 0x14D0001;
					break;

				case ArrowKeys.Down:
					wParam = 0x28;
					lParam = 0x1500001;
					break;

				default:
					return false;
			}

			for (int i = 0; i < hold; i += 50)
			{
				if (!PostMessage(hWnd, WM_KEYDOWN, wParam, lParam)) return false;
				Thread.Sleep(50);
			}

			if (!PostMessage(hWnd, WM_KEYUP, wParam, (lParam + 0xC0000000))) return false;

			return true;
		}
	}
}
