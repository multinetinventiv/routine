namespace Routine.Service
{
	public class ExceptionResult
	{
		public bool IsException { get { return true; } }
		public bool IsHandled { get; private set; }
		public string Type { get; private set; }
		public string Message { get; private set; }

		internal ExceptionResult(ExceptionResultData data) : this(data.Type, data.Message, data.IsHandled) { }
		public ExceptionResult() : this("NaN", "NaN", false) { }
		public ExceptionResult(string type, string message) : this(type, message, true) { }
		public ExceptionResult(string type, string message, bool handled)
		{
			Type = type;
			Message = message;
			IsHandled = handled;
		}
		}

	internal class ExceptionResultData
	{
		public bool IsException { get; set; }
		public bool IsHandled { get; set; }
		public string Type { get; set; }
		public string Message { get; set; }
	}
}
