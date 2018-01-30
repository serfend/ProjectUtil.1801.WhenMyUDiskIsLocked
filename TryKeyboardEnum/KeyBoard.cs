using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace TryKeyboardEnum
{
	static class KeyBoard
	{
		[DllImport("user32.dll", EntryPoint = "keybd_event", SetLastError = true)]
		private static extern void Keybd_event(Keys bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
		public const int KEYEVENTF_KEYUP = 0x02;
		public const int KEYEVENTF_KEYDOWN = 0x00;

		//相当于按下ctrl+v，然后回车
		public static void KeyBordPaste()
		{
			Keybd_event(Keys.ControlKey, 0, KEYEVENTF_KEYDOWN, 0);
			Keybd_event(Keys.V, 0, 0, 0);
			Keybd_event(Keys.ControlKey, 0, KEYEVENTF_KEYUP, 0);
			Keybd_event(Keys.Enter, 0, 0, 0);
		}
		public static void KeyDown(Keys key)
		{
			Keybd_event(key, 0, KEYEVENTF_KEYDOWN, 0);
		}
		public static void KeyUp(Keys key)
		{
			Keybd_event(key, 0, KEYEVENTF_KEYUP, 0);
		}
		public static void KeyPress(Keys key)
		{
			Keybd_event(key, 0, 0, 0);
		}
	}
}
