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
		public ValueData CreateData(object target, bool eager) { return CreateData(target, Constants.FIRST_DEPTH, eager); }
		internal ValueData CreateData(object target, int currentDepth) { return CreateData(target, currentDepth, FetchedEagerly); }
		internal ValueData CreateData(object target, int currentDepth, bool eager)
		{
			return ctx.CreateValueData(member.FetchFrom(target), IsList, MemberType, currentDepth, eager);
		}

		internal void LoadSubTypes()
		{
			//to force type to load
			var type = lazyMemberType.Value;
		}

		#region Formatting & Equality

		protected bool Equals(DomainMember other)
		{
			return string.Equals(Id, other.Id);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((DomainMember) obj);
		}

		public override int GetHashCode()
		{
			return (Id != null ? Id.GetHashCode() : 0);
		}

		public override string ToString()
		{
			return string.Format("Id: {0}", Id);
		}

		#endregion
	}
}
