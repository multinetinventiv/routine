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

		internal SoaExceptionResult(SoaExceptionResultData data) : this(data.Type, data.Message, data.IsHandled) { }
		public SoaExceptionResult() : this("NaN", "NaN", false) { }
		public SoaExceptionResult(string type, string message) : this(type, message, true) { }
		public SoaExceptionResult(string type, string message, bool handled)
		{
			Type = type;
			Message = message;
			IsHandled = handled;
		}

	}

	internal class SoaExceptionResultData
	{
		public bool IsException { get; set; }
		public bool IsHandled { get; set; }
		public string Type { get; set; }
		public string Message { get; set; }
	}
}
