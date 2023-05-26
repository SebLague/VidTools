using System.Collections.Generic;

namespace VidTools.Vis.Internal
{
	public class PathDrawPool
	{
		Queue<PathDrawer> available;
		HashSet<PathDrawer> inUse;

		public PathDrawPool()
		{
			available = new Queue<PathDrawer>();
			inUse = new HashSet<PathDrawer>();
		}

		public PathDrawer Get()
		{
			while (available.Count > 0)
			{
				PathDrawer pooledItem = available.Dequeue();
				if (pooledItem != null)
				{
					inUse.Add(pooledItem);
					return pooledItem;
				}
			}

			PathDrawer newItem = new PathDrawer();
			inUse.Add(newItem);
			return newItem;
		}

		public void Return(PathDrawer item)
		{
			available.Enqueue(item);
			inUse.Remove(item);
		}

		public void ReturnAll()
		{
			foreach (PathDrawer item in inUse)
			{
				available.Enqueue(item);
			}
			inUse.Clear();
		}


		public void ReturnAndReleaseAll()
		{
			ReturnAll();
			foreach (var item in available)
			{
				item.Release();
			}
			available.Clear();
		}
	}
}