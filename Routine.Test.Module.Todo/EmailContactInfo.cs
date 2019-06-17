using System;
using System.Collections.Generic;
using Routine.Test.Common;
using Routine.Test.Domain;

namespace Routine.Test.Module.Todo
{
	public class EmailContactInfo : IContactInfo
	{
		private readonly IRepository<EmailContactInfo> repository;
		private readonly IDomainContext ctx;

		public Guid Uid{get; private set;}
		public Guid OwnerAssigneeUid  { get; private set; }
		public string Name{get;private set;}
		public string Address  { get; private set; }
		public DateTime DateCreated  { get; private set; }

		private EmailContactInfo() {}
		public EmailContactInfo(IDomainContext ctx, IRepository<EmailContactInfo> repository)
		{
			this.ctx = ctx;
			this.repository = repository;
		}

		internal EmailContactInfo With(Assignee owner, string name, string address)
		{
			OwnerAssigneeUid = owner.Uid;
			Name = name;
			Address = address;
			DateCreated = DateTime.Now;

			repository.Insert(this);

			return this;
		}

		public Assignee Owner { get { return ctx.Query<Assignees>().ByUid(OwnerAssigneeUid); } }

		private void SendDefaultEmail()
		{
			Console.WriteLine(string.Format("sent an email to {0} via phone using address: {1}", Owner.Name, Address));
		}

		#region IContactInfo implementation
		string IContactInfo.Name {get{return Name;}}
		string IContactInfo.Info {get{return (string)Address;}}
		DateTime IContactInfo.DateCreated{get{return DateCreated;}}

		void IContactInfo.Poke(){SendDefaultEmail();}
		#endregion
	}

	public class EmailContactInfos : Query<EmailContactInfo>
	{
		public EmailContactInfos(IDomainContext context) : base(context){}

		public List<EmailContactInfo> ByOwnerAssigneeUid(Guid ownerAssigneeUid)
		{
			return By(o => o.OwnerAssigneeUid == ownerAssigneeUid);
		}
	}
}
