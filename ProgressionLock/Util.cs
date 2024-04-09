using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.ID;

namespace ProgressionLock
{
				public static class Util
				{
								public static T[] Sort<T>(this T[] arr, IComparer<T>? comp = null)
								{
												Array.Sort(arr, comp);
												return arr;
								}
				}
}
