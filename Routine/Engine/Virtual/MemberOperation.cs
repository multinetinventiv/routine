using System;
using System.Collections.Generic;

namespace Routine.Engine.Virtual
{
	public class MemberOperation : IOperation
	{
		private readonly IMember member;
		private readonly string namePrefix;

		public MemberOperation(IMember member) : this(member, Constants.PROPERTY_OPERATION_DEFAULT_PREFIX) { }
		public MemberOperation(IMember member, string namePrefix)
		{
			if (namePrefix == null) { throw new ArgumentNullException("namePrefix"); }

			this.member = member;
			this.namePrefix = namePrefix;
		}

		public string Name { get { return member.Name.Prepend(namePrefix); } }
		public object[] GetCustomAttributes() { return member.GetCustomAttributes(); }

		public IType ParentType { get { return member.ParentType; } }
		public IType ReturnType { get { return member.ReturnType; } }
		public object[] GetReturnTypeCustomAttributes() { return member.GetReturnTypeCustomAttributes(); }

		public List<IParameter> Parameters { get { return new List<IParameter>(); } }
		public bool IsPublic { get { return member.IsPublic; } }
		public IType GetDeclaringType(bool firstDeclaringType) { return member.GetDeclaringType(firstDeclaringType); }
		public object PerformOn(object target, params object[] parameters) { return member.FetchFrom(target); }
	}
}
