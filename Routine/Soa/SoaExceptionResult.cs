using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Routine.Soa
{
	public class SoaExceptionResult
	{
		public bool IsException { get { return true; } }
		public bool IsHandled { get; private set; }
		public string Type { get; private set; }
		public string Message { get; private set; }

		public SoaExceptionResult() : this("NaN", "NaN", false) { }
		public SoaExceptionResult(string type, string message) : this(type, message, true) { }
		public SoaExceptionResult(string type, string message, bool handled)
		{
			Type = type;
			Message = message;
			IsHandled = handled;
		}
	}
}
