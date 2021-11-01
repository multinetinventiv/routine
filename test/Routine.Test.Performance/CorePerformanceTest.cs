using NUnit.Framework;
using Routine.Client;
using Routine.Core;
using Routine.Core.Cache;
using Routine.Core.Reflection;
using Routine.Engine;
using Routine.Test.Performance.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

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

    public readonly struct BusinessPerformanceInput
    {
        public string Str { get; }
        public int Int { get; }
        public string Str2 { get; }
        public string Str3 { get; }
        public string Str4 { get; }

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

        public bool Equals(BusinessPerformanceInput other) =>
            string.Equals(Str, other.Str) &&
            Int == other.Int &&
            string.Equals(Str2, other.Str2) &&
            string.Equals(Str3, other.Str3) &&
            string.Equals(Str4, other.Str4);

        public override bool Equals(object obj) => obj is BusinessPerformanceInput input && Equals(input);

        public override int GetHashCode() => HashCode.Combine(Str, Int, Str2, Str3, Str4);
    }
}

#endregion

namespace Routine.Test.Performance
{
    [TestFixture]
    [Ignore("should be run manually")]
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

            ReflectionOptimizer.Enable();

            var apiCtx = BuildRoutine.Context()
                .AsClientApplication(
                    codingStyle = BuildRoutine.CodingStyle()
                        .FromBasic()
                        .AddCommonSystemTypes()
                        .AddTypes(GetType().Assembly, t => t.Namespace?.StartsWith("Routine.Test.Performance.Domain") == true)

                        .Use(p => p.ParseableValueTypePattern())

                        .Initializers.Add(c => c.PublicConstructors().When(type.of<BusinessPerformanceInput>()))
                        .Datas.Add(c => c.Properties(m => !m.IsInherited(true, true)))
                        .DataFetchedEagerly.Set(true)
                        .Operations.Add(c => c.Methods(o => !o.IsInherited(true, true)))
                        .IdExtractor.Set(c => c.IdByProperty(m => m.Returns<int>("Id")))
                        .Locator.Set(c => c.Locator(l => l.SingleBy(id => objectRepository[id])))
                        .ValueExtractor.Set(c => c.Value(e => e.By(o => $"{o}")))
                );

            objectService = apiCtx.ObjectService;
            rapp = apiCtx.Application;

            var _ = objectService.ApplicationModel;
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
            var result = new BusinessPerformance
            {
                Id = id
            };

            for (var i = 0; i < subObjectCount; i++)
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

