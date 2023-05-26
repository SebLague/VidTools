using System.Collections.Generic;

namespace VidTools.Vis.Internal
{
	public class Pool<T>
	{
		Queue<T> available;
		HashSet<T> inUse;

		public Pool()
		{
			available = new Queue<T>();
			inUse = new HashSet<T>();
		}

		public T GetItem()
		{
			while (available.Count > 0)
			{
				T pooledItem = available.Dequeue();
				if (pooledItem != null)
				{
					inUse.Add(pooledItem);
					return pooledItem;
				}
			}

			T newItem = System.Activator.CreateInstance<T>();
			inUse.Add(newItem);
			return newItem;
		}

		public void FinishedUsingItem(T item)
		{
			available.Enqueue(item);
			inUse.Remove(item);
		}

		public void FinishedUsingAllitems()
		{
			foreach (T items in inUse)
			{
				available.Enqueue(items);
			}
			inUse.Clear();
		}
	}
}