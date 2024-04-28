using System.Collections.Generic;
using System.Linq;
using TShockAPI;

using static ProgressionLock.Util;

namespace ProgressionLock
{
	#region Prev
	/*
	internal class TimeCompare : IComparer<DateTime>
	{
					private static TimeCompare? _instance;
					public static TimeCompare Instance
					{
									get
									{
													return _instance ?? (_instance = new TimeCompare());
									}
					}
					public int Compare(DateTime a, DateTime b)
					{
									return TimeSpan.Compare(DateTime.Now.Subtract(a).Duration(), DateTime.Now.Subtract(b).Duration());
					}
	}
	*/
	#endregion
	#region New
	internal class CompareHours : IComparer<LockDate>
	{
		private static CompareHours? _instance;
		public static CompareHours Instance => _instance ?? (_instance = new CompareHours());
		public int Compare(LockDate a, LockDate b)
		{
			return a.HoursFromStart.CompareTo(b.HoursFromStart);
		}
	}
	#endregion
	public class ProgressionLockerConfig
	{
		#region Prev
		/*
		[System.Text.Json.Serialization.JsonInclude]
		public IReadOnlyDictionary<Entities, LockDate[]> locks = new Dictionary<Entities, LockDate[]>();

		public LockDate GetMostRecentForType(Entities type)
		{
						LockDate[] locks = this.locks[type];
						DateTime now = DateTime.Now;
						DateTime lookingFor = locks.Select(x => x.StartDate.Convert())
										.Where(x => x.Year <= now.Year && x.Month <= now.Month && x.Day <= now.Day)
										.ToArray().Sort(TimeCompare.Instance).First();
						return locks.First(x => x.StartDate.Convert().Equals(lookingFor));
		}

		public bool TryGetRecent(Entities npcType, out LockDate progLocked)
		{
						if (!locks.ContainsKey(npcType))
						{
										progLocked = default;
										return false;
						}
						progLocked = GetMostRecentForType(npcType);
						return true;
		}
		*/
		#endregion

		#region New
		[System.Text.Json.Serialization.JsonInclude]
		[Newtonsoft.Json.JsonProperty(DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Populate)]
		public SimpleDateFormat serverStartDate;

		[System.Text.Json.Serialization.JsonInclude]
		[Newtonsoft.Json.JsonProperty(DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Populate)]
		public IReadOnlyDictionary<Entities, LockDate[]> locks = new Dictionary<Entities, LockDate[]>();

		/*public LockDate GetMostRecentForType(Entities type)
		{
						LockDate[] locks = this.locks[type];
						int hrsSinceStart = (int)DateTime.Now.Subtract(serverStartDate.Convert()).TotalHours;
						locks = locks.Where(y => y.HoursFromStart <= hrsSinceStart).ToArray().Sort(CompareHours.Instance);
						return locks.Last();
		}*/

		public bool TryGetRecent(Entities npcType, out LockDate progLocked)
		{
			if (!locks.ContainsKey(npcType))
			{
				progLocked = default;
				return false;
			}
			int hrsSinceStart = (int)DateTime.Now.Subtract(serverStartDate.Convert()).TotalHours;
			if (!locks[npcType].Any(x => x.HoursFromStart <= hrsSinceStart))
			{
				progLocked = default;
				return false;
			}
			progLocked = locks[npcType].Where(y => y.HoursFromStart <= hrsSinceStart).ToArray().Sort(CompareHours.Instance).Last();
			return true;
		}
		#endregion
	}

	public class LockerConfigFile : TShockAPI.Configuration.ConfigFile<ProgressionLockerConfig>
	{
		public static readonly string DirectoryPath = Path.Combine(TShock.SavePath, "Lock");
		public static readonly string FilePath = Path.Combine(DirectoryPath, "Lock.json");

		public bool TryRead(string filePath, out ProgressionLockerConfig config)
		{
			config = Read(filePath, out bool notFound);
			return !notFound;
		}
	}
}
