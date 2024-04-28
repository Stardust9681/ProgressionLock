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
		#region Prev
		//public SimpleDateFormat StartDate;
		#endregion

		[System.ComponentModel.DefaultValue(true)]
		[Newtonsoft.Json.JsonProperty(DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Populate)]
		public bool AllowedToSpawn;

		#region New
		public int HoursFromStart;
		#endregion
	}
}
