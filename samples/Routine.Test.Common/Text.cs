using System;
namespace Routine.Test.Common
{
	[Serializable]
	public struct Text : IEquatable<Text>
	{
		public static Text Parse(string value)
		{
			return new Text(value);
		}

		private readonly string value;

		public Text(string value)
		{
			this.value = value;
		}

		public string Value { get { return ToString(); } }

		public override string ToString()
		{
			return value;
		}

		public static bool operator ==(Text l, Text r) { return object.Equals(l, r); }
		public static bool operator !=(Text l, Text r) { return !(l == r); }

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(Text))
				return false;

			return Equals((Text)obj);
		}

		public bool Equals(Text other)
		{
			return value == other.value;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return value.GetHashCode();
			}
		}
	}
}

