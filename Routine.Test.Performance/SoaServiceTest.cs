using System;
using NUnit.Framework;
using Routine.Api;

namespace Routine.Test.Performance
{
	[TestFixture]
	public class SoaServiceTest
	{
		private IApiContext soaClientContext;
		private Rapplication rapp;

		[SetUp]
		public void SetUp()
		{			
			soaClientContext = BuildRoutine.Context().AsSoaClient(BuildRoutine.SoaClientConfig().FromBasic().ServiceUrlBaseIs("http://127.0.0.1:2222/Soa"));

			rapp = soaClientContext.Rapplication;
		}

		[Test][Ignore]
		public void ServiceClientTest()
		{
			var result = rapp.Get("Instance", "Todo-TodoModule");

			Console.WriteLine("Id: " + result.Id);
			Console.WriteLine("Value: " + result.Value);
			Console.WriteLine("Members:");
			foreach(var member in result.Members)
			{
				Console.WriteLine("\t" + member.Id + (member.IsList?" (List)":""));
				var value = member.GetValue();
				foreach(var item in value.List)
				{
					Console.WriteLine("\t\tId: " + item.Id);
					Console.WriteLine("\t\tValue: " + item.Value);
				}
			}
		}
	}
}

