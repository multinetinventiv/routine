using System;

namespace Routine.Test
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

		public string Value => ToString();

		public override string ToString()
		{
			return value;
		}

		public static bool operator ==(Text l, Text r) { return object.Equals(l, r); }
		public static bool operator !=(Text l, Text r) { return !(l == r); }

        public bool Equals(Text other)
        {
            return value == other.value;
        }

        public override bool Equals(object obj)
        {
            return obj is Text other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (value != null ? value.GetHashCode() : 0);
        }
    }
}