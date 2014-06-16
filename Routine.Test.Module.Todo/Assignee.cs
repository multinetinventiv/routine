using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Test.Common;
using Routine.Test.Domain;
using Routine.Test.Module.Todo.Api;

namespace Routine.Test.Module.Todo
{
	[Cached]
	public class Assignee : IAssignee
	{
		private readonly IDomainContext ctx;
		private readonly IRepository<Assignee> repository;

		public Guid Uid{get; private set;}
		public string Name{get; private set;}
		public FatString Address { get; private set; }

		private Assignee() {}
		public Assignee(IDomainContext ctx, IRepository<Assignee> repository)
		{
			this.ctx = ctx;
			this.repository = repository;
		}

		internal Assignee With(string name, FatString address)
		{
			Name = name;
			Address = address;

			Uid = repository.Insert<Guid>(this);

			return this;
		}

		public List<TodoItem> ItemsToBeDone { get { return ctx.Query<TodoItems>().ByAssigneeUidAndDone(Uid, false); } }

		public List<IContactInfo> ContactInfos
		{
			get
			{
				return ctx.Query<EmailContactInfos>().ByOwnerAssigneeUid(Uid)
						  .Cast<IContactInfo>()
						  .Union(
							ctx.Query<PhoneContactInfos>().ByOwnerAssigneeUid(Uid)
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

		[Hidden]
		public void ReleaseAllItems()
		{
			foreach (var item in ItemsToBeDone)
			{
				item.Release();
			}
		}

		public void AddPhone(string name, string areaCode, string number)
		{
			ctx.New<PhoneContactInfo>().With(this, name, areaCode, number);
		}

		public void AddEmail(string name, string address)
		{
			ctx.New<EmailContactInfo>().With(this, name, address);
		}

		#region IAssignee implementation
		List<ITodoItem> IAssignee.ItemsToBeDone
		{
			get { return ItemsToBeDone.Cast<ITodoItem>().ToList(); }
		} 
		#endregion
	}

	public class Assignees : Query<Assignee>
	{
		public Assignees(IDomainContext context)
			: base(context) {}

		public new List<Assignee> All()
		{
			return base.All();
		}

		public Assignee SingleByName(string name)
		{
			return SingleBy(a => a.Name == name);
		}

		public List<Assignee> ByName(string name)
		{
			return By(a => a.Name.Contains(name));
		}
	}
}

