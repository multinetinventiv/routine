using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Core.Configuration;

namespace Routine.Engine.Virtual
{
	public class ProxyMethod : IMethod
	{
		private readonly IMethod real;
		private readonly IType parentType;
		private readonly int parameterOffset;
		private readonly List<IParameter> parameters;
		private readonly Func<object, object[], object> targetDelegate;

		public SingleConfiguration<ProxyMethod, string> Name { get; }

		public ProxyMethod(IType parentType, IMethod real, params IParameter[] parameters) : this(parentType, real, (o, p) => o, parameters.AsEnumerable()) { }
		public ProxyMethod(IType parentType, IMethod real, Func<object, object[], object> targetDelegate, params IParameter[] parameters) : this(parentType, real, targetDelegate, parameters.AsEnumerable()) { }
		public ProxyMethod(IType parentType, IMethod real, Func<object, object[], object> targetDelegate, IEnumerable<IParameter> parameters)
		{
			if (parentType == null) { throw new ArgumentNullException("parentType"); }
			if (real == null) { throw new ArgumentNullException("real"); }
			if (targetDelegate == null) { throw new ArgumentNullException("targetDelegate"); }
			if (parameters == null) { throw new ArgumentNullException("parameters"); }

			Name = new SingleConfiguration<ProxyMethod, string>(this, "Name", true);

			this.real = real;
			this.parentType = parentType;
			this.targetDelegate = targetDelegate;
			
			this.parameters = parameters.Select((p, i) => new ProxyParameter(p, this, i) as IParameter).ToList();

			parameterOffset = this.parameters.Count;

			this.parameters.AddRange(real.Parameters.Select((p, i) => new ProxyParameter(p, this, parameterOffset + i) as IParameter));

			Name.Set(real.Name);
		}

		private object PerformOn(object target, object[] parameters)
		{
			return real.PerformOn(targetDelegate(target, parameters), parameters.Skip(parameterOffset).ToArray());
		}

		#region ITypeComponent implementation

		IType ITypeComponent.ParentType => parentType;
        string ITypeComponent.Name => Name.Get();
        object[] ITypeComponent.GetCustomAttributes() { return real.GetCustomAttributes(); }

		#endregion

		#region IParametric implementation

		List<IParameter> IParametric.Parameters => parameters;

        #endregion

		#region IReturnable implementation

		IType IReturnable.ReturnType => real.ReturnType;
        object[] IReturnable.GetReturnTypeCustomAttributes() { return real.GetReturnTypeCustomAttributes(); }

		#endregion

		#region IMethod

		object IMethod.PerformOn(object target, params object[] parameters) { return PerformOn(target, parameters); }
		bool IMethod.IsPublic => real.IsPublic;
        IType IMethod.GetDeclaringType(bool firstDeclaringType) { return parentType; }

		#endregion
	}
}