using System;
using System.Collections.Generic;
using System.Linq;

namespace Routine.Engine.Reflection
{
	internal class EnumTypeInfo : PreloadedTypeInfo
	{
		private readonly List<string> enumNames;
		private readonly List<object> enumValues;
		private readonly TypeInfo enumUnderlyingType;

		internal EnumTypeInfo(Type type)
			: base(type)
		{
			IsEnum = true;

			enumNames = type.GetEnumNames().ToList();
			enumValues = type.GetEnumValues().Cast<object>().ToList();
			enumUnderlyingType = Get(type.GetEnumUnderlyingType());
		}

		public override List<string> GetEnumNames() { return enumNames; }
		public override List<object> GetEnumValues() { return enumValues; }
		protected override TypeInfo GetEnumUnderlyingType() { return enumUnderlyingType; }
	}
}
