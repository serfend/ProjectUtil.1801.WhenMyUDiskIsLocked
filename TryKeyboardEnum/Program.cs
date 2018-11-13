using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace TryKeyboardEnum
{
	class Program
	{
		private static int LengthBegin = 3, LengthEnd = 4;
		private static string ConfigBEGIN = "#Begin#";
		private static string ConfigEND = "#End#";
		private static int MAXThread = 8;
		static void Main(string[] args)
		{
			workers = new List<BackgroundWorker>(MAXThread);
			for(int i = 0; i < MAXThread; i++)
			{
				workers.Add(new BackgroundWorker());
			}
			foreach (var worker in workers)
			{
				worker.WorkerReportsProgress = true;
				worker.DoWork += Worker_DoWork;
				worker.ProgressChanged += Worker_ProgressChanged;
				
			}
			foreach (var file in args)
			{
				if (file.IndexOf(ConfigBEGIN) > 0)
				{
					LengthBegin = Convert.ToInt32(file.Replace(ConfigBEGIN, string.Empty));
				}
				else if (file.IndexOf(ConfigEND) > 0)
				{
					LengthEnd = Convert.ToInt32(file.Replace(ConfigEND, string.Empty));
				}
				else
				{
					HandleFile(file);
				}				
			}
			Console.ReadLine();
		}
		private static void HandleFile(string target)
		{
			Console.WriteLine("处理文件" + target);
			var lines = System.IO.File.ReadLines(target);
			foreach(var line in lines)
			{
				TryWord(line.Split('$'));
			}
			foreach(var worker in workers)
			{
				worker.RunWorkerAsync();
			}
			
		}
		private static void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			if(sender is BackgroundWorker worker)
			{
				Console.WriteLine("剩余:" + task.Count + "完成:" +e.UserState);
			}
			
		}

		private static void Worker_DoWork(object sender, DoWorkEventArgs e)
		{
			if(sender is BackgroundWorker worker)
			{
				while (TryOnce(out string info))
				{
					worker.ReportProgress(0,info);
				}
			}
		}

		private static List< BackgroundWorker> workers;
		static void TryWord(string[] list)
		{

			var wordList = new WordEnum()
			{
				AllowSame = false,
				Keyword = new List<string>(list),
				MaxLength = -1,
			};
			for (int i = 3; i <= 4; i++)
			{
				wordList.NowLength = i;
				bool haveNext;
				OutPut(wordList.Now());
				do
				{
					OutPut(wordList.Next(out haveNext));
				} while (haveNext);
				Console.WriteLine("完成" + i);
			}
		}
		static void OutPut(List<string> target)
		{
			OutPut(target, 0,"");


		}
		private static Queue<string> task=new Queue<string>();
		static void OutPut(List<string> target,int nowIndex,string tmp)
		{
			if (nowIndex == target.Count )
			{
				task.Enqueue(tmp);
				return;
			}
			string s = target[nowIndex];
				if (s.IndexOf("/") > 0)
				{
					string[] tmps = s.Split('/');
					foreach(var s2 in tmps)
					{
						OutPut(target, nowIndex + 1, tmp + s2);
					}
				}
				else
				{
					OutPut(target, nowIndex + 1, tmp + s);
				}
			
		}
		static bool TryOnce(out string info)
		{
			info = null;
			if (task.Count == 0) return false;
			string tmp = task.Dequeue();
			TryOnce(tmp);
			info = tmp;
			return true;
		}
		static void TryOnce(string tmp)
		{
			//KeyBoard.KeyPress(Keys.Enter);//打开
			var process = ComRegister.ShellExcute.ShellExcuteExe("U盘、移动硬盘加密工具.exe", null, @"F:\Main");
			do
			{
				Thread.Sleep(10);
			} while (process.MainWindowHandle == IntPtr.Zero);
			
			var keyHdl= new WinMessager(process.MainWindowHandle);
			//Console.WriteLine(string.Format("主窗体Handle={0}", keyHdl.Target.ToString("X")));
			Thread.Sleep(100);
			
			keyHdl.PreviousWindow();
			//Console.WriteLine(string.Format("目标窗体Handle={0}", keyHdl.Target.ToString("X")));
			var inputBox = keyHdl.GetTarget(IntPtr.Zero, "TEdit", "");//找到输入框
			inputBox = keyHdl.GetTarget(inputBox.Target, "TEdit", "");//找到输入框
			
			//Console.WriteLine(string.Format("输入框Handle={0}", inputBox.Target.ToString("X")));
			//Console.WriteLine("尝试密码:" + tmp);
			inputBox.SetText(tmp);//设置密码

			var SubmitBox = keyHdl.GetTarget(IntPtr.Zero, "TButton", "解密");
			SubmitBox.SndMsg(WinMessager.WM_LBUTTONDOWN, IntPtr.Zero,null);
			SubmitBox.SndMsg(WinMessager.WM_LBUTTONUP, IntPtr.Zero, null);//提交
			Thread.Sleep(500);
			process.Kill();
			//KeyBoard.KeyPress(Keys.Enter);
		}
	}
}
