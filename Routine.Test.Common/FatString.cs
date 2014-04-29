using System;
namespace Routine.Test.Common
{
	[Serializable]
	public class FatString : IEquatable<FatString>
	{
		public static FatString Parse(string value)
		{
			return new FatString(value);
		}

		private readonly string value;

		public FatString(string value)
		{
			this.value = value;
		}

		public string Value { get { return ToString(); } }

		public override string ToString()
		{
			return value;
		}

		public static implicit operator FatString(string normalString) { return new FatString(normalString); }
		public static explicit operator string(FatString fatString) { return fatString.value; }

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(FatString))
				return false;

			return Equals((FatString)obj);
		}

		public bool Equals(FatString other)
		{
			if (other == null)
				return false;

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

