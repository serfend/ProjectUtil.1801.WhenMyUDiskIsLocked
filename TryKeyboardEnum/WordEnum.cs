using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TryKeyboardEnum
{
	class WordEnum
	{
		private List<string> keyword;
		private List<string> nowKeyWord;
		private List<string> nowNotSelectWord;
		private int maxLength;
		private int nowLength;
		private bool allowSame;
		private int nowIndex;
		public WordEnum()
		{
			NowIndex = 0;
			nowKeyWord = new List<string>();
			nowNotSelectWord = new List<string>();
		}
		/// <summary>
		/// 记录所有候选关键词，并升序排序
		/// </summary>
		public List<string> Keyword { get => keyword;set
			{
				keyword = value.Distinct().ToList();
				keyword.Sort((x, y) => x.CompareTo(y));
				NowLength = keyword.Count;
			}
		}

		/// <summary>
		/// 生成的最大长度
		/// </summary>
		public int MaxLength { get => maxLength; set => maxLength = value; }
		/// <summary>
		/// 当前长度变化时重置nowKeyWord
		/// </summary>
		public int NowLength { get => nowLength; set {
				if (value == nowLength) return;
				nowKeyWord.Clear();
				nowLength = value;
				int keyWordBegin = keyword.Count- nowLength;
				for(int i = 0; i < nowLength; i++)
				{
					nowKeyWord.Add(keyword[i]);
				}
				for(int i = nowLength; i < keyword.Count; i++)
				{
					nowNotSelectWord.Add(keyword[i]);
				}
			} }
		/// <summary>
		/// 是否允许关键词有重复
		/// </summary>
		public bool AllowSame { get => allowSame; set => allowSame = value; }
		/// <summary>
		/// 当前生成的合成词序号
		/// </summary>
		public int NowIndex { get => nowIndex; set {
				int delta = nowIndex - value;
				for (; delta > 0; delta--)
				{
					NowIndexChangeSuccess=GetNextWordPermutation(nowKeyWord.Count-1, (x, y) => x.CompareTo(y)>0);
					if (!NowIndexChangeSuccess) break;
				}
				for (; delta < 0; delta++)
				{
					NowIndexChangeSuccess=GetNextWordPermutation(nowKeyWord.Count-1, (x, y) => x.CompareTo(y) < 0);
					if (!NowIndexChangeSuccess) break;
				}
				nowIndex = value+delta;
			}
		}
		private bool NowIndexChangeSuccess;
		public List<string> Next(out bool haveNext)
		{
			NowIndex++;
			haveNext = NowIndexChangeSuccess;
			return Now();
		}
		public List<string> Previous(out bool haveNext)
		{
				NowIndex--;
				haveNext = NowIndexChangeSuccess;
				return Now();
		}
		/// <summary>
		/// 当前字典序
		/// </summary>
		public List<string> Now()
		{
			return nowKeyWord;
		}
		/// <summary>
		/// 获取从1-Index的下一个排列
		/// </summary>
		/// <param name="index"></param>
		/// <param name="cmp"></param>
		bool GetNextWordPermutation(int index,Func<string, string, bool> cmp)
		{
			if (index == -1)
			{//已经没有下一个排序
				return false;
			}
			if(GetNextWord(nowKeyWord[index], out string tmp, cmp))
			{//成功获取了下一个字符串
				nowKeyWord[index] = tmp;
				return true;
			}
			else
			{//没有获取到说明当前位已经不够用了
				int myIndex = nowNotSelectWord.Count;
				nowNotSelectWord.Add(nowKeyWord[index]);//将当前位移除
				bool flag = GetNextWordPermutation(index - 1, cmp);//将上一位完成
				if (flag)
				{
					for(int i = 0; i < nowNotSelectWord.Count; i++)
					{
						if (!cmp(nowNotSelectWord[myIndex], nowNotSelectWord[i]))
						{
							myIndex = i;
						}
					}
					nowKeyWord[index] = nowNotSelectWord[myIndex];//把最小的给当前位
				}
				nowNotSelectWord.RemoveAt(myIndex);
				return flag;
				
			}
		}
		/// <summary>
		/// 获取比目标大的最小的字符串
		/// </summary>
		/// <param name="target">原始目标</param>
		/// <param name="newTarget">输出的新目标</param>
		/// <param name="cmp">判断大小方法,用于判断两个字符串的大小</param>
		/// <returns>当没有合适的时候返回false</returns>
		bool GetNextWord(string target,out string newTarget,Func<string,string,bool> cmp)
		{
			for (int i =0;i<nowNotSelectWord.Count;i++)
			{
				if (cmp(target,nowNotSelectWord[i]))
				{
					newTarget = nowNotSelectWord[i];
					nowNotSelectWord[i] = target;
					return true;
				}
			}
			newTarget = null;
			return false;
		}

	}
}
