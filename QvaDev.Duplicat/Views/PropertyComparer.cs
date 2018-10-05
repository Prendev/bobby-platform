using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using QvaDev.Common.Attributes;

namespace QvaDev.Duplicat.Views
{
	public class PropertyComparer : IComparer<PropertyInfo>
	{
		public int Compare(PropertyInfo x, PropertyInfo y)
		{
			Debug.Assert(x != null, nameof(x) + " != null");
			Debug.Assert(y != null, nameof(y) + " != null");

			var xp = x.GetCustomAttributes(true).FirstOrDefault(a => a is DisplayPriorityAttribute) as DisplayPriorityAttribute;
			var yp = y.GetCustomAttributes(true).FirstOrDefault(a => a is DisplayPriorityAttribute) as DisplayPriorityAttribute;

			if (xp == null && yp == null) return 0;
			if (xp == null) return yp.Reverse ? -1 : 1;
			if (yp == null) return xp.Reverse ? 1 : -1;

			if (xp.Reverse != yp.Reverse) return xp.Reverse ? 1 : -1;
			if (xp.Priority < yp.Priority) return -1 * (xp.Reverse ? -1 : 1);
			if (xp.Priority > yp.Priority) return 1 * (xp.Reverse ? -1 : 1);
			return 0;
		}
	}
}
