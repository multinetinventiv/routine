using System;

namespace Routine.Core.Configuration
{
	public class ConfigurationException : Exception
	{
		private static string GetTypeOf(object relatedObj) { return relatedObj == null ? "null" : relatedObj.GetType().FullName; }

		private static string BuildMessage(string configurationName, object relatedObj, bool onlyName)
		{
			if (string.IsNullOrEmpty(configurationName))
			{
				return string.Format("Cannot get configured value for '{0}' ('{1}')", relatedObj, GetTypeOf(relatedObj));
			}

			if (onlyName)
			{
				return string.Format("Cannot configure '{0}'", configurationName);
			}

			return string.Format("Cannot configure '{0}' for '{1}' ('{2}')", configurationName, relatedObj, GetTypeOf(relatedObj));
		}

		public ConfigurationException() { }
		public ConfigurationException(object relatedObj) : this(null, relatedObj) { }
		public ConfigurationException(string configurationName) : this(configurationName, (Exception)null) { }
		public ConfigurationException(string configurationName, Exception innerException) : this(configurationName, null, innerException, true) { }
		public ConfigurationException(string configurationName, object relatedObj) : this(configurationName, relatedObj, null) { }
		public ConfigurationException(string configurationName, object relatedObj, Exception innerException) : this(configurationName, relatedObj, innerException, false) { }
		private ConfigurationException(string configurationName, object relatedObj, Exception innerException, bool onlyName) : base(BuildMessage(configurationName, relatedObj, onlyName), innerException) { }
	}
}