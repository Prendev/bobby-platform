using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace QvaDev.Common.Services
{
	[XmlRoot("weeklyevents")]
	public class WeeklyEvents
	{
		[XmlElement("event")]
		public List<NewsEvent> Events { get; } = new List<NewsEvent>();
	}

	public class NewsEvent
	{
		public enum ImpactTypes
		{
			Unknown,
			Holiday,
			Low,
			Medium,
			High
		}

		[XmlElement("title")]
		public string Title { get; set; }

		[XmlElement("country")]
		public string Country { get; set; }

		[XmlElement("date")]
		public string Date { get; set; }

		[XmlElement("time")]
		public string Time { get; set; }

		[XmlElement("impact")]
		public string Impact { get; set; }

		public DateTime EventTimeUtc => ParseDateTime();
		public ImpactTypes ImpactType => ParseImpact();

		private DateTime ParseDateTime()
		{
			var dateTime = DateTime
				.ParseExact($"{Date} {Time}", "MM-dd-yyyy h:mmtt", CultureInfo.InvariantCulture);
			dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
			return dateTime;
		}

		private ImpactTypes ParseImpact()
		{
			if (!Enum.TryParse(Impact, out ImpactTypes impactType)) impactType = ImpactTypes.Unknown;
			return impactType;
		}
	}
}
