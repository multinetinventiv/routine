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
				.UsingInterception(BuildRoutine.InterceptionConfig()
					.FromBasic()
					.InterceptPerformOperation.Done(i => i.Do()
						.Before(ctx => Console.WriteLine("before - " + ctx.OperationModelId))
						.Success(ctx => Console.WriteLine("success - " + ctx.Result))
						.Fail(ctx => Console.WriteLine("fail - " + ctx.Exception))
						.After(ctx => Console.WriteLine("after - " + ctx.OperationModelId))
						.When(ctx => ctx.OperationModelId != "TestMaxLength")))
				.AsSoaClient(BuildRoutine.SoaClientConfig()
					.FromBasic()
					.ServiceUrlBaseIs("http://127.0.0.1:5485/Soa")
					.DefaultParametersAre("language_code")
					.ExtractDefaultParameterValue.Done(e => e.Always("tr-TR").When("language_code"))
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

			var languageCodeFromServer = projectManagementModule.Perform("TestLanguageCode");
			Console.WriteLine(languageCodeFromServer.Object.Id);

			Console.WriteLine("-----------");

			Console.WriteLine("Performing singleton operation (Customers.All)");
			var customers = rapp.Get("Instance", "m-project-management--customers").Perform("All");

			Console.WriteLine("Sending data input... (ProjectManagementModule.CreateProjects)");
			var bulkProjects = projectManagementModule.Perform("CreateProjects",
				rapp.NewVar<DateTime>("defaultDeadline", DateTime.Now.AddDays(7), "s-date-time"),
				rapp.NewVarList("projects",
					rapp.Init("m-project-management--new-project",
						rapp.NewVar("customer", customers.List[0]),
						rapp.NewVar<DateTime>("deadline", DateTime.Now.AddDays(14), "s-date-time"),
						rapp.NewVar<string>("name", "project 1", "s-string"),
						rapp.NewVarList("features", 
							rapp.Init("m-project-management--new-feature",
								rapp.NewVar<string>("name", "project 1 - feature 1", "s-string"),
								rapp.NewVar<bool>("someBool", true, "s-boolean")
							),
							rapp.Init("m-project-management--new-feature",
								rapp.NewVar<string>("name", "project 1 - feature 2", "s-string"),
								rapp.NewVar<bool>("someBool", false, "s-boolean")
							)
						)
					),
					rapp.Init("m-project-management--new-project",
						rapp.NewVar("customer", customers.List[0]),
						rapp.NewVar<DateTime>("deadline", DateTime.Now.AddDays(21), "s-date-time"),
						rapp.NewVar<string>("name", "project 2", "s-string"),
						rapp.NewVarList("features",
							rapp.Init("m-project-management--new-feature",
								rapp.NewVar<string>("name", "project 2 - feature 1", "s-string"),
								rapp.NewVar<bool>("someBool", false, "s-boolean")
							)
						)
					)
				)
			);
			Console.WriteLine("bulk projects created;");
			foreach (var bulkProject in bulkProjects.List)
			{
				Console.WriteLine("\tbulk project: {0} - {1}", bulkProject.Id, bulkProject.Value);
			}
			Console.WriteLine("------------");

			Console.WriteLine("testing max length;");
			var maxLengthResult = projectManagementModule.Perform("TestMaxLength", rapp.NewVar("count", 1000, "s-int-32"));
			Console.WriteLine("Total {0} items fetched successfully", maxLengthResult.List.Count);
			Console.WriteLine("------------");

		}

		[Test]
		public void ManuelTestField()
		{
			Console.WriteLine(typeof(IList<string>).GetMethod("IndexOf").ContainsGenericParameters);
			Console.WriteLine(typeof(IList<>).GetMethod("IndexOf").ContainsGenericParameters);
		}
	}
}

