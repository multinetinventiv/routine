using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Fasterflect;
using NUnit.Framework;
using Routine.Client;
using Routine.Core;
using Routine.Core.Cache;
using Routine.Engine;
using Routine.Test.Performance.Domain;

#region Test Model

namespace Routine.Test.Performance.Domain
{
	public class BusinessPerformance
	{
		public int Id { get; set; }
		public List<BusinessPerformanceSub> Items { get; set; }
		public BusinessPerformance() { Items = new List<BusinessPerformanceSub>(); }

		public BusinessPerformanceSub GetSub(int index) { return Items[index]; }
		public int Create(List<BusinessPerformanceInput> input) { return 0; }
	}

	public class BusinessPerformanceSub
	{
		public int Id { get; set; }
		public string Prop1 { get; set; }
		public string Prop2 { get; set; }
		public string Prop3 { get; set; }
		public string Prop4 { get; set; }
		public string Prop5 { get; set; }
		public string Prop6 { get; set; }
		public string Prop7 { get; set; }
		public string Prop8 { get; set; }
		public string Prop9 { get; set; }
		public string Prop10 { get; set; }
	}

	public struct BusinessPerformanceInput
	{
		public string Str { get; private set; }
		public int Int { get; private set; }
		public string Str2 { get; private set; }
		public string Str3 { get; private set; }
		public string Str4 { get; private set; }

		//public BusinessPerformanceInput(string s, int i, string s2)
		//	: this()
		//{
		//	Str = s;
		//	Int = i;
		//	Str2 = s2;
		//}
		public BusinessPerformanceInput(string s, int i, string s2, string s3, string s4)
			: this()
		{
			Str = s;
			Int = i;
			Str2 = s2;
			Str3 = s3;
			Str4 = s4;
		}

		public bool Equals(BusinessPerformanceInput other)
		{
			return string.Equals(Str, other.Str) && Int == other.Int && string.Equals(Str2, other.Str2) && string.Equals(Str3, other.Str3) && string.Equals(Str4, other.Str4);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is BusinessPerformanceInput && Equals((BusinessPerformanceInput)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = (Str != null ? Str.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ Int;
				hashCode = (hashCode * 397) ^ (Str2 != null ? Str2.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Str3 != null ? Str3.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Str4 != null ? Str4.GetHashCode() : 0);
				return hashCode;
			}
		}
	}
}

#endregion

namespace Routine.Test.Performance
{
	[TestFixture]
	public class CorePerformanceTest
	{
		#region Setup & Helpers

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
						.AddTypes(GetType().Assembly, t => t.Namespace.StartsWith("Routine.Test.Performance.Domain"))

						.Use(p => p.ShortModelIdPattern("System", "s"))
						.Use(p => p.ParseableValueTypePattern())

						.TypeId.Set(c => c.By(t => t.FullName))
						.Initializers.Add(c => c.PublicInitializers().When(type.of<BusinessPerformanceInput>()))
						.Members.Add(c => c.Members(m => !m.IsInherited(true, true)).When(t => t.IsDomainType))
						.MemberFetchedEagerly.Set(true)
						.Operations.Add(c => c.Operations(o => !o.IsInherited(true, true)).When(t => t.IsDomainType))
						.IdExtractor.Set(c => c.IdByMember(m => m.Returns<int>("Id")))
						.Locator.Set(c => c.Locator(l => l.SingleBy(id => objectRepository[id])))
						.ValueExtractor.Set(c => c.Value(e => e.By(o => string.Format("{0}", o))))
				);

			objectService = apiCtx.ObjectService;
			rapp = apiCtx.Application;

			var applicationModel = objectService.GetApplicationModel();
		}

		protected void AddToRepository(BusinessPerformance obj)
		{
			objectRepository.Add(codingStyle.GetIdExtractor(obj.GetTypeInfo()).GetId(obj), obj);
			foreach (var sub in obj.Items)
			{
				objectRepository.Add(codingStyle.GetIdExtractor(sub.GetTypeInfo()).GetId(sub), sub);
			}
		}

