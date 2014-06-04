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

		public new List<Project> All()
		{
			return base.All();
		}
	}

	internal struct NewProject
	{
		public Customer Customer { get; private set; }
		public string Name { get; private set; }
		public List<NewFeature> Features { get; private set; }

		public DateTime Deadline { get; set; }

		public NewProject(Customer customer, string name) : this(customer, name, default(DateTime)) { }
		public NewProject(Customer customer, string name, DateTime deadline) : this()
		{
			Customer = customer;
			Name = name;
			Features = new List<NewFeature>();

			Deadline = deadline;
		}
	}
}
