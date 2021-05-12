using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Routine.Client;
using Routine.Core.Rest;

namespace Routine.Test.Performance
{
    [TestFixture]
    public class ServiceServiceTest
    {
        private IClientContext clientContext;
        private Rapplication rapp;

        [SetUp]
        public void SetUp()
        {
            var applicationBuilder = default(IApplicationBuilder);
            var httpContextAccessor = default(IHttpContextAccessor);
            clientContext = BuildRoutine.Context(applicationBuilder, httpContextAccessor)
                .UsingInterception(BuildRoutine.InterceptionConfig()
                    .FromBasic()
                    .ServiceInterceptors.Add(c => c.Interceptor(i => i.Do()
                        .Before(ctx => Console.WriteLine("before - " + ctx.OperationName))
                        .Success(ctx => Console.WriteLine("success - " + ctx.Result))
                        .Fail(ctx => Console.WriteLine("fail - " + ctx.Exception))
                        .After(ctx => Console.WriteLine("after - " + ctx.OperationName))
                        .When(ctx => ctx.OperationName != "TestMaxLength" && ctx.OperationName != "TestBigInput"))))
                .UsingJavaScriptSerializer(int.MaxValue)
                .UsingRestClient(request => request.Timeout = Timeout.Infinite)
                .AsServiceClient(BuildRoutine.ServiceClientConfig()
                    .FromBasic()
                    .ServiceUrlBase.Set("http://localhost:5485/Service")
                    .RequestHeaders.Add("language_code")
                    .RequestHeaderValue.Set("tr-TR", "language_code")
                    .ResponseHeaderProcessors.Add(r => r.For("code", "message").Do((code, message) => Console.WriteLine("response -> {0} - {1}", code, message)))
                    .Use(p => p.FormattedExceptionPattern("{0} occured with message: {1}, handled:{2}"))
                );

            rapp = clientContext.Application;
        }

