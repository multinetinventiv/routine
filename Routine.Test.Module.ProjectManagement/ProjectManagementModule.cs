using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Routine.Test.Common.Domain;

namespace Routine.Test.Module.ProjectManagement
{
	public class ProjectManagementModule
	{
		private readonly IDomainContext ctx;

		public ProjectManagementModule(IDomainContext ctx)
		{
			this.ctx = ctx;
		}

		public Project NewProject(string name)
		{
			return ctx.New<Project>().With(name);
		}

		public void TestError()
		{
			throw new Exception("test exception");
		}

		public void CreateTestData()
		{
			NewProject("Routine").NewFeature("UI");
			NewProject("Multinet.Framework").NewFeature("Dependency Injection");
		}
	}
}
