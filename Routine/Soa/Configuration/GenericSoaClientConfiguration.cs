namespace Routine.Soa.Configuration
{
	public class GenericSoaClientConfiguration : ISoaClientConfiguration
	{
		private string ServiceUrlBase{get;set;}
		public GenericSoaClientConfiguration ServiceUrlBaseIs(string serviceUrlBase){ServiceUrlBase = serviceUrlBase; return this;}

		public GenericSoaClientConfiguration()
		{
		}

		public GenericSoaClientConfiguration Merge(GenericSoaClientConfiguration other)
		{
			return this;
		}

		#region ISoaClientConfiguration implementation
		string ISoaClientConfiguration.ServiceUrlBase {get{return ServiceUrlBase;}}
		#endregion
	}
}

