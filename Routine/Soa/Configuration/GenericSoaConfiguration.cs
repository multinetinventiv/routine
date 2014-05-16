using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Routine.Core;
using Routine.Core.Extractor;
using Routine.Core.Interceptor;
using Routine.Soa.Context;

namespace Routine.Soa.Configuration
{
	public class GenericSoaConfiguration : ISoaConfiguration
	{
		private const string ACTION = "action";

		private List<string> DefaultParameters { get; set; }
		public GenericSoaConfiguration DefaultParametersAre(params string[] parameters) { DefaultParameters.AddRange(parameters); return this; }

		public MultipleExtractor<GenericSoaConfiguration, Exception, SoaExceptionResult> ExtractExceptionResult { get; private set; }

		public GenericSoaConfiguration() : this(true) { }

		internal GenericSoaConfiguration(bool rootConfig)
		{
			DefaultParameters = new List<string>();

			ExtractExceptionResult = new MultipleExtractor<GenericSoaConfiguration, Exception, SoaExceptionResult>(this, "ExceptionResult");
			
			if (rootConfig)
			{
				RegisterRoutes();
			}
		}

		public GenericSoaConfiguration Merge(GenericSoaConfiguration other)
		{
			DefaultParameters.AddRange(other.DefaultParameters);

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

		string ISoaConfiguration.ActionRouteName { get { return ACTION; } }
		List<string> ISoaConfiguration.DefaultParameters { get { return DefaultParameters; } }

		IExtractor<Exception, SoaExceptionResult> ISoaConfiguration.ExceptionResultExtractor { get { return ExtractExceptionResult; } }

		#endregion
	}
}

