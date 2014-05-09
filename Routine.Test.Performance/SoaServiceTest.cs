using System;
using System.Collections.Generic;
using NUnit.Framework;
using Routine.Core;
using Routine.Core.Api;

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
			soaClientContext = BuildRoutine.Context()
				.AsSoaClient(BuildRoutine.SoaClientConfig()
					.FromBasic()
					.ServiceUrlBaseIs("http://127.0.0.1:6848/Soa")
					.Use(p => p.FormattedExceptionPattern("{1} occured with message: {0}, handled:{2}"))
				);
				

			rapp = soaClientContext.Rapplication;
		}

		[Test]
		public void ServiceClientTest()
		{
			var todoModule = rapp.Get("Instance", "m-todo--todo-module");

			Console.WriteLine("Id: " + todoModule.Id);
			Console.WriteLine("Value: " + todoModule.Value);
			Console.WriteLine("Members:");
			foreach(var member in todoModule.Members)
			{
				Console.WriteLine("\t" + member.Id + (member.IsList?" (List)":""));
				var value = member.GetValue();
				foreach(var item in value.List)
				{
					Console.WriteLine("\t\tId: " + item.Id);
					Console.WriteLine("\t\tValue: " + item.Value);
				}
			}
			var availables = rapp.GetAvailableObjects("m-todo--assignees");
			Console.WriteLine("Available objects for m-todo--assignees:");
			foreach (var available in availables)
			{
				Console.WriteLine("\t" + available.Id);
			}

			var assignees = availables[0];
			var testAssignee = assignees.Perform("SingleByName", rapp.NewVar<string>("name", "test", "s-string"));

			Console.WriteLine("SingleByName(test):");

			Console.WriteLine("\tId: " + testAssignee.Object.Id);
			Console.WriteLine("\tValue: " + testAssignee.Object.Value);
			Console.WriteLine("\tMembers:");
			foreach (var member in testAssignee.Object.Members)
			{
				Console.WriteLine("\t\t" + member.Id + (member.IsList ? " (List)" : ""));
				var value = member.GetValue();
				foreach (var item in value.List)
				{
					Console.WriteLine("\t\t\tId: " + item.Id);
					Console.WriteLine("\t\t\tValue: " + item.Value);
				}
			}
			
			Console.WriteLine("\tUpdating object...");
			var updateResult = testAssignee.Object.Perform("Update", rapp.NewVar<string>("name", "test", "s-string"));
			Console.WriteLine("\tUpdate result is void: " + updateResult.IsVoid);

			Console.WriteLine("----------");
			var projectManagementModule = rapp.Get("Instance", "m-project-management--project-management-module");
			try
			{
				projectManagementModule.Perform("TestError");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		[Test]
		public void ManuelTestField()
		{
			Console.WriteLine(typeof(IList<string>).GetMethod("IndexOf").ContainsGenericParameters);
			Console.WriteLine(typeof(IList<>).GetMethod("IndexOf").ContainsGenericParameters);
		}
	}
}

