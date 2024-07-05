using System.Collections.Generic;
using System.Linq;
using TShockAPI;

using static ProgressionLock.Util;

namespace ProgressionLock
{
	#region New
	internal class CompareHours : IComparer<LockDate>
	{
		private static CompareHours? _instance;
		public static CompareHours Instance => _instance ??= new CompareHours();
		public int Compare(LockDate a, LockDate b)
		{
			return a.HoursFromStart.CompareTo(b.HoursFromStart);
		}
	}
	#endregion
	public class ProgressionLockerConfig
	{
		#region New
		[System.Text.Json.Serialization.JsonInclude]
		[Newtonsoft.Json.JsonProperty(DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Populate)]
		public SimpleDateFormat serverStartTime;

		[System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.Always)]
		[Newtonsoft.Json.JsonProperty(DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Populate)]
		internal Dictionary<Entities, LockDate[]> locks = new Dictionary<Entities, LockDate[]>();
		
		public bool TryGetRecent(Entities npcType, out LockDate progLocked)
		{
			if (!locks.ContainsKey(npcType))
			{
				progLocked = default;
				return false;
			}
			int hrsSinceStart = (int)DateTime.Now.Subtract(serverStartTime.Convert()).TotalHours;
			if (!locks[npcType].Any(x => x.HoursFromStart <= hrsSinceStart))
			{
				progLocked = default;
				return false;
			}
			progLocked = locks[npcType].Where(y => y.HoursFromStart <= hrsSinceStart).ToArray().Sort(CompareHours.Instance).Last();
			return true;
		}

		public bool TryGetNext(Entities type, out LockDate progLocked)
		{
			if (!locks.ContainsKey(type))
			{
				progLocked = default;
				return false;
			}
			int hrsSinceStart = (int)DateTime.Now.Subtract(serverStartTime.Convert()).TotalHours;
			if (!locks[type].Any(x => x.HoursFromStart > hrsSinceStart))
			{
				TryGetRecent(type, out progLocked);
				return false;
			}
			progLocked = locks[type].Where(x => x.HoursFromStart > hrsSinceStart).ToArray().Sort(CompareHours.Instance).First();
			return true;
		}

		public void ReplaceWith(Entities type, LockDate[] newArr)
		{
			if (!locks.ContainsKey(type))
				return;
			locks[type] = newArr;
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
