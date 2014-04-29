using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Routine.Test.Common;
using Routine.Test.Domain;
using Routine.Test.Module.ProjectManagement.Api;

namespace Routine.Test.Module.ProjectManagement
{
	public class Feature : IFeature
	{
		private IDomainContext ctx;
		private IRepository<Feature> repository;

		private Feature() { }
		public Feature(IDomainContext ctx, IRepository<Feature> repository)
		{
			this.ctx = ctx;
			this.repository = repository;
		}

		public Guid Uid { get; private set; }
		public string Name { get; private set; }
		public Guid ProjectUid { get; private set; }

		internal Feature With(string name, Project project)
		{
			Name = name;
			ProjectUid = project.Uid;

			Uid = repository.Insert<Guid>(this);

			return this;
		}

		public Project Project { get { return ctx.Query<Projects>().ByUid(ProjectUid); } }
		public List<ITask> Tasks { get { return ctx.Query<FeatureTasks>().ByFeature(this).Select(f => f.Task).ToList(); } }

		public void AddTask(ITask task)
		{
			ctx.New<FeatureTask>().With(this, task);
		}
	}

	public class Features : Query<Feature>
	{
		public Features(IDomainContext ctx) : base(ctx) { }

		public List<Feature> ByProject(Project project)
		{
			return By(f => f.ProjectUid == project.Uid);
		}
	}
}
