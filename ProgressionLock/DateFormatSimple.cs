using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProgressionLock
{
	public struct SimpleDateFormat
	{
		[JsonInclude]
		public Month Month;
		[JsonInclude]
		public byte Day;
		[JsonInclude]
		public short Year;
		[JsonInclude]
		public byte Hours;

		public SimpleDateFormat(DateTime date)
		{
			Month = (Month)date.Month;
			Day = (byte)date.Day;
			Year = (short)date.Year;
			Hours = (byte)date.TimeOfDay.Hours;
		}

		public DateTime Convert()
		{
			return new DateTime((int)Year, (int)Month, (int)Day, (int)Hours, 0, 0);
		}
	}

	public enum Month : byte
	{
		Jan = 1,
		Feb = 2,
		Mar = 3,
		Apr = 4,
		May = 5,
		Jun = 6,
		Jul = 7,
		Aug = 8,
		Sep = 9,
		Oct = 10,
		Nov = 11,
		Dec = 12
	}
}
