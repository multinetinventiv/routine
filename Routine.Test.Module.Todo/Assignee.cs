using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Test.Common.Domain;

namespace Routine.Test.Module.Todo
{
	public class Assignee
	{
		private readonly IDomainContext ctx;
		private readonly IRepository<Assignee> repository;

		public Guid Uid{get; private set;}
		public string Name{get; private set;}

		private Assignee() {}
		public Assignee(IDomainContext ctx, IRepository<Assignee> repository)
		{
			this.ctx = ctx;
			this.repository = repository;
		}

		internal Assignee With(string name)
		{
			Name = name;

			Uid = repository.Insert<Guid>(this);

			return this;
		}

		public List<TodoItem> ItemsToBeDone{get{return ctx.Get<TodoItemSearch>().ByAssigneeUidAndDone(Uid, false);}}

		public List<IContactInfo> ContactInfos
		{
			get
			{
				return ctx.Get<EmailContactInfoSearch>().ByOwnerAssigneeUid(Uid)
						  .Cast<IContactInfo>()
						  .Union(
							ctx.Get<PhoneContactInfoSearch>().ByOwnerAssigneeUid(Uid)
						  	   .Cast<IContactInfo>())
						  .OrderByDescending(c => c.DateCreated)
						  .ToList();
			}
		}

		public void Update(string name)
		{
			Name = name;

			repository.Update(this);
		}

		public void AddPhone(string name, string areaCode, string number)
		{
			ctx.New<PhoneContactInfo>().With(this, name, areaCode, number);
		}

		public void AddEmail(string name, string address)
		{
			ctx.New<EmailContactInfo>().With(this, name, address);
		}
	}

	public class AssigneeSearch : Search<Assignee>
	{
		public AssigneeSearch(IDomainContext context)
			: base(context) {}

		public Assignee SingleByName(string name)
		{
			return SingleBy(a => a.Name == name);
		}
	}
}

