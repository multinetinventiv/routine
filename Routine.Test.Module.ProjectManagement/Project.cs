using System;
using System.Collections.Generic;
using Routine.Test.Domain;
using Routine.Test.Module.ProjectManagement.Api;
using System.Linq;

namespace Routine.Test.Module.ProjectManagement
{
	public class Project : IProject
	{
		private readonly IDomainContext ctx;
		private readonly IRepository<Project> repository;

		private Project() { }
		public Project(IDomainContext ctx, IRepository<Project> repository)
		{
			this.ctx = ctx;
			this.repository = repository;
		}

		public Guid Uid { get; private set; }
		public string Name { get; private set; }
		public DateTime Deadline { get; private set; }
		public Guid CustomerUid { get; private set; }

		internal Project With(Customer customer, string name) { return With(customer, name, default(DateTime)); }
		internal Project With(Customer customer, string name, DateTime deadline)
		{
			Name = name;
			Deadline = deadline;
			CustomerUid = customer.Uid;

			repository.Insert(this);

			return this;
		}

		internal Project With(NewProject newProject) 
		{ 
			With(newProject.Customer, newProject.Name, newProject.Deadline);

			foreach (var feature in newProject.Features)
			{
				NewFeature(feature.Name);
			}

			return this;
		}

		public List<string> GetAvailableNamesForRename()
		{
			return new List<string>{ Name + "_1", Name + "_2" };
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

	public class Projects : Query<Project>, IProjects
	{
		public Projects(IDomainContext ctx) : base(ctx) { }

		internal new List<Project> All()
		{
			return base.All();
		}

		public List<Project> ByName(string name)
		{
			return By(p => p.Name.StartsWith(name));
		}

		#region IProjects implementation

		List<IProject> IProjects.All(){return All().Cast<IProject>().ToList();}

		#endregion
	}

	public struct NewProject
	{
		public Customer Customer { get; private set; }
		public string Name { get; private set; }
		public List<NewFeature> Features { get; private set; }

		public DateTime Deadline { get; set; }

		public NewProject(Customer customer, DateTime deadline, string name, params NewFeature[] features)
			: this()
		{
			Customer = customer;
			Name = name;
			Features = new List<NewFeature>(features);

			Deadline = deadline;
		}
		public NewProject(Customer customer, string name, bool someBool) : this(customer, default(DateTime), name) { }
	}
}
