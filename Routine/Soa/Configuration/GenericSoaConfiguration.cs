using System;
using System.Web.Mvc;
using System.Web.Routing;
using Routine.Core;
using Routine.Core.Extractor;

namespace Routine.Soa.Configuration
{
	public class GenericSoaConfiguration : ISoaConfiguration
	{
		public MultipleExtractor<GenericSoaConfiguration, Exception, SoaExceptionResult> ExceptionResult { get; private set; }

		public GenericSoaConfiguration() : this(true) { }

		internal GenericSoaConfiguration(bool rootConfig)
		{
			ExceptionResult = new MultipleExtractor<GenericSoaConfiguration, Exception, SoaExceptionResult>(this, "ExceptionResult");

			if (rootConfig)
			{
				RegisterRoutes();
			}
		}

		public GenericSoaConfiguration Merge(GenericSoaConfiguration other)
		{
			ExceptionResult.Merge(other.ExceptionResult);

			return this;
		}

		private void RegisterRoutes()
		{
			RouteTable.Routes.MapRoute(
				"Soa",
				"Soa/{action}/{id}",
				new {controller="Soa", action="Index", id=""}
			);
		}

		#region ISoaConfiguration implementation

		IExtractor<Exception, SoaExceptionResult> ISoaConfiguration.ExceptionResultExtractor { get { return ExceptionResult; } }

		#endregion
	}
}

