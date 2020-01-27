using System;
using System.Collections;
using System.Collections.Generic;

namespace DDONamedGearPlanner
{
	public class LargeContainer<T>
	{
		const int PAGE_SIZE = 1000000;
		List<T[]> Pages = new List<T[]>();

		int LastPage = 0;
		int LastIndex = -1;

		public ulong Count => Pages.Count == 0 ? 0 : (ulong)((Pages.Count - 1) * PAGE_SIZE) + (ulong)(LastIndex + 1);
		public T Last => (LastPage > -1 && LastIndex > -1) ? Pages[LastPage][LastIndex] : default(T);
		public T this[ulong u] => Pages[(int)(u / PAGE_SIZE)][(int)(u % PAGE_SIZE)];

		public LargeContainer(ulong capacity)
		{
			Pages = new List<T[]>((int)Math.Max(capacity / PAGE_SIZE, 1));
		}

		public void Add(T t)
		{
			if (LastPage == Pages.Count || (LastIndex == PAGE_SIZE - 1))
			{
				Pages.Add(new T[PAGE_SIZE]);
				LastPage = Pages.Count - 1;
				LastIndex = -1;
			}

			Pages[LastPage][++LastIndex] = t;
		}

		public void Clear()
		{
			Pages.Clear();
			Pages = null;
		}
	}
}
