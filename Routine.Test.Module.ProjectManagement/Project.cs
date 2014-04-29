using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Routine.Test.Domain;

namespace Routine.Test.Module.ProjectManagement
{
	public class Project
	{
		private IDomainContext ctx;
		private IRepository<Project> repository;

		private Project() { }
		public Project(IDomainContext ctx, IRepository<Project> repository)
		{
			this.ctx = ctx;
			this.repository = repository;
		}

		public Guid Uid { get; private set; }
		public string Name { get; private set; }

		internal Project With(string name)
		{
			Name = name;

			Uid = repository.Insert<Guid>(this);

			return this;
		}

		public void Rename(string name)
		{
			Name = name;

			repository.Update(this);
		}

		public void NewFeature(string name)
		{
			ctx.New<Feature>().With(name, this);
		}

		public List<Feature> Features { get { return ctx.Query<Features>().ByProject(this); } }
	}

	public class Projects : Query<Project>
	{
		public Projects(IDomainContext ctx) : base(ctx) { }
	}
}
