using System;

namespace Routine.Engine.Virtual
{
	public class OperationMember : IMember
	{
		private readonly object[] parameters;
		private readonly IOperation operation;
		private readonly string ignorePrefix;

		public OperationMember(IOperation operation, params object[] parameters) : this(operation, string.Empty, parameters) { }
		public OperationMember(IOperation operation, string ignorePrefix, params object[] parameters)
		{
			if (ignorePrefix == null) { throw new ArgumentNullException("ignorePrefix"); }
			if (operation.Parameters.Count != parameters.Length) { throw new ArgumentException("Given parameters and operation parameters do not match"); }
			if (operation.ReturnsVoid()) { throw new ArgumentException("Given operation must have a return type"); }

			this.operation = operation;
			this.ignorePrefix = ignorePrefix;
			this.parameters = parameters;
		}

		public string Name { get { return operation.Name.After(ignorePrefix); } }
		public object[] GetCustomAttributes() { return operation.GetCustomAttributes(); }
		public IType ParentType { get { return operation.ParentType; } }
		public IType ReturnType { get { return operation.ReturnType; } }
		public object[] GetReturnTypeCustomAttributes() { return operation.GetReturnTypeCustomAttributes(); }
		public bool IsPublic { get { return operation.IsPublic; } }
		public IType GetDeclaringType(bool firstDeclaringType) { return operation.GetDeclaringType(firstDeclaringType); }
		public object FetchFrom(object target) { return operation.PerformOn(target, parameters); }
	}
}
