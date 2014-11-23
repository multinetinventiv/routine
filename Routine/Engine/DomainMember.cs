using System;
using Routine.Core;
using Routine.Core.Configuration;

namespace Routine.Engine
{
	public class DomainMember
	{
		private readonly ICoreContext ctx;
		private readonly IMember member;
		private readonly Lazy<DomainType> lazyMemberType;

		public string Id { get; private set; }
		public Marks Marks { get; private set; }
		public bool IsList { get; private set; }
		public bool FetchedEagerly { get; private set; }
		public DomainType MemberType { get { return lazyMemberType.Value; } }

		public DomainMember(ICoreContext ctx, IMember member)
		{
			this.ctx = ctx;
			this.member = member;
			
			Id = member.Name;
			Marks = new Marks(ctx.CodingStyle.GetMarks(member));
			IsList = member.ReturnType.CanBeCollection();
			FetchedEagerly = ctx.CodingStyle.IsFetchedEagerly(member);

			var returnType = IsList ? member.ReturnType.GetItemType() : member.ReturnType;

			try
			{
				ctx.CodingStyle.GetTypeId(returnType); //To eagerly check if type is configured
				lazyMemberType = new Lazy<DomainType>(() => ctx.GetDomainType(returnType));
			}
			catch (ConfigurationException ex)
			{
				throw new TypeNotConfiguredException(returnType, ex);
			}
		}

		public bool MarkedAs(string mark)
		{
			return Marks.Has(mark);
		}

		public MemberModel GetModel()
		{
			return new MemberModel
			{
				Id = Id,
				Marks = Marks.List,
				IsList = IsList,
				ViewModelId = MemberType.Id
			};
		}

		public ValueData CreateData(object target) { return CreateData(target, FetchedEagerly); }
		public ValueData CreateData(object target, bool eager)
		{
			return ctx.CreateValueData(member.FetchFrom(target), IsList, MemberType, eager);
		}
	}
}
