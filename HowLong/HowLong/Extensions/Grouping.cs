using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HowLong.Extensions
{
	public class Grouping<TK, T> : ObservableCollection<T>
	{
		public TK Key { get; }
		public Grouping(TK name, IEnumerable<T> items)
		{
			Key = name;
			foreach (var item in items)
				Items.Add(item);
		}
	}
}