		private static BusinessPerformance NewObj(int id, int subObjectCount)
		{
			var result = new BusinessPerformance();

			result.Id = id;

			for (int i = 0; i < subObjectCount; i++)
			{
				result.Items.Add(new BusinessPerformanceSub
				{
					Id = id * (subObjectCount + 1) + i,
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

		private double Run(string name, Action testAction)
		{
			return Run(name, testAction, 1);
		}

		private double Run(string name, Action testAction, int count)
		{
			//first call is not included to let it do its inital loading & caching etc.
			testAction();

			var timer = Stopwatch.StartNew();
			for (int i = 0; i < count; i++)
			{
				testAction();
			}
			timer.Stop();

			Console.WriteLine(name + " -> " + timer.Elapsed.TotalMilliseconds);

			return timer.Elapsed.TotalMilliseconds;
		}

		#endregion

		[Test]
		public void ReflectionCore_PropertyAccess()
		{
			const int load = 100000;

			Console.WriteLine("Load -> " + load);

			Console.WriteLine("-------");
			var obj = new BusinessPerformance { Id = 1 };

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

		[TestCase(100, 10)]
		[TestCase(100, 100)]
		[TestCase(100, 1000)]
		[TestCase(1000, 10)]
		[TestCase(1000, 100)]
		[TestCase(1000, 1000)]
		public void GetObjectData_WithListProperty(int load, int sub_obj_count)
		{
			const double max_engine_overhead_ratio = 5;
			const double max_client_overhead_ratio = 2;

			#region setup
			Console.WriteLine("Load -> " + load + "x" + sub_obj_count);
			Console.WriteLine("------");

			const int obj_id = 1;

			var obj = NewObj(obj_id, sub_obj_count);

			var obj_type = typeof(BusinessPerformance).FullName;
			var sub_type = typeof(BusinessPerformanceSub).FullName;

			AddToRepository(obj);
			var dummyLoadToPrintMessagesBeforeManuelCoding = objectService.Get(new ObjectReferenceData
			{
				ActualModelId = obj_type,
				ViewModelId = obj_type,
				Id = obj_id.ToString(CultureInfo.InvariantCulture)
			});
			#endregion

			#region manuel
			var manuel_time = Run("manuel", () =>
					{
						var ord = new ObjectReferenceData
						{
							ActualModelId = obj_type,
							ViewModelId = obj_type,
							Id = obj_id.ToString(CultureInfo.InvariantCulture)
						};

						var foundObj = objectRepository[ord.Id] as BusinessPerformance;
						var result = new ObjectData
						{
							Reference = new ObjectReferenceData
							{
								ActualModelId = ord.ActualModelId,
								ViewModelId = ord.ViewModelId,
								Id = foundObj.Id.ToString(CultureInfo.InvariantCulture),
							},
							Members = new Dictionary<string, ValueData>{
							{"Items", new ValueData {
									IsList = true,
									Values = foundObj.Items.Select(sub => new ObjectData{
										Reference = new ObjectReferenceData {
											ActualModelId = sub_type,
											ViewModelId = sub_type,
											Id = sub.Id.ToString(CultureInfo.InvariantCulture),
										},
										Value = sub.ToString(),
										
										#region Prop1 - Prop10
										Members = new Dictionary<string, ValueData>
										{
											{
												"Prop1", 
												new ValueData
												{
													IsList =  false,
													Values = new List<ObjectData>
													{
														new ObjectData
														{
															Reference = new ObjectReferenceData
															{
																ActualModelId = "System.String",
																ViewModelId = "System.String",
																Id = sub.Prop1
															},
															Value = sub.Prop1
														}
													}
												}
											},
											{
												"Prop2", 
												new ValueData
												{
													IsList =  false,
													Values = new List<ObjectData>
													{
														new ObjectData
														{
															Reference = new ObjectReferenceData
															{
																ActualModelId = "System.String",
																ViewModelId = "System.String",
																Id = sub.Prop2
															},
															Value = sub.Prop2
														}
													}
												}
											},
											{
												"Prop3", 
												new ValueData
												{
													IsList =  false,
													Values = new List<ObjectData>
													{
														new ObjectData
														{
															Reference = new ObjectReferenceData
															{
																ActualModelId = "System.String",
																ViewModelId = "System.String",
																Id = sub.Prop3
															},
															Value = sub.Prop3
														}
													}
												}
											},
											{
												"Prop4", 
												new ValueData
												{
													IsList =  false,
													Values = new List<ObjectData>
													{
														new ObjectData
														{
															Reference = new ObjectReferenceData
															{
																ActualModelId = "System.String",
																ViewModelId = "System.String",
																Id = sub.Prop4
															},
															Value = sub.Prop4
														}
													}
												}
											},
											{
												"Prop5", 
												new ValueData
												{
													IsList =  false,
													Values = new List<ObjectData>
													{
														new ObjectData
														{
															Reference = new ObjectReferenceData
															{
																ActualModelId = "System.String",
																ViewModelId = "System.String",
																Id = sub.Prop5
															},
															Value = sub.Prop5
														}
													}
												}
											},
											{
												"Prop6", 
												new ValueData
												{
													IsList =  false,
													Values = new List<ObjectData>
													{
														new ObjectData
														{
															Reference = new ObjectReferenceData
															{
																ActualModelId = "System.String",
																ViewModelId = "System.String",
																Id = sub.Prop6
															},
															Value = sub.Prop6
														}
													}
												}
											},
											{
												"Prop7", 
												new ValueData
												{
													IsList =  false,
													Values = new List<ObjectData>
													{
														new ObjectData
														{
															Reference = new ObjectReferenceData
															{
																ActualModelId = "System.String",
																ViewModelId = "System.String",
																Id = sub.Prop7
															},
															Value = sub.Prop7
														}
													}
												}
											},
											{
												"Prop8", 
												new ValueData
												{
													IsList =  false,
													Values = new List<ObjectData>
													{
														new ObjectData
														{
															Reference = new ObjectReferenceData
															{
																ActualModelId = "System.String",
																ViewModelId = "System.String",
																Id = sub.Prop8
															},
															Value = sub.Prop8
														}
													}
												}
											},
											{
												"Prop9", 
												new ValueData
												{
													IsList =  false,
													Values = new List<ObjectData>
													{
														new ObjectData
														{
															Reference = new ObjectReferenceData
															{
																ActualModelId = "System.String",
																ViewModelId = "System.String",
																Id = sub.Prop9
															},
															Value = sub.Prop9
														}
													}
												}
											},
											{
												"Prop10", 
												new ValueData
												{
													IsList =  false,
													Values = new List<ObjectData>
													{
														new ObjectData
														{
															Reference = new ObjectReferenceData
															{
																ActualModelId = "System.String",
																ViewModelId = "System.String",
																Id = sub.Prop10
															},
															Value = sub.Prop10
														}
													}
												}
											}
										}
										#endregion

									}).ToList()
								}
							}
						}
						};
					}, load);
			#endregion

			#region engine
			var engine_time = Run("engine", () =>
					{
						var result = objectService.Get(new ObjectReferenceData
						{
							ActualModelId = obj_type,
							ViewModelId = obj_type,
							Id = obj_id.ToString(CultureInfo.InvariantCulture)
						});
					}, load);
			#endregion

			#region client
			var client_time = Run("client api", () =>
					{
						var items = rapp.Get(obj_id.ToString(CultureInfo.InvariantCulture), obj_type)["Items"].Get().List.Select(r => r.Value);
					}, load);
			#endregion

			Assert.LessOrEqual(engine_time / manuel_time, max_engine_overhead_ratio, "Engine over manuel is above expected");
			Assert.LessOrEqual(client_time / engine_time, max_client_overhead_ratio, "Client over engine is above expected");
		}

		[TestCase(1000)]
		[TestCase(10000)]
		[TestCase(100000)]
		public void PerformOperation_LightParameter_HeavyLoad(int load)
		{
			const double max_engine_overhead_ratio = 7.2;
			const double max_client_overhead_ratio = 2;

			#region setup
			Console.WriteLine("Load -> " + load);
			Console.WriteLine("------");

			const int obj_id = 1;

			var obj_type = typeof(BusinessPerformance).FullName;
			var sub_type = typeof(BusinessPerformanceSub).FullName;

			var obj = NewObj(obj_id, 1);

			AddToRepository(obj);
			#endregion

			#region manuel
			var manuel_time = Run("manuel", () =>
				{
					var ord = new ObjectReferenceData
					{
						ActualModelId = obj_type,
						ViewModelId = obj_type,
						Id = obj_id.ToString(CultureInfo.InvariantCulture)
					};
					var foundObj = objectRepository[ord.Id] as BusinessPerformance;

					var sub = foundObj.GetSub(int.Parse("0"));
					var returnResult = new ValueData
					{
						IsList = false,
						Values = new List<ObjectData> {
							new ObjectData {
								Value = sub.ToString(),
								Reference = new ObjectReferenceData {
									ActualModelId = sub_type,
									ViewModelId = sub_type,
									Id = sub.Id.ToString(CultureInfo.InvariantCulture)
								},
								#region Prop1 - Prop10
								Members = new Dictionary<string, ValueData>
								{
									{
										"Prop1", 
										new ValueData
										{
											IsList =  false,
											Values = new List<ObjectData>
											{
												new ObjectData
												{
													Reference = new ObjectReferenceData
													{
														ActualModelId = "System.String",
														ViewModelId = "System.String",
														Id = sub.Prop1
													},
													Value = sub.Prop1
												}
											}
										}
									},
									{
										"Prop2", 
										new ValueData
										{
											IsList =  false,
											Values = new List<ObjectData>
											{
												new ObjectData
												{
													Reference = new ObjectReferenceData
													{
														ActualModelId = "System.String",
														ViewModelId = "System.String",
														Id = sub.Prop2
													},
													Value = sub.Prop2
												}
											}
										}
									},
									{
										"Prop3", 
										new ValueData
										{
											IsList =  false,
											Values = new List<ObjectData>
											{
												new ObjectData
												{
													Reference = new ObjectReferenceData
													{
														ActualModelId = "System.String",
														ViewModelId = "System.String",
														Id = sub.Prop3
													},
													Value = sub.Prop3
												}
											}
										}
									},
									{
										"Prop4", 
										new ValueData
										{
											IsList =  false,
											Values = new List<ObjectData>
											{
												new ObjectData
												{
													Reference = new ObjectReferenceData
													{
														ActualModelId = "System.String",
														ViewModelId = "System.String",
														Id = sub.Prop4
													},
													Value = sub.Prop4
												}
											}
										}
									},
									{
										"Prop5", 
										new ValueData
										{
											IsList =  false,
											Values = new List<ObjectData>
											{
												new ObjectData
												{
													Reference = new ObjectReferenceData
													{
														ActualModelId = "System.String",
														ViewModelId = "System.String",
														Id = sub.Prop5
													},
													Value = sub.Prop5
												}
											}
										}
									},
									{
										"Prop6", 
										new ValueData
										{
											IsList =  false,
											Values = new List<ObjectData>
											{
												new ObjectData
												{
													Reference = new ObjectReferenceData
													{
														ActualModelId = "System.String",
														ViewModelId = "System.String",
														Id = sub.Prop6
													},
													Value = sub.Prop6
												}
											}
										}
									},
									{
										"Prop7", 
										new ValueData
										{
											IsList =  false,
											Values = new List<ObjectData>
											{
												new ObjectData
												{
													Reference = new ObjectReferenceData
													{
														ActualModelId = "System.String",
														ViewModelId = "System.String",
														Id = sub.Prop7
													},
													Value = sub.Prop7
												}
											}
										}
									},
									{
										"Prop8", 
										new ValueData
										{
											IsList =  false,
											Values = new List<ObjectData>
											{
												new ObjectData
												{
													Reference = new ObjectReferenceData
													{
														ActualModelId = "System.String",
														ViewModelId = "System.String",
														Id = sub.Prop8
													},
													Value = sub.Prop8
												}
											}
										}
									},
									{
										"Prop9", 
										new ValueData
										{
											IsList =  false,
											Values = new List<ObjectData>
											{
												new ObjectData
												{
													Reference = new ObjectReferenceData
													{
														ActualModelId = "System.String",
														ViewModelId = "System.String",
														Id = sub.Prop9
													},
													Value = sub.Prop9
												}
											}
										}
									},
									{
										"Prop10", 
										new ValueData
										{
											IsList =  false,
											Values = new List<ObjectData>
											{
												new ObjectData
												{
													Reference = new ObjectReferenceData
													{
														ActualModelId = "System.String",
														ViewModelId = "System.String",
														Id = sub.Prop10
													},
													Value = sub.Prop10
												}
											}
										}
									}
								}
								#endregion
							}
						}
					};
				}, load);
			#endregion

			#region engine
			var engine_time = Run("engine", () =>
				{
					var ord = new ObjectReferenceData
					{
						ActualModelId = obj_type,
						ViewModelId = obj_type,
						Id = obj_id.ToString(CultureInfo.InvariantCulture)
					};
					var returnResult = objectService.PerformOperation(ord, "GetSub", new Dictionary<string, ParameterValueData>{
					{"index", new ParameterValueData {
							IsList = false,
							Values = new List<ParameterData> {
								new ParameterData {
									ObjectModelId= "s-int-32",
									ReferenceId = "0"
								}
							}
						}
					}
				});
				}, load);

			#endregion

			#region client
			var client_time = Run("client api", () =>
				{
					var rvar = rapp.NewVar("index", rapp.Get("0", "s-int-32"));
					var returnResult = rapp.Get(obj_id.ToString(CultureInfo.InvariantCulture), obj_type).Perform("GetSub", rvar);
				}, load);

			#endregion

			Assert.LessOrEqual(engine_time / manuel_time, max_engine_overhead_ratio, "Engine over manuel is above expected");
			Assert.LessOrEqual(client_time / engine_time, max_client_overhead_ratio, "Client over engine is above expected");
		}

		[TestCase(10, 10)]
		[TestCase(10, 100)]
		[TestCase(10, 1000)]
		[TestCase(10, 10000)]
		[TestCase(10, 100000)]
		public void PerformOperation_HeavyParameter_LightLoad(int load, int input_count)
		{
			const double max_engine_overhead_ratio = 7.2;
			const double max_client_overhead_ratio = 2;

			#region setup
			Console.WriteLine("Load -> " + load + "x" + input_count);
			Console.WriteLine("------");

			const int obj_id = 1;

			var obj_type = typeof(BusinessPerformance).FullName;
			var input_type = typeof(BusinessPerformanceInput).FullName;

			var obj = NewObj(obj_id, 1);

			AddToRepository(obj);
			const string str_in = "str_in";
			const string int_in = "20";

			#endregion

			#region manuel
			var manuel_time = Run("manuel", () =>
			{
				var parameters = new Dictionary<string, ParameterValueData>
				{
					{
						"input", new ParameterValueData
						{
							IsList = false,
							Values = Enumerable
								.Range(0, input_count)
								.Select(i =>

									#region parameter data
									new ParameterData
									{
										ObjectModelId = input_type,
										InitializationParameters = new Dictionary<string, ParameterValueData>
										{
											{
												"s", new ParameterValueData
												{
													IsList = false,
													Values = new List<ParameterData>
													{
														new ParameterData
														{
															ObjectModelId = "s-string",
															ReferenceId = str_in
														}
													}
												}
											},
											{
												"i", new ParameterValueData
												{
													IsList = false,
													Values = new List<ParameterData>
													{
														new ParameterData
														{
															ObjectModelId = "s-int-32",
															ReferenceId = int_in
														}
													}
												}
											},
											{
												"s2", new ParameterValueData
												{
													IsList = false,
													Values = new List<ParameterData>
													{
														new ParameterData
														{
															ObjectModelId = "s-string",
															ReferenceId = str_in
														}
													}
												}
											},
											{
												"s3", new ParameterValueData
												{
													IsList = false,
													Values = new List<ParameterData>
													{
														new ParameterData
														{
															ObjectModelId = "s-string",
															ReferenceId = str_in
														}
													}
												}
											},
											{
												"s4", new ParameterValueData
												{
													IsList = false,
													Values = new List<ParameterData>
													{
														new ParameterData
														{
															ObjectModelId = "s-string",
															ReferenceId = str_in
														}
													}
												}
											}
										}
									} 
								#endregion

								).ToList()
						}
					}
				};

				var ord = new ObjectReferenceData
				{
					ActualModelId = obj_type,
					ViewModelId = obj_type,
					Id = obj_id.ToString(CultureInfo.InvariantCulture)
				};

				var foundObj = objectRepository[ord.Id] as BusinessPerformance;

				var parameter = parameters["input"];
				var returnResult = foundObj
					.Create(Enumerable
						.Range(0, input_count)
						.Select(i => new BusinessPerformanceInput(
							parameter.Values[i].InitializationParameters["s"].Values[0].ReferenceId,
							int.Parse(parameter.Values[i].InitializationParameters["i"].Values[0].ReferenceId),
							parameter.Values[i].InitializationParameters["s2"].Values[0].ReferenceId,
							parameter.Values[i].InitializationParameters["s3"].Values[0].ReferenceId,
							parameter.Values[i].InitializationParameters["s4"].Values[0].ReferenceId
						)).ToList())
					.ToString(CultureInfo.InvariantCulture);

			}, load);
			#endregion
			
			#region engine
			var engine_time = Run("engine", () =>
			{
				var parameters = new Dictionary<string, ParameterValueData>
				{
					{
						"input", new ParameterValueData
						{
							IsList = false,
							Values = Enumerable
								.Range(0, input_count)
								.Select(i =>

									#region parameter data
									new ParameterData
									{
										ObjectModelId = input_type,
										InitializationParameters = new Dictionary<string, ParameterValueData>
										{
											{
												"s", new ParameterValueData
												{
													IsList = false,
													Values = new List<ParameterData>
													{
														new ParameterData
														{
															ObjectModelId = "s-string",
															ReferenceId = str_in
														}
													}
												}
											},
											{
												"i", new ParameterValueData
												{
													IsList = false,
													Values = new List<ParameterData>
													{
														new ParameterData
														{
															ObjectModelId = "s-int-32",
															ReferenceId = int_in
														}
													}
												}
											},
											{
												"s2", new ParameterValueData
												{
													IsList = false,
													Values = new List<ParameterData>
													{
														new ParameterData
														{
															ObjectModelId = "s-string",
															ReferenceId = str_in
														}
													}
												}
											},
											{
												"s3", new ParameterValueData
												{
													IsList = false,
													Values = new List<ParameterData>
													{
														new ParameterData
														{
															ObjectModelId = "s-string",
															ReferenceId = str_in
														}
													}
												}
											},
											{
												"s4", new ParameterValueData
												{
													IsList = false,
													Values = new List<ParameterData>
													{
														new ParameterData
														{
															ObjectModelId = "s-string",
															ReferenceId = str_in
														}
													}
												}
											}
										}
									} 
								#endregion

								).ToList()
						}
					}
				};
				var ord = new ObjectReferenceData
				{
					ActualModelId = obj_type,
					ViewModelId = obj_type,
					Id = obj_id.ToString(CultureInfo.InvariantCulture)
				};
				var returnResult = objectService.PerformOperation(ord, "Create", parameters);
			}, load);
			#endregion

			#region client
			var client_time = Run("client api", () =>
			{
				var rvar = rapp.NewVarList("input",
					Enumerable
						.Range(0, input_count)
						.Select(i =>
							rapp.Init(input_type,
								rapp.NewVar("s", str_in, "s-string"),
								rapp.NewVar("i", int_in, "s-int-32"),
								rapp.NewVar("s2", str_in, "s-string"),
								rapp.NewVar("s3", str_in, "s-string"),
								rapp.NewVar("s4", str_in, "s-string")
							)
						)
					);
				var returnResult = rapp.Get(obj_id.ToString(CultureInfo.InvariantCulture), obj_type).Perform("Create", rvar);
			}, load);

			#endregion

			Assert.LessOrEqual(engine_time / manuel_time, max_engine_overhead_ratio, "Engine over manuel is above expected");
			Assert.LessOrEqual(client_time / engine_time, max_client_overhead_ratio, "Client over engine is above expected");
		}
	}
}

