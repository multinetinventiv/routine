using System;
using System.Collections.Generic;
using Routine.Test.Domain;

namespace Routine.Test.Module.ProjectManagement
{
	public class Customer
	{
		private readonly IDomainContext context;
		private readonly IRepository<Customer> repository;

		private Customer() { }
		public Customer(IDomainContext context, IRepository<Customer> repository)
		{
			this.context = context;
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
