using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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

		public List<Project> Projects { get { return ctx.Query<Projects>().All(); } }

		public Customer NewCustomer(string name)
		{
			return ctx.New<Customer>().With(name);
		}

		public Project NewProject(string name) { return NewProject(ctx.Query<Customers>().All().First(), name); }
		public Project NewProject(Customer customer, string name)
		{
			return ctx.New<Project>().With(customer, name);
		}

		public void TestError()
		{
			NewProject(ctx.Query<Customers>().All().First(), "should be rolled back");
			throw new Exception("test exception");
		}

		public string TestLanguageCode()
		{
			return Thread.CurrentThread.CurrentCulture.Name;
		}

		public List<string> TextMaxLength(int count)
		{
			var result = new List<string>();

			for (int i = 0; i < count * 100; i++)
			{
				result.Add(CreateBigString(i));
			}

			return result;
		}

		private string CreateBigString(int ix)
		{
			var result = ix + "-";

			for (int i = 0; i < 100; i++)
			{
				result += "X";
			}

			return result;
		}

		public void CreateTestData()
		{
			var ikinoktabir = NewCustomer("ikinoktabir");
			NewProject(ikinoktabir, "Routine").NewFeature("UI");

			var multinet = NewCustomer("Multinet");
			NewProject(multinet, "Multinet.Framework").NewFeature("Dependency Injection");
		}

		public string TestRequestCache(Project project, string name)
		{
			var otherProject = ctx.Query<Projects>().All().Single(p => p.Uid == project.Uid);

			otherProject.Rename(name);

			return project.Name;
		}

		//public CreateProjectsOperation BeginCreateProjects()
		//{
		//	return ctx.New<CreateProjectsOperation>();
		//}

		//TODO: input data sample
		public List<Project> CreateProjects(DateTime defaultDeadline, List<NewProject> projects)
		{
			var op = new CreateProjectsOperation(ctx);

			op.DefaultDeadline = defaultDeadline;
			op.Projects.AddRange(projects);

			return op.Create();
		}
	}

	internal class CreateProjectsOperation
	{
		private readonly IDomainContext ctx;

		public CreateProjectsOperation(IDomainContext ctx)
		{
			this.ctx = ctx;

			Projects = new List<NewProject>();
		}

		public DateTime DefaultDeadline { get; set; }
		public List<NewProject> Projects { get; private set; }

		public List<Project> Create()
		{
			var result = new List<Project>();

			foreach (var project in Projects)
			{
				var newProject = project;

				if (newProject.Deadline == default(DateTime))
				{
					newProject.Deadline = DefaultDeadline;
				}

				result.Add(ctx.New<Project>().With(newProject));
			}

			return result;
		}
	}
}
