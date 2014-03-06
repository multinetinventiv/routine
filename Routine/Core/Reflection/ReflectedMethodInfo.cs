using System.Linq;
using System;

namespace Routine.Core.Reflection
{
	public class ReflectedMethodInfo : MethodInfo
	{
		internal ReflectedMethodInfo(System.Reflection.MethodInfo methodInfo)
			: base(methodInfo){}

		protected override MethodInfo Load(){return this;}

		public override ParameterInfo[] GetParameters(){return methodInfo.GetParameters().Select(p => ParameterInfo.Reflected(p)).ToArray();}

		public override TypeInfo GetFirstDeclaringType()
		{
			return SearchFirstDeclaringType();
		}

		public override object Invoke(object target, params object[] parameters)
		{
			return methodInfo.Invoke(target, parameters);
		}

		public override object InvokeStatic(params object[] parameters)
		{
			return methodInfo.Invoke(null, parameters);
		}

		public override string Name{get{return methodInfo.Name;}}
		public override bool IsPublic{get{return methodInfo.IsPublic;}}

		public override TypeInfo DeclaringType{get{return TypeInfo.Get(methodInfo.DeclaringType);}}
		public override TypeInfo ReflectedType{get{return TypeInfo.Get(methodInfo.ReflectedType);}}
		public override TypeInfo ReturnType{get{return TypeInfo.Get(methodInfo.ReturnType);}}
	}
}

