using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Routine.Test.Domain;

namespace Routine.Test.Module.ProjectManagement
{
	public class Customer
	{
		private IDomainContext ctx;
		private IRepository<Customer> repository;

		private Customer() { }
		public Customer(IDomainContext ctx, IRepository<Customer> repository)
		{
			this.ctx = ctx;
			this.repository = repository;
		}

		public Guid Uid { get; private set; }
		public string Name { get; private set; }

		internal Customer With(string name)
		{
			Name = name;

			repository.Insert(this);

			return this;
		}
	}

	public class Customers : Query<Customer>
	{
		public Customers(IDomainContext context) : base(context) { }

		public new List<Customer> All()
		{
			return base.All();
		}
	}
}
