using System;

namespace Routine.Core.Reflection
{
	internal class ArrayTypeInfo : PreloadedTypeInfo
	{
		private TypeInfo elementType;

		internal ArrayTypeInfo(Type type) : base(type)
		{
			IsArray = true;
		}

		protected override void Load()
		{
			base.Load();

			elementType = TypeInfo.Get(type.GetElementType());
		}

		protected override TypeInfo GetElementType()
		{
			return elementType;
		}
	}
}