        [Test]
        // [Ignore("")]
        public void ServiceClientTest()
        {
            var todoModule = rapp.Get("Instance", "Test.Todo.TodoModule");

            Console.WriteLine("Id: " + todoModule.Id);
            Console.WriteLine("Value: " + todoModule.Display);
            Console.WriteLine("Datas:");
            foreach (var dataValue in todoModule.DataValues)
            {
                Console.WriteLine("\t" + dataValue.Data.Name + (dataValue.Data.IsList ? " (List)" : ""));
                var value = dataValue.Get();
                foreach (var item in value.List)
                {
                    Console.WriteLine("\t\tId: " + item.Id);
                    Console.WriteLine("\t\tValue: " + item.Display);
                }
            }
            var instances = rapp["Test.Todo.Assignees"].StaticInstances;
            Console.WriteLine("Available objects for Test.Todo.Assignees:");
            foreach (var instance in instances)
            {
                Console.WriteLine("\t" + instance.Id);
            }

            var assignees = instances[0];
            var testAssignee = assignees.Perform("SingleByName", rapp.NewVar("name", "test", "System.String"));

            Console.WriteLine("SingleByName(test):");

            Console.WriteLine("\tId: " + testAssignee.Object.Id);
            Console.WriteLine("\tValue: " + testAssignee.Object.Display);
            Console.WriteLine("\tData:");
            foreach (var dataValue in testAssignee.Object.DataValues)
            {
                Console.WriteLine("\t\t" + dataValue.Data.Name + (dataValue.Data.IsList ? " (List)" : ""));
                var value = dataValue.Get();
                foreach (var item in value.List)
                {
                    Console.WriteLine("\t\t\tId: " + item.Id);
                    Console.WriteLine("\t\t\tValue: " + item.Display);
                }
            }

            Console.WriteLine("\tUpdating object...");
            var updateResult = testAssignee.Object.Perform("Update", rapp.NewVar("name", "test", "System.String"));
            Console.WriteLine("\tUpdate result is void: " + updateResult.IsVoid);

            Console.WriteLine("----------");
            var projectManagementModule = rapp.Get("Instance", "Test.ProjectManagement.ProjectManagementModule");
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
            var customers = rapp.Get("Instance", "Test.ProjectManagement.Customers").Perform("All");

            Console.WriteLine("Sending data input... (ProjectManagementModule.CreateProjects)");
            var bulkProjects = projectManagementModule.Perform("CreateProjects",
                rapp.NewVar("defaultDeadline", DateTime.Now.AddDays(7), "System.DateTime"),
                rapp.NewVarList("projects",
                    rapp.Init("Test.ProjectManagement.NewProject",
                        rapp.NewVar("customer", customers.List[0]),
                        rapp.NewVar("deadline", DateTime.Now.AddDays(14), "System.DateTime"),
                        rapp.NewVar("name", "project 1", "System.String"),
                        rapp.NewVarList("features",
                            rapp.Init("Test.ProjectManagement.NewFeature",
                                rapp.NewVar("name", "project 1 - feature 1", "System.String"),
                                rapp.NewVar("someBool", true, "System.Boolean")
                            ),
                            rapp.Init("Test.ProjectManagement.NewFeature",
                                rapp.NewVar("name", "project 1 - feature 2", "System.String"),
                                rapp.NewVar("someBool", false, "System.Boolean")
                            )
                        )
                    ),
                    rapp.Init("Test.ProjectManagement.NewProject",
                        rapp.NewVar("customer", customers.List[0]),
                        rapp.NewVar("deadline", DateTime.Now.AddDays(21), "System.DateTime"),
                        rapp.NewVar("name", "project 2", "System.String"),
                        rapp.NewVarList("features",
                            rapp.Init("Test.ProjectManagement.NewFeature",
                                rapp.NewVar("name", "project 2 - feature 1", "System.String"),
                                rapp.NewVar("someBool", false, "System.Boolean")
                            )
                        )
                    )
                )
            );
            Console.WriteLine("bulk projects created;");
            foreach (var bulkProject in bulkProjects.List)
            {
                Console.WriteLine("\tbulk project: {0} - {1}", bulkProject.Id, bulkProject.Display);
            }
            Console.WriteLine("------------");


            Console.WriteLine("testing max length;");
            var stopwatch = Stopwatch.StartNew();
            var maxLengthResult = projectManagementModule.Perform("TestMaxLength", rapp.NewVar("count", 1000, "System.Int32"));
            stopwatch.Stop();
            Console.WriteLine("Total {0} items fetched successfully in {1:c}", maxLengthResult.List.Count, stopwatch.Elapsed);
            Console.WriteLine("------------");

            Console.WriteLine("Sending big data input... (ProjectManagementModule.TestBigInput)");
            const int bulk_count = 20000;
            stopwatch = Stopwatch.StartNew();
            var bulkProjectCount = projectManagementModule.Perform("TestBigInput",
                rapp.NewVarList("projects",
                    Enumerable.Range(0, bulk_count).Select(i =>
                        rapp.Init("Test.ProjectManagement.NewProject",
                            rapp.NewVar("customer", customers.List[0]),
                            rapp.NewVar("deadline", DateTime.Now.AddDays(21), "System.DateTime"),
                            rapp.NewVar("name", "project " + i, "System.String"),
                            rapp.NewVarList("features",
                                Enumerable.Range(0, 3).Select(j =>
                                    rapp.Init("Test.ProjectManagement.NewFeature",
                                        rapp.NewVar("name", "project " + i + " - feature " + j, "System.String"),
                                        rapp.NewVar("someBool", false, "System.Boolean")
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
        [Ignore("")]
        public void ManuelTestField()
        {
            Console.WriteLine(Path.Combine("", "test/path"));
            Console.WriteLine(Path.Combine("/", "test/path"));
            Console.WriteLine(Path.Combine("root/", "test/path"));
            Console.WriteLine(Path.Combine("/root", "test/path"));
            Console.WriteLine(Path.Combine("/root/", "test/path"));
            Console.WriteLine(Path.Combine("root", "test/path"));
            Console.WriteLine(Path.Combine("root/", "/test/path"));
        }
    }
}