        private double Run(string name, Action testAction, int count)
        {
            //first call is not included to let it do its inital loading & caching etc.
            testAction();

            var timer = Stopwatch.StartNew();
            for (var i = 0; i < count; i++)
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
                    var _ = obj.Id;
                }, load);

            Console.WriteLine("-------");

            var prop = obj.GetType().GetProperty("Id");
            Run("System.Reflection Cached Access", () =>
            {
                var _ = prop?.GetValue(obj, Array.Empty<object>());
            }, load);
            ReflectionOptimizer.Disable();
            var rprop1 = obj.GetTypeInfo().GetProperty("Id");
            Run("Routine.Core.Reflection Cached Access (without optimizer) ", () =>
            {
                var _ = rprop1.GetValue(obj);
            }, load);
            ReflectionOptimizer.Enable();
            var rprop2 = obj.GetTypeInfo().GetProperty("Id");
            Run("Routine.Core.Reflection Cached Access", () =>
            {
                var _ = rprop2.GetValue(obj);
            }, load);
            Console.WriteLine("-------");

            Run("System.Reflection Access", () =>
            {
                var _ = obj.GetType().GetProperty("Id")?.GetValue(obj, Array.Empty<object>());
            }, load);
            ReflectionOptimizer.Disable();
            Run("Routine.Core.Reflection Access (without optimizer)", () =>
            {
                var _ = obj.GetTypeInfo().GetProperty("Id").GetValue(obj);
            }, load);
            ReflectionOptimizer.Enable();
            Run("Routine.Core.Reflection Access", () =>
                {
                    var _ = obj.GetTypeInfo().GetProperty("Id").GetValue(obj);
                }, load);

            Console.WriteLine("-------");

            Run("Routine.Core.Reflection Access -> GetTypeInfo()", () =>
                {
                    var _ = obj.GetTypeInfo();
                }, load);
            var type = obj.GetTypeInfo();
            Run("Routine.Core.Reflection Access -> GetProperty('Id')", () =>
                {
                    var _ = type.GetProperty("Id");
                }, load);
            var rprop3 = type.GetProperty("Id");
            Run("Routine.Core.Reflection Access -> GetValue(obj)", () =>
                {
                    var _ = rprop3.GetValue(obj);
                }, load);
        }

        [TestCase(100, 10)]
        [TestCase(100, 100)]
        [TestCase(100, 1000)]
        [TestCase(1000, 10)]
        [TestCase(1000, 100)]
        [TestCase(1000, 1000)]
        public void GetObjectData_WithListProperty(int load, int subObjCount)
        {
            const double maxEngineOverheadRatio = 5;
            const double maxClientOverheadRatio = 2;

            #region setup
            Console.WriteLine("Load -> " + load + "x" + subObjCount);
            Console.WriteLine("------");

            const int objId = 1;

            var obj = NewObj(objId, subObjCount);

            var objType = typeof(BusinessPerformance).FullName;
            var subType = typeof(BusinessPerformanceSub).FullName;

            AddToRepository(obj);
            var __ = objectService.Get(new ReferenceData
            {
                ModelId = objType,
                ViewModelId = objType,
                Id = objId.ToString(CultureInfo.InvariantCulture)
            });
            #endregion

            #region manuel
            var manuelTime = Run("manuel", () =>
                    {
                        var ord = new ReferenceData
                        {
                            ModelId = objType,
                            ViewModelId = objType,
                            Id = objId.ToString(CultureInfo.InvariantCulture)
                        };

                        var foundObj = (BusinessPerformance)objectRepository[ord.Id];
                        var _ = new ObjectData
                        {
                            ModelId = ord.ModelId,
                            Id = foundObj.Id.ToString(CultureInfo.InvariantCulture),
                            Data = new Dictionary<string, VariableData>{
                            {"Items", new VariableData {
                                    IsList = true,
                                    Values = foundObj.Items.Select(sub => new ObjectData{
                                        ModelId = subType,
                                        Id = sub.Id.ToString(CultureInfo.InvariantCulture),
                                        Display = sub.ToString(),
										
										#region Prop1 - Prop10
										Data = new Dictionary<string, VariableData>
                                        {
                                            {
                                                "Prop1",
                                                new VariableData
                                                {
                                                    IsList =  false,
                                                    Values = new List<ObjectData>
                                                    {
                                                        new()
                                                        {
                                                            ModelId = "System.String",
                                                            Id = sub.Prop1,
                                                            Display = sub.Prop1
                                                        }
                                                    }
                                                }
                                            },
                                            {
                                                "Prop2",
                                                new VariableData
                                                {
                                                    IsList =  false,
                                                    Values = new List<ObjectData>
                                                    {
                                                        new()
                                                        {
                                                            ModelId = "System.String",
                                                            Id = sub.Prop2,
                                                            Display = sub.Prop2
                                                        }
                                                    }
                                                }
                                            },
                                            {
                                                "Prop3",
                                                new VariableData
                                                {
                                                    IsList =  false,
                                                    Values = new List<ObjectData>
                                                    {
                                                        new()
                                                        {
                                                            ModelId = "System.String",
                                                            Id = sub.Prop3,
                                                            Display = sub.Prop3
                                                        }
                                                    }
                                                }
                                            },
                                            {
                                                "Prop4",
                                                new VariableData
                                                {
                                                    IsList =  false,
                                                    Values = new List<ObjectData>
                                                    {
                                                        new()
                                                        {
                                                            ModelId = "System.String",
                                                            Id = sub.Prop4,
                                                            Display = sub.Prop4
                                                        }
                                                    }
                                                }
                                            },
                                            {
                                                "Prop5",
                                                new VariableData
                                                {
                                                    IsList =  false,
                                                    Values = new List<ObjectData>
                                                    {
                                                        new()
                                                        {
                                                            ModelId = "System.String",
                                                            Id = sub.Prop5,
                                                            Display = sub.Prop5
                                                        }
                                                    }
                                                }
                                            },
                                            {
                                                "Prop6",
                                                new VariableData
                                                {
                                                    IsList =  false,
                                                    Values = new List<ObjectData>
                                                    {
                                                        new()
                                                        {
                                                            ModelId = "System.String",
                                                            Id = sub.Prop6,
                                                            Display = sub.Prop6
                                                        }
                                                    }
                                                }
                                            },
                                            {
                                                "Prop7",
                                                new VariableData
                                                {
                                                    IsList =  false,
                                                    Values = new List<ObjectData>
                                                    {
                                                        new()
                                                        {
                                                            ModelId = "System.String",
                                                            Id = sub.Prop7,
                                                            Display = sub.Prop7
                                                        }
                                                    }
                                                }
                                            },
                                            {
                                                "Prop8",
                                                new VariableData
                                                {
                                                    IsList =  false,
                                                    Values = new List<ObjectData>
                                                    {
                                                        new()
                                                        {
                                                            ModelId = "System.String",
                                                            Id = sub.Prop8,
                                                            Display = sub.Prop8
                                                        }
                                                    }
                                                }
                                            },
                                            {
                                                "Prop9",
                                                new VariableData
                                                {
                                                    IsList =  false,
                                                    Values = new List<ObjectData>
                                                    {
                                                        new()
                                                        {
                                                            ModelId = "System.String",
                                                            Id = sub.Prop9,
                                                            Display = sub.Prop9
                                                        }
                                                    }
                                                }
                                            },
                                            {
                                                "Prop10",
                                                new VariableData
                                                {
                                                    IsList =  false,
                                                    Values = new List<ObjectData>
                                                    {
                                                        new()
                                                        {
                                                            ModelId = "System.String",
                                                            Id = sub.Prop10,
                                                            Display = sub.Prop10
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
            var engineTime = Run("engine", () =>
                    {
                        var _ = objectService.Get(new ReferenceData
                        {
                            ModelId = objType,
                            ViewModelId = objType,
                            Id = objId.ToString(CultureInfo.InvariantCulture)
                        });
                    }, load);
            #endregion

            #region client
            var clientTime = Run("client api", () =>
                    {
                        var _ = rapp.Get(objId.ToString(CultureInfo.InvariantCulture), objType)["Items"].Get().List.Select(r => r.Display);
                    }, load);
            #endregion

            Assert.LessOrEqual(engineTime / manuelTime, maxEngineOverheadRatio, "Engine over manuel is above expected");
            Assert.LessOrEqual(clientTime / engineTime, maxClientOverheadRatio, "Client over engine is above expected");
        }

        [TestCase(1000)]
        [TestCase(10000)]
        [TestCase(100000)]
        public void PerformOperation_LightParameter_HeavyLoad(int load)
        {
            const double maxEngineOverheadRatio = 7.2;
            const double maxClientOverheadRatio = 2;

            #region setup
            Console.WriteLine("Load -> " + load);
            Console.WriteLine("------");

            const int objId = 1;

            var objType = typeof(BusinessPerformance).FullName;
            var subType = typeof(BusinessPerformanceSub).FullName;

            var obj = NewObj(objId, 1);

            AddToRepository(obj);
            #endregion

            #region manuel
            var manuelTime = Run("manuel", () =>
                {
                    var ord = new ReferenceData
                    {
                        ModelId = objType,
                        ViewModelId = objType,
                        Id = objId.ToString(CultureInfo.InvariantCulture)
                    };
                    var foundObj = (BusinessPerformance)objectRepository[ord.Id];

                    var sub = foundObj.GetSub(int.Parse("0"));
                    var _ = new VariableData
                    {
                        IsList = false,
                        Values = new List<ObjectData> {
                            new()
                            {
                                Display = sub.ToString(),
                                ModelId = subType,
                                Id = sub.Id.ToString(CultureInfo.InvariantCulture),
								#region Prop1 - Prop10
								Data = new Dictionary<string, VariableData>
                                {
                                    {
                                        "Prop1",
                                        new VariableData
                                        {
                                            IsList =  false,
                                            Values = new List<ObjectData>
                                            {
                                                new()
                                                {
                                                    ModelId = "System.String",
                                                    Id = sub.Prop1,
                                                    Display = sub.Prop1
                                                }
                                            }
                                        }
                                    },
                                    {
                                        "Prop2",
                                        new VariableData
                                        {
                                            IsList =  false,
                                            Values = new List<ObjectData>
                                            {
                                                new()
                                                {
                                                    ModelId = "System.String",
                                                    Id = sub.Prop2,
                                                    Display = sub.Prop2
                                                }
                                            }
                                        }
                                    },
                                    {
                                        "Prop3",
                                        new VariableData
                                        {
                                            IsList =  false,
                                            Values = new List<ObjectData>
                                            {
                                                new()
                                                {
                                                    ModelId = "System.String",
                                                    Id = sub.Prop3,
                                                    Display = sub.Prop3
                                                }
                                            }
                                        }
                                    },
                                    {
                                        "Prop4",
                                        new VariableData
                                        {
                                            IsList =  false,
                                            Values = new List<ObjectData>
                                            {
                                                new()
                                                {
                                                    ModelId = "System.String",
                                                    Id = sub.Prop4,
                                                    Display = sub.Prop4
                                                }
                                            }
                                        }
                                    },
                                    {
                                        "Prop5",
                                        new VariableData
                                        {
                                            IsList =  false,
                                            Values = new List<ObjectData>
                                            {
                                                new()
                                                {
                                                    ModelId = "System.String",
                                                    Id = sub.Prop5,
                                                    Display = sub.Prop5
                                                }
                                            }
                                        }
                                    },
                                    {
                                        "Prop6",
                                        new VariableData
                                        {
                                            IsList =  false,
                                            Values = new List<ObjectData>
                                            {
                                                new()
                                                {
                                                    ModelId = "System.String",
                                                    Id = sub.Prop6,
                                                    Display = sub.Prop6
                                                }
                                            }
                                        }
                                    },
                                    {
                                        "Prop7",
                                        new VariableData
                                        {
                                            IsList =  false,
                                            Values = new List<ObjectData>
                                            {
                                                new()
                                                {
                                                    ModelId = "System.String",
                                                    Id = sub.Prop7,
                                                    Display = sub.Prop7
                                                }
                                            }
                                        }
                                    },
                                    {
                                        "Prop8",
                                        new VariableData
                                        {
                                            IsList =  false,
                                            Values = new List<ObjectData>
                                            {
                                                new()
                                                {
                                                    ModelId = "System.String",
                                                    Id = sub.Prop8,
                                                    Display = sub.Prop8
                                                }
                                            }
                                        }
                                    },
                                    {
                                        "Prop9",
                                        new VariableData
                                        {
                                            IsList =  false,
                                            Values = new List<ObjectData>
                                            {
                                                new()
                                                {
                                                    ModelId = "System.String",
                                                    Id = sub.Prop9,
                                                    Display = sub.Prop9
                                                }
                                            }
                                        }
                                    },
                                    {
                                        "Prop10",
                                        new VariableData
                                        {
                                            IsList =  false,
                                            Values = new List<ObjectData>
                                            {
                                                new()
                                                {
                                                    ModelId = "System.String",
                                                    Id = sub.Prop10,
                                                    Display = sub.Prop10
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
            var engineTime = Run("engine", () =>
                {
                    var ord = new ReferenceData
                    {
                        ModelId = objType,
                        ViewModelId = objType,
                        Id = objId.ToString(CultureInfo.InvariantCulture)
                    };
                    var _ = objectService.Do(ord, "GetSub", new Dictionary<string, ParameterValueData>{
                    {"index", new ParameterValueData {
                            IsList = false,
                            Values = new List<ParameterData> {
                                new()
                                {
                                    ModelId= "System.Int32",
                                    Id = "0"
                                }
                            }
                        }
                    }
                });
                }, load);

            #endregion

            #region client
            var clientTime = Run("client api", () =>
                {
                    var rvar = rapp.NewVar("index", rapp.Get("0", "System.Int32"));
                    var _ = rapp.Get(objId.ToString(CultureInfo.InvariantCulture), objType).Perform("GetSub", rvar);
                }, load);

            #endregion

            Assert.LessOrEqual(engineTime / manuelTime, maxEngineOverheadRatio, "Engine over manuel is above expected");
            Assert.LessOrEqual(clientTime / engineTime, maxClientOverheadRatio, "Client over engine is above expected");
        }

        [TestCase(10, 10)]
        [TestCase(10, 100)]
        [TestCase(10, 1000)]
        [TestCase(10, 10000)]
        [TestCase(10, 100000)]
        public void PerformOperation_HeavyParameter_LightLoad(int load, int inputCount)
        {
            const double maxEngineOverheadRatio = 7.2;
            const double maxClientOverheadRatio = 2;

            #region setup
            Console.WriteLine("Load -> " + load + "x" + inputCount);
            Console.WriteLine("------");

            const int objId = 1;

            var objType = typeof(BusinessPerformance).FullName;
            var inputType = typeof(BusinessPerformanceInput).FullName;

            var obj = NewObj(objId, 1);

            AddToRepository(obj);
            const string strIn = "str_in";
            const string intIn = "20";

            #endregion

            #region manuel
            var manuelTime = Run("manuel", () =>
            {
                var parameters = new Dictionary<string, ParameterValueData>
                {
                    {
                        "input", new ParameterValueData
                        {
                            IsList = false,
                            Values = Enumerable
                                .Range(0, inputCount)
                                .Select(_ =>

									#region parameter data
									new ParameterData
                                    {
                                        ModelId = inputType,
                                        InitializationParameters = new Dictionary<string, ParameterValueData>
                                        {
                                            {
                                                "s", new ParameterValueData
                                                {
                                                    IsList = false,
                                                    Values = new List<ParameterData>
                                                    {
                                                        new()
                                                        {
                                                            ModelId = "s-string",
                                                            Id = strIn
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
                                                        new()
                                                        {
                                                            ModelId = "s-int-32",
                                                            Id = intIn
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
                                                        new()
                                                        {
                                                            ModelId = "s-string",
                                                            Id = strIn
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
                                                        new()
                                                        {
                                                            ModelId = "s-string",
                                                            Id = strIn
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
                                                        new()
                                                        {
                                                            ModelId = "s-string",
                                                            Id = strIn
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

                var ord = new ReferenceData
                {
                    ModelId = objType,
                    ViewModelId = objType,
                    Id = objId.ToString(CultureInfo.InvariantCulture)
                };

                var foundObj = (BusinessPerformance)objectRepository[ord.Id];

                var parameter = parameters["input"];
                var _ = foundObj
                    .Create(Enumerable
                        .Range(0, inputCount)
                        .Select(i => new BusinessPerformanceInput(
                            parameter.Values[i].InitializationParameters["s"].Values[0].Id,
                            int.Parse(parameter.Values[i].InitializationParameters["i"].Values[0].Id),
                            parameter.Values[i].InitializationParameters["s2"].Values[0].Id,
                            parameter.Values[i].InitializationParameters["s3"].Values[0].Id,
                            parameter.Values[i].InitializationParameters["s4"].Values[0].Id
                        )).ToList())
                    .ToString(CultureInfo.InvariantCulture);

            }, load);
            #endregion

            #region engine
            var engineTime = Run("engine", () =>
            {
                var parameters = new Dictionary<string, ParameterValueData>
                {
                    {
                        "input", new ParameterValueData
                        {
                            IsList = false,
                            Values = Enumerable
                                .Range(0, inputCount)
                                .Select(_ =>

									#region parameter data
									new ParameterData
                                    {
                                        ModelId = inputType,
                                        InitializationParameters = new Dictionary<string, ParameterValueData>
                                        {
                                            {
                                                "s", new ParameterValueData
                                                {
                                                    IsList = false,
                                                    Values = new List<ParameterData>
                                                    {
                                                        new()
                                                        {
                                                            ModelId = "System.String",
                                                            Id = strIn
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
                                                        new()
                                                        {
                                                            ModelId = "System.Int32",
                                                            Id = intIn
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
                                                        new()
                                                        {
                                                            ModelId = "System.String",
                                                            Id = strIn
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
                                                        new()
                                                        {
                                                            ModelId = "System.String",
                                                            Id = strIn
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
                                                        new()
                                                        {
                                                            ModelId = "System.String",
                                                            Id = strIn
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
                var ord = new ReferenceData
                {
                    ModelId = objType,
                    ViewModelId = objType,
                    Id = objId.ToString(CultureInfo.InvariantCulture)
                };
                var _ = objectService.Do(ord, "Create", parameters);
            }, load);
            #endregion

            #region client
            var clientTime = Run("client api", () =>
            {
                var rvar = rapp.NewVarList("input",
                    Enumerable
                        .Range(0, inputCount)
                        .Select(_ =>
                            rapp.Init(inputType,
                                rapp.NewVar("s", strIn, "System.String"),
                                rapp.NewVar("i", intIn, "System.Int32"),
                                rapp.NewVar("s2", strIn, "System.String"),
                                rapp.NewVar("s3", strIn, "System.String"),
                                rapp.NewVar("s4", strIn, "System.String")
                            )
                        )
                    );
                var _ = rapp.Get(objId.ToString(CultureInfo.InvariantCulture), objType).Perform("Create", rvar);
            }, load);

            #endregion

            Assert.LessOrEqual(engineTime / manuelTime, maxEngineOverheadRatio, "Engine over manuel is above expected");
            Assert.LessOrEqual(clientTime / engineTime, maxClientOverheadRatio, "Client over engine is above expected");
        }
    }
}

