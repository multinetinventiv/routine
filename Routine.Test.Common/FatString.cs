namespace Routine.Test.Common
{
	public class FatString
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

		public override string ToString()
		{
			return value;
		}
	}
}

