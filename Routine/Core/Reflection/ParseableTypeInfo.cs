using System;

namespace Routine.Core.Reflection
{
	internal class ParseableTypeInfo : PreloadedTypeInfo
	{
		private MethodInfo parseMethod;

		internal ParseableTypeInfo(Type type) 
			: base(type) {}

		protected override void Load()
		{
			base.Load();

			parseMethod = MethodInfo.Preloaded(type.GetMethod("Parse", new []{typeof(string)}));

			if(parseMethod.ReturnType != this)
			{
				throw new InvalidOperationException(type + " was loaded as Parseable but has no appropriate Parse method");
			}
		}

		protected override MethodInfo GetParseMethod()
		{
			return parseMethod;
		}
	}
}
