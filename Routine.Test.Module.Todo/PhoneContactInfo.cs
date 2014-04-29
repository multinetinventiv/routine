using System;
using System.Collections.Generic;
using Routine.Test.Domain;

namespace Routine.Test.Module.Todo
{
	public class PhoneContactInfo : IContactInfo
	{
		private readonly IRepository<PhoneContactInfo> repository;
		private readonly IDomainContext ctx;

		public Guid Uid{get; private set;}
		public Guid OwnerAssigneeUid  { get; private set; }
		public string Name{get;private set;}
		public string AreaCode  { get; private set; }
		public string Number  { get; private set; }
		public DateTime DateCreated  { get; private set; }

		private PhoneContactInfo() {}
		public PhoneContactInfo(IDomainContext ctx, IRepository<PhoneContactInfo> repository)
		{
			this.ctx = ctx;
			this.repository = repository;
		}

		internal PhoneContactInfo With(Assignee owner, string name, string areaCode, string number)
		{
			OwnerAssigneeUid = owner.Uid;
			Name = name;
			AreaCode = areaCode;
			Number = number;
			DateCreated = DateTime.Now;

			Uid = repository.Insert<Guid>(this);

			return this;
		}

		public Assignee Owner { get { return ctx.Query<Assignees>().ByUid(OwnerAssigneeUid); } }

		private void SendDefaultSms()
		{
			Console.WriteLine(string.Format("called {0} via phone using number: {1}", Owner.Name, (this as IContactInfo).Info));
		}

		#region IContactInfo implementation
		string IContactInfo.Name {get{return Name;}}
		string IContactInfo.Info {get{return "0 (" + AreaCode + ") " + Number;}}
		DateTime IContactInfo.DateCreated{get{return DateCreated;}}

		void IContactInfo.Poke(){SendDefaultSms();}
		#endregion
	}

	public class PhoneContactInfos : Query<PhoneContactInfo>
	{
		public PhoneContactInfos(IDomainContext context) : base(context){}

		public List<PhoneContactInfo> ByOwnerAssigneeUid(Guid ownerAssigneeUid)
		{
			return By(o => o.OwnerAssigneeUid == ownerAssigneeUid);
		}
	}
}
