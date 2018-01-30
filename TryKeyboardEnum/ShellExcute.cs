using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ComRegister
{
	static class ShellExcute
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="target"></param>
		/// <param name="args"></param>
		/// <param name="path"></param>
		public static Process ShellExcuteExe(string target, string args = null, string path = null)
		{
			try
			{
				var p = new ProcessStartInfo(target)
				{
					UseShellExecute = true,
					WorkingDirectory = path,
					WindowStyle = ProcessWindowStyle.Normal,
					Arguments = args
				};
				var process = new Process()
				{
					StartInfo = p,
				};
				process.Start();
				
				return process;
			}
			catch (Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
			}
			return null;

		}
	}
}
