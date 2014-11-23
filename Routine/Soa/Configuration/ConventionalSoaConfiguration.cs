using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Routine.Core.Configuration;

namespace Routine.Soa.Configuration
{
	public class ConventionalSoaConfiguration : ISoaConfiguration
	{
		private const string ACTION = "action";

		public SingleConfiguration<ConventionalSoaConfiguration, int> MaxResultLength { get; private set; }
		public ListConfiguration<ConventionalSoaConfiguration, string> Headers { get; private set; }
		public ConventionalConfiguration<ConventionalSoaConfiguration, Exception, SoaExceptionResult> ExceptionResult { get; private set; }

		public ConventionalSoaConfiguration() : this(true) { }
		internal ConventionalSoaConfiguration(bool rootConfig)
		{
			MaxResultLength = new SingleConfiguration<ConventionalSoaConfiguration, int>(this, "MaxResultLength", true);
			Headers = new ListConfiguration<ConventionalSoaConfiguration, string>(this, "Headers");
			ExceptionResult = new ConventionalConfiguration<ConventionalSoaConfiguration, Exception, SoaExceptionResult>(this, "ExceptionResult");

			if (rootConfig)
			{
				RegisterRoutes();
			}
		}

		public ConventionalSoaConfiguration Merge(ConventionalSoaConfiguration other)
		{
			Headers.Merge(other.Headers);
			ExceptionResult.Merge(other.ExceptionResult);

			return this;
		}

		private void RegisterRoutes()
		{
			RouteTable.Routes.MapRoute(
				"Soa",
				"Soa/{" + ACTION + "}/{id}",
				new { controller = "Soa", action = "Index", id = "" }
			);
		}

		#region ISoaConfiguration implementation

		string ISoaConfiguration.GetActionRouteName() { return ACTION; }
		List<string> ISoaConfiguration.GetHeaders() { return Headers.Get(); }
		int ISoaConfiguration.GetMaxResultLength() { return MaxResultLength.Get(); }
		SoaExceptionResult ISoaConfiguration.GetExceptionResult(Exception exception) { return ExceptionResult.Get(exception); }

		#endregion
	}
}

