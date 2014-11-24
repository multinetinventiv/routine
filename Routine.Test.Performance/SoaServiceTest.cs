using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web.Script.Serialization;
using NUnit.Framework;
using Routine.Client;
using Routine.Core.Rest;

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
					.ServiceInterceptors.Add(c => c.Interceptor(i => i.Do()
						.Before(ctx => Console.WriteLine("before - " + ctx.OperationModelId))
						.Success(ctx => Console.WriteLine("success - " + ctx.Result))
						.Fail(ctx => Console.WriteLine("fail - " + ctx.Exception))
						.After(ctx => Console.WriteLine("after - " + ctx.OperationModelId))
						.When(ctx => ctx.OperationModelId != "TestMaxLength" && ctx.OperationModelId != "TestBigInput"))))
				.UsingSerializer(new JsonRestSerializer(new JavaScriptSerializer { MaxJsonLength = int.MaxValue }))
				.UsingRestClient(request => request.Timeout = Timeout.Infinite)
				.AsSoaClient(BuildRoutine.SoaClientConfig()
					.FromBasic()
					.ServiceUrlBase.Set("http://127.0.0.1:5485/Soa")
					.Headers.Add("language_code")
					.HeaderValue.Set("tr-TR", "language_code")
					.Use(p => p.FormattedExceptionPattern("{1} occured with message: {0}, handled:{2}"))
				);


			rapp = soaClientContext.Application;
		}

		[Test]
		[Ignore]
		public void ServiceClientTest()
		{
			var todoModule = rapp.Get("Instance", "m-todo--todo-module");

			Console.WriteLine("Id: " + todoModule.Id);
			Console.WriteLine("Value: " + todoModule.Value);
			Console.WriteLine("Members:");
			foreach (var memberValue in todoModule.MemberValues)
			{
				Console.WriteLine("\t" + memberValue.Member.Id + (memberValue.Member.IsList ? " (List)" : ""));
				var value = memberValue.Get();
				foreach (var item in value.List)
				{
					Console.WriteLine("\t\tId: " + item.Id);
					Console.WriteLine("\t\tValue: " + item.Value);
				}
			}
			var instances = rapp["m-todo--assignees"].StaticInstances;
			Console.WriteLine("Available objects for m-todo--assignees:");
			foreach (var instance in instances)
			{
				Console.WriteLine("\t" + instance.Id);
			}

			var assignees = instances[0];
			var testAssignee = assignees.Perform("SingleByName", rapp.NewVar("name", "test", "s-string"));

			Console.WriteLine("SingleByName(test):");

			Console.WriteLine("\tId: " + testAssignee.Object.Id);
			Console.WriteLine("\tValue: " + testAssignee.Object.Value);
			Console.WriteLine("\tMembers:");
			foreach (var memberValue in testAssignee.Object.MemberValues)
			{
				Console.WriteLine("\t\t" + memberValue.Member.Id + (memberValue.Member.IsList ? " (List)" : ""));
				var value = memberValue.Get();
				foreach (var item in value.List)
				{
					Console.WriteLine("\t\t\tId: " + item.Id);
					Console.WriteLine("\t\t\tValue: " + item.Value);
				}
			}

			Console.WriteLine("\tUpdating object...");
			var updateResult = testAssignee.Object.Perform("Update", rapp.NewVar("name", "test", "s-string"));
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
				rapp.NewVar("defaultDeadline", DateTime.Now.AddDays(7), "s-date-time"),
				rapp.NewVarList("projects",
					rapp.Init("m-project-management--new-project",
						rapp.NewVar("customer", customers.List[0]),
						rapp.NewVar("deadline", DateTime.Now.AddDays(14), "s-date-time"),
						rapp.NewVar("name", "project 1", "s-string"),
						rapp.NewVarList("features",
							rapp.Init("m-project-management--new-feature",
								rapp.NewVar("name", "project 1 - feature 1", "s-string"),
								rapp.NewVar("someBool", true, "s-boolean")
							),
							rapp.Init("m-project-management--new-feature",
								rapp.NewVar("name", "project 1 - feature 2", "s-string"),
								rapp.NewVar("someBool", false, "s-boolean")
							)
						)
					),
					rapp.Init("m-project-management--new-project",
						rapp.NewVar("customer", customers.List[0]),
						rapp.NewVar("deadline", DateTime.Now.AddDays(21), "s-date-time"),
						rapp.NewVar("name", "project 2", "s-string"),
						rapp.NewVarList("features",
							rapp.Init("m-project-management--new-feature",
								rapp.NewVar("name", "project 2 - feature 1", "s-string"),
								rapp.NewVar("someBool", false, "s-boolean")
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
			var stopwatch = Stopwatch.StartNew();
			var maxLengthResult = projectManagementModule.Perform("TestMaxLength", rapp.NewVar("count", 1000, "s-int-32"));
			stopwatch.Stop();
			Console.WriteLine("Total {0} items fetched successfully in {1:c}", maxLengthResult.List.Count, stopwatch.Elapsed);
			Console.WriteLine("------------");

			Console.WriteLine("Sending big data input... (ProjectManagementModule.TestBigInput)");
			const int bulk_count = 20000;
			stopwatch = Stopwatch.StartNew();
			var bulkProjectCount = projectManagementModule.Perform("TestBigInput",
				rapp.NewVarList("projects",
					Enumerable.Range(0, bulk_count).Select(i =>
						rapp.Init("m-project-management--new-project",
							rapp.NewVar("customer", customers.List[0]),
							rapp.NewVar("deadline", DateTime.Now.AddDays(21), "s-date-time"),
							rapp.NewVar("name", "project " + i, "s-string"),
							rapp.NewVarList("features",
								Enumerable.Range(0, 3).Select(j =>
									rapp.Init("m-project-management--new-feature",
										rapp.NewVar("name", "project " + i + " - feature " + j, "s-string"),
										rapp.NewVar("someBool", false, "s-boolean")
										)
									)
								)
							)
						)
					)
				);
			stopwatch.Stop();
			Console.WriteLine("total {0} newproject input sent in {1:c}", bulkProjectCount, stopwatch.Elapsed);
			Console.WriteLine("------------");

		}

		[Test]
		[Ignore]
		public void ManuelTestField()
		{
			Console.WriteLine(typeof(IList<string>).GetMethod("IndexOf").ContainsGenericParameters);
			Console.WriteLine(typeof(IList<>).GetMethod("IndexOf").ContainsGenericParameters);
		}
	}
}

