using System;
using System.Collections;

namespace Routine.Engine.Reflection
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

			elementType = Get(type.GetElementType());
		}

		protected override TypeInfo GetElementType()
		{
			return elementType;
		}

		public override object CreateInstance()
		{
			return CreateListInstance(0);
		}

		public override IList CreateListInstance(int length)
		{
			return Array.CreateInstance(elementType.GetActualType(), length);
		}
	}
}
