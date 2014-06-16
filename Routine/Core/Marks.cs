using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Routine.Core
{
	public class Marks
	{
		private Dictionary<string, bool> marks;

		public Marks(IEnumerable<string> list)
		{
			marks = new Dictionary<string, bool>();

			foreach (var mark in list)
			{
				marks.Add(mark, true);
			}
		}

		public List<string> List { get { return marks.Keys.ToList(); } }

		public bool Has(string mark) { return marks.ContainsKey(mark); }

		public void Join(IEnumerable<string> list)
		{
			foreach (var mark in list)
			{
				if (!marks.ContainsKey(mark))
				{
					marks.Add(mark, true);
				}
			}
		}
	}
}
