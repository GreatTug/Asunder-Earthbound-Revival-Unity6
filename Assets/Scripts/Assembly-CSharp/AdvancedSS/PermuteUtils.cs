using System.Collections.Generic;

namespace AdvancedSS
{
	public class PermuteUtils
	{
		public static IEnumerable<IEnumerable<T>> Permute<T>(IEnumerable<T> list, int count)
		{
			if (count == 0)
			{
				yield return new T[0];
				yield break;
			}
			int startingElementIndex = 0;
			foreach (T startingElement in list)
			{
				IEnumerable<T> remainingItems = AllExcept(list, startingElementIndex);
				using (IEnumerator<IEnumerable<T>> enumerator2 = Permute(remainingItems, count - 1).GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						yield return Concat(b: enumerator2.Current, a: new T[1] { startingElement });
					}
				}
				startingElementIndex++;
			}
		}

		public static IEnumerable<T> Concat<T>(IEnumerable<T> a, IEnumerable<T> b)
		{
			foreach (T item in a)
			{
				yield return item;
			}
			foreach (T item2 in b)
			{
				yield return item2;
			}
		}

		public static IEnumerable<T> AllExcept<T>(IEnumerable<T> input, int indexToSkip)
		{
			int index = 0;
			foreach (T item in input)
			{
				if (index != indexToSkip)
				{
					yield return item;
				}
				index++;
			}
		}
	}
}
