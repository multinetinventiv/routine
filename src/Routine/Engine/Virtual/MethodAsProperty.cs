using System;

namespace Routine.Engine.Virtual
{
	public class MethodAsProperty : IProperty
	{
		private readonly object[] parameters;
		private readonly IMethod method;
		private readonly string ignorePrefix;

		public MethodAsProperty(IMethod method, params object[] parameters) : this(method, string.Empty, parameters) { }
		public MethodAsProperty(IMethod method, string ignorePrefix, params object[] parameters)
		{
			if (ignorePrefix == null) { throw new ArgumentNullException("ignorePrefix"); }
			if (method.Parameters.Count != parameters.Length) { throw new ArgumentException("Given parameters and method parameters do not match"); }
			if (method.ReturnsVoid()) { throw new ArgumentException("Given method must have a return type"); }

			this.method = method;
			this.ignorePrefix = ignorePrefix;
			this.parameters = parameters;
		}

		public string Name { get { return method.Name.After(ignorePrefix); } }
		public object[] GetCustomAttributes() { return method.GetCustomAttributes(); }
		public IType ParentType { get { return method.ParentType; } }
		public IType ReturnType { get { return method.ReturnType; } }
		public object[] GetReturnTypeCustomAttributes() { return method.GetReturnTypeCustomAttributes(); }
		public bool IsPublic { get { return method.IsPublic; } }
		public IType GetDeclaringType(bool firstDeclaringType) { return method.GetDeclaringType(firstDeclaringType); }
		public object FetchFrom(object target) { return method.PerformOn(target, parameters); }
	}
}
