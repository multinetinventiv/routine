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

		public ProxyMethod(IType parentType, IMethod real, params IParameter[] parameters) : this(parentType, real, (o, _) => o, parameters.AsEnumerable()) { }
		public ProxyMethod(IType parentType, IMethod real, Func<object, object[], object> targetDelegate, params IParameter[] parameters) : this(parentType, real, targetDelegate, parameters.AsEnumerable()) { }
		public ProxyMethod(IType parentType, IMethod real, Func<object, object[], object> targetDelegate, IEnumerable<IParameter> parameters)
		{
            if (parameters == null) { throw new ArgumentNullException(nameof(parameters)); }

			Name = new SingleConfiguration<ProxyMethod, string>(this, nameof(Name), true);

			this.real = real ?? throw new ArgumentNullException(nameof(real));
			this.parentType = parentType ?? throw new ArgumentNullException(nameof(parentType));
			this.targetDelegate = targetDelegate ?? throw new ArgumentNullException(nameof(targetDelegate));
			
			this.parameters = parameters.Select((p, i) => new ProxyParameter(p, this, i) as IParameter).ToList();

			parameterOffset = this.parameters.Count;

			this.parameters.AddRange(real.Parameters.Select((p, i) => new ProxyParameter(p, this, parameterOffset + i) as IParameter));

			Name.Set(real.Name);
		}

		private object PerformOn(object target, object[] parameters) => real.PerformOn(targetDelegate(target, parameters), parameters.Skip(parameterOffset).ToArray());

        #region ITypeComponent implementation

		IType ITypeComponent.ParentType => parentType;
        string ITypeComponent.Name => Name.Get();
        object[] ITypeComponent.GetCustomAttributes() => real.GetCustomAttributes();

        #endregion

		#region IParametric implementation

		List<IParameter> IParametric.Parameters => parameters;

        #endregion

		#region IReturnable implementation

		IType IReturnable.ReturnType => real.ReturnType;
        object[] IReturnable.GetReturnTypeCustomAttributes() => real.GetReturnTypeCustomAttributes();

        #endregion

		#region IMethod

		object IMethod.PerformOn(object target, params object[] parameters) => PerformOn(target, parameters);
        bool IMethod.IsPublic => real.IsPublic;
        IType IMethod.GetDeclaringType(bool firstDeclaringType) => parentType;

        #endregion
	}
}