using System;
using System.Collections.Generic;
using Routine.Core.Configuration;
using System.Threading.Tasks;

namespace Routine.Engine.Virtual
{
    public class VirtualMethod : IMethod
    {
        private readonly IType parentType;

        public SingleConfiguration<VirtualMethod, string> Name { get; }
        public SingleConfiguration<VirtualMethod, IType> ReturnType { get; }
        public SingleConfiguration<VirtualMethod, Func<object, object[], object>> Body { get; }
        public SingleConfiguration<VirtualMethod, Func<object, IType>> TypeRetrieveStrategy { get; }
        public ListConfiguration<VirtualMethod, IParameter> Parameters { get; }

        public VirtualMethod(IType parentType)
        {
            this.parentType = parentType;

            Name = new SingleConfiguration<VirtualMethod, string>(this, nameof(Name), true);
            ReturnType = new SingleConfiguration<VirtualMethod, IType>(this, nameof(ReturnType), true);
            Body = new SingleConfiguration<VirtualMethod, Func<object, object[], object>>(this, nameof(Body), true);
            TypeRetrieveStrategy = new SingleConfiguration<VirtualMethod, Func<object, IType>>(this, nameof(TypeRetrieveStrategy), true);
            Parameters = new ListConfiguration<VirtualMethod, IParameter>(this, nameof(Parameters));

            TypeRetrieveStrategy.Set(o => o is VirtualObject vo ? vo.Type : o.GetTypeInfo());
        }

        private object Perform(object target, object[] parameters)
        {
            parameters ??= Array.Empty<object>();

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
                throw new NullReferenceException($"Cannot perform {Name.Get()} method on null target");
            }

            if (!ValidateType(target, parentType))
            {
                throw new InvalidCastException(
                    $"Parent type of '{Name.Get()}' method is configured as {parentType}, but given target is of type {GetType(target)}");
            }
        }

        private void ValidateParameters(object[] parameters)
        {
            var iParameters = Parameters.Get();
            if (parameters.Length != iParameters.Count)
            {
                throw new InvalidOperationException(
                    $"'{Name.Get()}' has {iParameters.Count} parameters, but given parameter count is {parameters.Length}");
            }

            for (var i = 0; i < iParameters.Count; i++)
            {
                var iParameter = iParameters[i];

                if (parameters[i] == null && iParameter.ParameterType.IsValueType)
                {
                    throw new NullReferenceException(
                        $"The type of '{iParameter.Name}' parameter of '{Name.Get()}' method is configured as {iParameter.ParameterType}, " +
                        "but given argument is null. Null cannot be passed for value type parameters. Instead, default() should be used.");
                }

                if (!ValidateType(parameters[i], iParameter.ParameterType))
                {
                    throw new InvalidCastException(
                        $"The type of '{iParameter.Name}' parameter of '{Name.Get()}' method is configured as {iParameter.ParameterType}, " +
                        $"but given argument is of type {GetType(parameters[i])}");
                }
            }
        }

        private void ValidateResult(object result)
        {
            if (result == null && !ReturnType.Get().IsVoid && ReturnType.Get().IsValueType)
            {
                throw new NullReferenceException(
                    $"Return type of '{Name.Get()}' method is configured as {ReturnType.Get()}," +
                    " but perform result is null. Null cannot be returned for value type methods. Instead, default() should be used.");
            }

            if (!ValidateType(result, ReturnType.Get()))
            {
                throw new InvalidCastException(
                    $"Return type of '{Name.Get()}' method is configured as {ReturnType.Get()}, " +
                    $"but perform result is of type {GetType(result)}");
            }
        }

        private bool ValidateType(object @object, IType expected) => @object == null || GetType(@object).CanBe(expected);
        private IType GetType(object @object) => @object == null ? null : TypeRetrieveStrategy.Get()(@object);

        #region ITypeComponent implementation

        object[] ITypeComponent.GetCustomAttributes() => Array.Empty<object>();

        IType ITypeComponent.ParentType => parentType;
        string ITypeComponent.Name => Name.Get();

        #endregion

        #region IParametric implementation

        List<IParameter> IParametric.Parameters => Parameters.Get();

        #endregion

        #region IReturnable implementation

        object[] IReturnable.GetReturnTypeCustomAttributes() => Array.Empty<object>();

        IType IReturnable.ReturnType => ReturnType.Get();

        #endregion

        #region IMethod implementation

        bool IMethod.IsPublic => true;

        IType IMethod.GetDeclaringType(bool firstDeclaringType) => parentType;
        object IMethod.PerformOn(object target, params object[] parameters) => Perform(target, parameters);
        Task<object> IMethod.PerformOnAsync(object target, params object[] parameters) => Task.FromResult(Perform(target, parameters));

        #endregion
    }
}