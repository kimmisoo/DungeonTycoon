using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum PRIORITY_SORT_TYPE { ASCENDING, DESCENDING };

public class PriorityQueue<T> : ICollection<T> where T : IComparable
{
	private readonly List<T> items = new List<T>();

	public int Count { get { return items.Count; } }
	public bool IsReadOnly { get { return false; } }
	public PRIORITY_SORT_TYPE SortType { get; private set; }
	public T this[int index] { get { return items[index]; } }
	public int useCount = 0;
	int parentIndex = 0;
	int index = 0;

	public PriorityQueue()
	{
		SortType = PRIORITY_SORT_TYPE.ASCENDING;
	}

	public PriorityQueue(PRIORITY_SORT_TYPE sortType)
	{
		SortType = sortType;
	}

    // heap에서 삽입 메서드. 문제없음.
	public void Add(T item)
	{
		items.Add(item);
		useCount++;
		index = useCount - 1;//items.Count - 1;
		bool keepGoing = true;

		while (keepGoing)
		{
			if (index == 0)
                break;
			parentIndex = (index - 1) / 2;
			keepGoing = CompareAndSwap(index, parentIndex);
			if (keepGoing)
                index = parentIndex;
		}
	}
	
	public void Clear()
	{
		items.Clear();
		useCount = 0;
	}

	public void HalfClear()
	{
		if(items.Count > 500)
		{
			items.RemoveRange(0, items.Count / 2);
		}
		useCount = 0;
	}

	public bool Contains(T item)
	{
		return items.Contains(item);
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		items.CopyTo(array, arrayIndex);
	}

	public bool Remove(T item)
	{
		
		return RemoveAt(items.IndexOf(item));
	}

    // 이거도 문제없음.
	public bool RemoveAt(int index)
	{
		if (index < 0) return false;

		if (index == (/*items.Count*/useCount - 1))
		{
			items.RemoveAt(index);
			useCount--;
			return true;
		}

		items[index] = items[/*items.Count*/useCount - 1];
		items.RemoveAt(/*items.Count*/useCount - 1);
		useCount--;
		bool keepGoing = true;

		while (keepGoing)
		{
			int childIndex = GetPrimaryChildIndex(index);
			if (childIndex < 0) break;
			keepGoing = CompareAndSwap(childIndex, index);
			if (keepGoing) index = childIndex;
		}
		return true;
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return items.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return items.GetEnumerator();
	}

	public T Pop()
	{
		T item = default(T);

		if (/*items.Count*/useCount > 0)
		{
			item = items[0];
			RemoveAt(0);		
		}

		return item;
	}

	public T Peek()
	{
		return (/*items.Count*/ useCount > 0) ? items[0] : default(T);
	}

	public bool CompareAndSwap(int indexA, int indexB)
	{
		int result = items[indexA].CompareTo(items[indexB]);

		if (SortType == PRIORITY_SORT_TYPE.ASCENDING)
		{
			if (result < 0)
			{
				Swap(indexA, indexB);
				return true;
			}
			else
			{
				return false;
			}
		}
		else
		{
			if (result > 0)
			{
				Swap(indexA, indexB);
				return true;
			}
			else
			{
				return false;
			}
		}
	}

	void Swap(int indexA, int indexB)
	{
		T temp = items[indexB];
		items[indexB] = items[indexA];
		items[indexA] = temp;
	}

	int GetPrimaryChildIndex(int parentIndex)
	{
		int primaryChildIndex;
		int leftChildIndex = ((parentIndex + 1) * 2) - 1;
		int rightChildIndex = (parentIndex + 1) * 2;

        // 자식 둘다 있을 때.
		if ((leftChildIndex < useCount/*items.Count*/) && (rightChildIndex < useCount/*items.Count*/))
		{
			int result = items[leftChildIndex].CompareTo(items[rightChildIndex]);

			if (SortType == PRIORITY_SORT_TYPE.ASCENDING)
			{
				if (result < 0)
				{
					primaryChildIndex = leftChildIndex;
				}
				else
				{
					primaryChildIndex = rightChildIndex;
				}
			}
			else
			{
				if (result > 0)
				{
					primaryChildIndex = leftChildIndex;
				}
				else
				{
					primaryChildIndex = rightChildIndex;
				}
			}
		}
		else
		{
			if (leftChildIndex < useCount/*items.Count*/)
			{
				primaryChildIndex = leftChildIndex;
			}
			else if (rightChildIndex < useCount/*items.Count*/)
			{
				primaryChildIndex = rightChildIndex;
			}
			else
			{
				primaryChildIndex = -1;
			}
		}

		return primaryChildIndex;
	}
}