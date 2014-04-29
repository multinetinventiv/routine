using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Routine.Core.Rest
{
	public interface IRestSerializer
	{
		T Deserialize<T>(string responseString);
		object Deserialize(string responseString);
	}

	public class JsonRestSerializer : IRestSerializer
	{
		private JavaScriptSerializer realSerializer;

		public JsonRestSerializer() : this(new JavaScriptSerializer()) { }
		public JsonRestSerializer(JavaScriptSerializer realSerializer)
		{
			if (realSerializer == null) { throw new ArgumentNullException("realSerializer"); }

			this.realSerializer = realSerializer;
		}

		public T Deserialize<T>(string responseString)
		{
			return realSerializer.Deserialize<T>(responseString);
		}

		public object Deserialize(string responseString)
		{
			return realSerializer.DeserializeObject(responseString);
		}
	}
}
