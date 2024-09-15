using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProgressionLock
{
	public struct LockDate
	{
		[System.ComponentModel.DefaultValue(true)]
		[Newtonsoft.Json.JsonProperty(DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Populate)]
		public bool AllowedToSpawn;

		public int HoursFromStart;
	}

	public static class LockDateUtils
	{
		public static LockDate[] Construct(int hrs, bool canSpawn, params LockDate[] others)
		{
			if (others is null || others.Length == 0)
			{
				return new LockDate[] { new LockDate() { HoursFromStart = hrs, AllowedToSpawn = canSpawn } };
			}
			LockDate[] newArr = new LockDate[others.Length + 1];
			others.CopyTo(newArr, 0);
			newArr[newArr.Length - 1] = new LockDate() { HoursFromStart = hrs, AllowedToSpawn = canSpawn };
			return newArr;
		}

		public static int HoursFromDays(int days) => days * 24;
		public static int HoursFromWeeks(int weeks) => weeks * 168;
	}
}
