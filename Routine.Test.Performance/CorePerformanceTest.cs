using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Fasterflect;
using NUnit.Framework;
using Routine.Api;
using Routine.Core;
using Routine.Core.Cache;
using Routine.Test.Performance.Domain;

namespace Routine.Test.Performance.Domain
{
	public class BusinessPerformance
	{
		public int Id{get;set;}
		public List<BusinessPerformanceSub> Items{ get; set;}
		public BusinessPerformance() {Items = new List<BusinessPerformanceSub>();}

		public BusinessPerformanceSub GetSub(int index){return Items[index];}
	}

	public class BusinessPerformanceSub
	{
		public int Id{ get; set;}
		public string Prop1{get;set;}
		public string Prop2{get;set;}
		public string Prop3{get;set;}
		public string Prop4{get;set;}
		public string Prop5{get;set;}
		public string Prop6{get;set;}
		public string Prop7{get;set;}
		public string Prop8{get;set;}
		public string Prop9{get;set;}
		public string Prop10{get;set;}
	}
}

namespace Routine.Test.Performance
{
	[TestFixture]
	public class CorePerformanceTest
	{
		private Dictionary<string, object> objectRepository;

		private ICodingStyle codingStyle;
		private IObjectService objectService;
		private Rapplication rapp;

		[SetUp]
		public void SetUp()
		{
			objectRepository = new Dictionary<string, object>();
			
			var apiCtx = BuildRoutine.Context()
				.UsingCache(new DictionaryCache())
				.AsClientApplication(
					codingStyle = BuildRoutine.CodingStyle()
					.FromBasic()				
					.DomainTypeRootNamespacesAre("Routine.Test.Performance.Domain")
					.Use(p => p.NullPattern("_null"))
					.Use(p => p.ShortModelIdPattern("System", "s"))
					.Use(p => p.ParseableValueTypePattern())
					.SerializeModelId.Done(s => s.SerializeBy(t => t.FullName).DeserializeBy(id => id.ToType()))
						
					.SelectMembers.Done(s => s.ByPublicProperties(p => p.IsOnReflected() && !p.IsIndexer).When(t => t.IsDomainType))
					.SelectOperations.Done(s => s.ByPublicMethods(m => m.IsOnReflected()).When(t => t.IsDomainType))
						
					.ExtractId.Done(e => e.ByProperty(p => p.Returns<int>("Id")).ReturnAsString())
					.Locate.Done(l => l.ByConverting(id => objectRepository[id]).WhenId(id => objectRepository.ContainsKey(id)))
						
					.ExtractDisplayValue.Done(e => e.ByConverting(o => string.Format("{0}", o)))
				);
				
			objectService = apiCtx.ObjectService;
			rapp = apiCtx.Rapplication;
		}

		#region Helpers

		protected void AddToRepository(BusinessPerformance obj)
		{
			objectRepository.Add(codingStyle.IdExtractor.Extract(obj), obj);
			foreach(var sub in obj.Items)
			{
				objectRepository.Add(codingStyle.IdExtractor.Extract(sub), sub);
			}
		}

		private static BusinessPerformance NewObj(int id, int subObjectCount)
		{
			var result = new BusinessPerformance();

			result.Id = id;

			for(int i = 0; i < subObjectCount; i++)
			{
				result.Items.Add(new BusinessPerformanceSub {
					Id = id*(subObjectCount + 1) + i,
					Prop1 = i + "_prop1",
					Prop2 = i + "_prop2",
					Prop3 = i + "_prop3",
					Prop4 = i + "_prop4",
					Prop5 = i + "_prop5",
					Prop6 = i + "_prop6",
					Prop7 = i + "_prop7",
					Prop8 = i + "_prop8",
					Prop9 = i + "_prop9",
					Prop10 = i + "_prop10",
				});
			}

			return result;
		}

		private void Run(string name, Action testAction)
		{
			Run(name, testAction, 1);
		}

		private void Run(string name, Action testAction, int count)
		{
			//first call is not included to let it do its inital loading & caching etc.
			testAction();

			var timer = Stopwatch.StartNew();
			for(int i = 0; i < count; i++)
			{
				testAction();
			}
			timer.Stop();

			Console.WriteLine(name + " -> " + timer.Elapsed.TotalMilliseconds);
		}

		#endregion


