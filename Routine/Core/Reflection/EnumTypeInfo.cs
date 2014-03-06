using System;

namespace Routine.Core.Reflection
{
	internal class EnumTypeInfo : PreloadedTypeInfo
	{
		internal EnumTypeInfo(Type type) : base(type)
		{
			IsEnum = true;
		}
	}
}
