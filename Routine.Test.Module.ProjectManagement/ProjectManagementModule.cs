using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Routine.Test.Domain;

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
			ctx.New<Project>().With("should be rolled back");
			throw new Exception("test exception");
		}

		public void CreateTestData()
		{
			NewProject("Routine").NewFeature("UI");
			NewProject("Multinet.Framework").NewFeature("Dependency Injection");
		}

		public string TestRequestCache(Project project, string name)
		{
			var otherProject = ctx.Query<Projects>().All().Single(p => p.Uid == project.Uid);

			otherProject.Rename(name);

			return project.Name;
		}
	}
}
