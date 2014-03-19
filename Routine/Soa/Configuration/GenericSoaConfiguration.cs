using System;
using System.Web.Mvc;
using System.Web.Routing;
using Routine.Core;
using Routine.Core.Extractor;

namespace Routine.Soa.Configuration
{
	public class GenericSoaConfiguration : ISoaConfiguration
	{
		private const string ACTION = "action";

		public MultipleExtractor<GenericSoaConfiguration, Exception, SoaExceptionResult> ExtractExceptionResult { get; private set; }

		public GenericSoaConfiguration() : this(true) { }

		internal GenericSoaConfiguration(bool rootConfig)
		{
			ExtractExceptionResult = new MultipleExtractor<GenericSoaConfiguration, Exception, SoaExceptionResult>(this, "ExceptionResult");

			if (rootConfig)
			{
				RegisterRoutes();
			}
		}

		public GenericSoaConfiguration Merge(GenericSoaConfiguration other)
		{
			ExtractExceptionResult.Merge(other.ExtractExceptionResult);

			return this;
		}

		private void RegisterRoutes()
		{
			RouteTable.Routes.MapRoute(
				"Soa",
				"Soa/{"+ACTION+"}/{id}",
				new {controller="Soa", action="Index", id=""}
			);
		}

		#region ISoaConfiguration implementation

		IExtractor<Exception, SoaExceptionResult> ISoaConfiguration.ExceptionResultExtractor { get { return ExtractExceptionResult; } }
		string ISoaConfiguration.ActionRouteName { get { return ACTION; } }

		#endregion
	}
}