		[Test]
		public void ReflectionCore_PropertyAccess()
		{
			const int load = 100000;

			Console.WriteLine ("Load -> " + load);

			Console.WriteLine("-------");
			var obj = new BusinessPerformance{ Id = 1};

			Run("Direct Access", () =>
				{
					var name = obj.Id;
				}, load);

			Console.WriteLine("-------");

			var accessor = obj.GetType().Property("Id").DelegateForGetPropertyValue();
			Run("Fasterflect Cached Access", () =>
				{
					var name = accessor(obj);
				}, load);

			var rprop = obj.GetTypeInfo().GetProperty("Id");
			Run("Routine.Core.Reflection Cached Access", () =>
			{
				var name = rprop.GetValue(obj);
			}, load);
			
			var prop = obj.GetType().GetProperty("Id");
			Run("System.Reflection Cached Access", () =>
				{
					var name = prop.GetValue(obj, new object[0]);
				}, load);
			Console.WriteLine("-------");

			Run("Routine.Core.Reflection Access", () =>
				{
					var name = obj.GetTypeInfo().GetProperty("Id").GetValue(obj);
				}, load);
			Run("Fasterflect Access", () =>
				{
					var name = obj.GetType().Property("Id").Get(obj);
				}, load);
			Run("System.Reflection Access", () =>
				{
					var name = obj.GetType().GetProperty("Id").GetValue(obj, new object[0]);
				}, load);

			Console.WriteLine("-------");

			Run("Routine.Core.Reflection Access -> GetTypeInfo()", () =>
				{
					var name = obj.GetTypeInfo();
				}, load);
			var type = obj.GetTypeInfo();
			Run("Routine.Core.Reflection Access -> GetProperty('Id')", () =>
				{
					var name = type.GetProperty("Id");
				}, load);
			rprop = type.GetProperty("Id");
			Run("Routine.Core.Reflection Access -> GetValue(obj)", () =>
				{
					var name = rprop.GetValue(obj);
				}, load);
		}

		[Test]
		public void GetObjectData_WithListProperty()
		{
			const int load = 100;
			const int sub_obj_count = 200;

			Console.WriteLine ("Load -> " + load + "x" + sub_obj_count);
			Console.WriteLine ("------");

			const int obj_id = 1;

			var obj = NewObj(obj_id, sub_obj_count);

			var obj_type = typeof(BusinessPerformance).FullName;
			var sub_type = typeof(BusinessPerformanceSub).FullName;

			AddToRepository(obj);
			var dummyLoadToPrintMessagesBeforeManuelCoding = objectService.Get(new ObjectReferenceData {
				ActualModelId = obj_type,
				ViewModelId = obj_type,
				Id = obj_id.ToString()
			});

			Run("Manuel Coding", () =>
				{
					var ord = new ObjectReferenceData {
						ActualModelId = obj_type,
						ViewModelId = obj_type,
						Id = obj_id.ToString()
					};

					var foundObj = objectRepository[ord.Id] as BusinessPerformance;
					var result = new ObjectData {
						Reference = new ObjectReferenceData {
							ActualModelId = ord.ActualModelId,
							ViewModelId = ord.ViewModelId,
							Id = foundObj.Id.ToString(),
						},
						Members = new Dictionary<string, ValueData>{
							{"Items", new ValueData {
									IsList = true,
									Values = foundObj.Items.Select(sub => new ObjectData{
										Reference = new ObjectReferenceData {
											ActualModelId = sub_type,
											ViewModelId = sub_type,
											Id = sub.Id.ToString(),
										},
										Value = sub.ToString()
									}).ToList()
								}
							}
						}
					};
				}, load);

			Run("Routine ObjectService Access", () =>
				{			
					var result = objectService.Get(new ObjectReferenceData {
						ActualModelId = obj_type,
						ViewModelId = obj_type,
						Id = obj_id.ToString()
					});
				}, load);

			Run("Routine Client Api Access", () =>
				{
				var items = rapp.Get(obj_id.ToString(), obj_type)["Items"].GetValue().List.Select(r => r.Value);
				}, load);
		}

		[Test]
		public void PerformOperation_WithParseableParameter()
		{
			const int load = 1000;

			Console.WriteLine ("Load -> " + load);
			Console.WriteLine ("------");

			const int obj_id = 1;

			var obj_type = typeof(BusinessPerformance).FullName;
			var sub_type = typeof(BusinessPerformanceSub).FullName;

			var obj = NewObj(obj_id, 1);

			AddToRepository(obj);

			Run("Manuel Coding", () =>
			{
				var ord = new ObjectReferenceData {
					ActualModelId = obj_type,
					ViewModelId = obj_type,
					Id = obj_id.ToString()
				};
				BusinessPerformance foundObj = objectRepository[ord.Id] as BusinessPerformance;

				var result = foundObj.GetSub(int.Parse("0"));
				var returnResult = new ValueData {
					IsList = false,
					Values = new List<ObjectData> {
						new ObjectData {
							Value = result.ToString(),
							Reference = new ObjectReferenceData {
								ActualModelId = sub_type,
								ViewModelId = sub_type,
								Id = result.Id.ToString()
							}
						}
					}
				};
			}, load);

			Run("Routine ObjectService Access", () =>
			{
				var ord = new ObjectReferenceData {
					ActualModelId = obj_type,
					ViewModelId = obj_type,
					Id = obj_id.ToString()
				};
				var returnResult = objectService.PerformOperation(ord, "GetSub", new Dictionary<string, ReferenceData>{
					{"index", new ReferenceData {
							IsList = false,
							References = new List<ObjectReferenceData> {
								new ObjectReferenceData {
									ActualModelId= "s-int32",
									Id = "0"
								}
							}
						}
					}
				});
			}, load);

			Run("Routine Client Api Access", () =>
			{
				var rvar = rapp.NewVar("index", rapp.Get("0", "s-int32"));
				var returnResult = rapp.Get(obj_id.ToString(), obj_type).Perform("GetSub", rvar);
			}, load);
		}
	}
}

