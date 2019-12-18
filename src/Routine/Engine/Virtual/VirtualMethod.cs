using System;
using System.Collections.Generic;
using Routine.Core.Configuration;

namespace Routine.Engine.Virtual
{
	public class VirtualMethod : IMethod
	{
		private readonly IType parentType;

		public SingleConfiguration<VirtualMethod, string> Name { get; private set; }
		public SingleConfiguration<VirtualMethod, IType> ReturnType { get; private set; }
		public SingleConfiguration<VirtualMethod, Func<object, object[], object>> Body { get; private set; }
		public SingleConfiguration<VirtualMethod, Func<object, IType>> TypeRetrieveStrategy { get; private set; }
		public ListConfiguration<VirtualMethod, IParameter> Parameters { get; private set; }

		public VirtualMethod(IType parentType)
		{
			this.parentType = parentType;

			Name = new SingleConfiguration<VirtualMethod, string>(this, "Name", true);
			ReturnType = new SingleConfiguration<VirtualMethod, IType>(this, "ReturnType", true);
			Body = new SingleConfiguration<VirtualMethod, Func<object, object[], object>>(this, "Body", true);
			TypeRetrieveStrategy = new SingleConfiguration<VirtualMethod, Func<object, IType>>(this, "TypeRetrieveStrategy", true);
			Parameters = new ListConfiguration<VirtualMethod, IParameter>(this, "Parameters");

			TypeRetrieveStrategy.Set(o =>
			{
				IType type;

				var vo = o as VirtualObject;
				if (vo != null)
				{
					type = vo.Type;
				}
				else
				{
					type = o.GetTypeInfo();
				}

				return type;
			});
		}

		private object Perform(object target, object[] parameters)
		{
			if (parameters == null) { parameters = new object[0]; }

			ValidateTarget(target);
			ValidateParameters(parameters);

			var result = Body.Get()(target, parameters);

			ValidateResult(result);

			return result;
		}

		private void ValidateTarget(object target)
		{
			if (target == null)
			{
				throw new NullReferenceException(string.Format("Cannot perform {0} method on null target", Name.Get()));
			}

			if (!ValidateType(target, parentType))
			{
				throw new InvalidCastException(
					string.Format("Parent type of '{0}' method is configured as {1}, but given target is of type {2}", Name.Get(),
						parentType, GetType(target)));
			}
		}

		private void ValidateParameters(object[] parameters)
		{
			var iParameters = Parameters.Get();
			if (parameters.Length != iParameters.Count)
			{
				throw new InvalidOperationException(string.Format("'{0}' has {1} parameters, but given parameter count is {2}",
					Name.Get(), iParameters.Count, parameters.Length));
			}

			for (int i = 0; i < iParameters.Count; i++)
			{
				var iParameter = iParameters[i];

				if (parameters[i] == null && iParameter.ParameterType.IsValueType)
				{
					throw new NullReferenceException(
						string.Format(
							"The type of '{0}' parameter of '{1}' method is configured as {2}, but given argument is null. Null cannot be passed for value type parameters. Instead, default() should be used.",
							iParameter.Name, Name.Get(), iParameter.ParameterType));
				}

				if (!ValidateType(parameters[i], iParameter.ParameterType))
				{
					throw new InvalidCastException(
						string.Format(
							"The type of '{0}' parameter of '{1}' method is configured as {2}, but given argument is of type {3}",
							iParameter.Name, Name.Get(), iParameter.ParameterType, GetType(parameters[i])));
				}
			}
		}

		private void ValidateResult(object result)
		{
			if (result == null && !ReturnType.Get().IsVoid && ReturnType.Get().IsValueType)
			{
				throw new NullReferenceException(
					string.Format(
						"Return type of '{0}' method is configured as {1}, but perform result is null. Null cannot be returned for value type methods. Instead, default() should be used.",
						Name.Get(), ReturnType.Get()));
			}

			if (!ValidateType(result, ReturnType.Get()))
			{
				throw new InvalidCastException(
					string.Format("Return type of '{0}' method is configured as {1}, but perform result is of type {2}", Name.Get(),
						ReturnType.Get(), GetType(result)));
			}
		}

		private bool ValidateType(object @object, IType expected)
		{
			if (@object == null) { return true; }

			return GetType(@object).CanBe(expected);
		}

		private IType GetType(object @object)
		{
			if (@object == null) { return null; }

			return TypeRetrieveStrategy.Get()(@object);
		}

		#region ITypeComponent implementation

		object[] ITypeComponent.GetCustomAttributes() { return new object[0]; }

		IType ITypeComponent.ParentType { get { return parentType; } }
		string ITypeComponent.Name { get { return Name.Get(); } }

		#endregion

		#region IParametric implementation

		List<IParameter> IParametric.Parameters { get { return Parameters.Get(); } }

		#endregion

		#region IReturnable implementation

		object[] IReturnable.GetReturnTypeCustomAttributes() { return new object[0]; }

		IType IReturnable.ReturnType { get { return ReturnType.Get(); } }

		#endregion

		#region IMethod implementation

		bool IMethod.IsPublic { get { return true; } }

		IType IMethod.GetDeclaringType(bool firstDeclaringType) { return parentType; }
		object IMethod.PerformOn(object target, params object[] parameters) { return Perform(target, parameters); }

		#endregion
	}
}