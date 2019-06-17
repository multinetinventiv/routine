using Routine.Core;

namespace Routine.Engine
{
	public class DomainData
	{
		private readonly ICoreContext ctx;
		private readonly IProperty property;

		public string Name { get; private set; }
		public Marks Marks { get; private set; }
		public bool IsList { get; private set; }
		public bool FetchedEagerly { get; private set; }
		public DomainType DataType { get; private set; }

		public DomainData(ICoreContext ctx, IProperty property)
		{
			this.ctx = ctx;
			this.property = property;

			Name = ctx.CodingStyle.GetName(property);
			Marks = new Marks(ctx.CodingStyle.GetMarks(property));
			IsList = property.ReturnType.CanBeCollection();
			FetchedEagerly = ctx.CodingStyle.IsFetchedEagerly(property);

			var returnType = IsList ? property.ReturnType.GetItemType() : property.ReturnType;

			if (!ctx.CodingStyle.ContainsType(returnType))
			{
				throw new TypeNotConfiguredException(returnType);
			}

			DataType = ctx.GetDomainType(returnType);
		}

		public bool MarkedAs(string mark)
		{
			return Marks.Has(mark);
		}

		public DataModel GetModel()
		{
			return new DataModel
			{
				Name = Name,
				Marks = Marks.List,
				IsList = IsList,
				ViewModelId = DataType.Id
			};
		}

		public VariableData CreateData(object target) { return CreateData(target, FetchedEagerly); }
		public VariableData CreateData(object target, bool eager) { return CreateData(target, Constants.FIRST_DEPTH, eager); }
		internal VariableData CreateData(object target, int currentDepth) { return CreateData(target, currentDepth, FetchedEagerly); }
		internal VariableData CreateData(object target, int currentDepth, bool eager)
		{
			return ctx.CreateValueData(property.FetchFrom(target), IsList, DataType, currentDepth, eager);
		}

		#region Formatting & Equality

		protected bool Equals(DomainData other)
		{
			return string.Equals(Name, other.Name);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((DomainData) obj);
		}

		public override int GetHashCode()
		{
			return (Name != null ? Name.GetHashCode() : 0);
		}

		public override string ToString()
		{
			return string.Format("{1} {0}", Name, DataType);
		}

		#endregion
	}
}
