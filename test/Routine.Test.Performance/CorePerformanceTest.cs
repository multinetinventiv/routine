using Routine.Client;
using Routine.Core.Reflection;
using Routine.Core;
using Routine.Engine;
using Routine.Test.Performance.Domain;
using System.Diagnostics;
using System.Globalization;

namespace Routine.Test.Performance;

[TestFixture]
public class CorePerformanceTest
{
    #region Setup & Helpers

    private Dictionary<string, object> _objectRepository;

    private ICodingStyle _codingStyle;
    private IObjectService _objectService;
    private Rapplication _rapp;

    [SetUp]
    public void SetUp()
    {
        _objectRepository = new();

        ReflectionOptimizer.Enable();

        var apiCtx = BuildRoutine.Context()
            .AsClientApplication(
                _codingStyle = BuildRoutine.CodingStyle()
                    .FromBasic()
                    .AddCommonSystemTypes()
                    .AddTypes(GetType().Assembly, t => t.Namespace?.StartsWith("Routine.Test.Performance.Domain") == true)

                    .Use(p => p.ParseableValueTypePattern())

                    .Initializers.Add(c => c.PublicConstructors().When(type.of<BusinessPerformanceInput>()))
                    .Datas.Add(c => c.Properties(m => !m.IsInherited(true, true)))
                    .DataFetchedEagerly.Set(true)
                    .Operations.Add(c => c.Methods(o => !o.IsInherited(true, true)))
                    .IdExtractor.Set(c => c.IdByProperty(m => m.Returns<int>("Id")))
                    .Locator.Set(c => c.Locator(l => l.SingleBy(id => _objectRepository[id])))
                    .ValueExtractor.Set(c => c.Value(e => e.By(o => $"{o}")))
            );

        _objectService = apiCtx.ObjectService;
        _rapp = apiCtx.Application;

        var _ = _objectService.ApplicationModel;

        var optimized = false;
        var onOptimized = new EventHandler((_, _) => optimized = true);
        ReflectionOptimizer.Optimized += onOptimized;

        var obj = new BusinessPerformance();
        var rprop1 = obj.GetTypeInfo().GetProperty("Id");
        rprop1.GetValue(obj);

        while(!optimized) { Thread.Sleep(1); }
        ReflectionOptimizer.Optimized -= onOptimized;
    }

    protected void AddToRepository(BusinessPerformance obj)
    {
        _objectRepository.Add(_codingStyle.GetIdExtractor(obj.GetTypeInfo()).GetId(obj), obj);
        foreach (var sub in obj.Items)
        {
            _objectRepository.Add(_codingStyle.GetIdExtractor(sub.GetTypeInfo()).GetId(sub), sub);
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
            result.Items.Add(new()
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
        var warmUpCount = 10;
        for (var i = 0; i < warmUpCount; i++)
        {
            testAction();
        }

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
    [Ignore("should be run manually")]
    public void ReflectionCore_PropertyAccess()
    {
        const int load = 1000000;

        Console.WriteLine("Load -> " + load);

        Console.WriteLine("-------");
        var obj = new BusinessPerformance { Id = 1 };

        Run("Direct Access",
            () => { var _ = obj.Id; },
            load
        );

        Console.WriteLine("-------");

        var prop = obj.GetType().GetProperty("Id");
        Run("System.Reflection Cached Access",
            () => { var _ = prop?.GetValue(obj); },
            load
        );

        ReflectionOptimizer.Disable();
        var rprop1 = obj.GetTypeInfo().GetProperty("Id");
        Run("Routine.Core.Reflection Cached Access (without optimizer)",
            () => { var _ = rprop1.GetValue(obj); },
            load
        );

        ReflectionOptimizer.Enable();
        var rprop2 = obj.GetTypeInfo().GetProperty("Id");
        Run("Routine.Core.Reflection Cached Access",
            () => { var _ = rprop2.GetValue(obj); },
            load
        );

        Console.WriteLine("-------");

        Run("System.Reflection Access",
            () => { var _ = obj.GetType().GetProperty("Id").GetValue(obj, Array.Empty<object>()); },
            load
        );

        ReflectionOptimizer.Disable();
        Run("Routine.Core.Reflection Access (without optimizer)",
            () => { var _ = obj.GetTypeInfo().GetProperty("Id").GetValue(obj); },
            load
        );

        ReflectionOptimizer.Enable();
        Run("Routine.Core.Reflection Access",
            () => { var _ = obj.GetTypeInfo().GetProperty("Id").GetValue(obj); },
            load
        );

        Console.WriteLine("-------");

        Run("System.Reflection Access -> GetType()",
            () => { var _ = obj.GetType(); },
            load
        );
        Run("Routine.Core.Reflection Access -> GetTypeInfo()",
            () => { var _ = obj.GetTypeInfo(); },
            load
        );

        Console.WriteLine("-------");

        var type = obj.GetType();
        Run("System.Reflection Access -> GetProperty('Id')",
            () => { var _ = type.GetProperty("Id"); },
            load
        );
        var typeInfo = obj.GetTypeInfo();
        Run("Routine.Core.Reflection Access -> GetProperty('Id')",
            () => { var _ = typeInfo.GetProperty("Id"); },
            load
        );

        Console.WriteLine("-------");

        var prop3 = type.GetProperty("Id");
        Run("System.Reflection Access -> GetValue(obj)",
            () => { var _ = prop3.GetValue(obj); },
            load
        );
        var rprop3 = typeInfo.GetProperty("Id");
        Run("Routine.Core.Reflection Access -> GetValue(obj)",
            () => { var _ = rprop3.GetValue(obj); },
            load
        );

        Console.WriteLine("-------");
    }

    [TestCase(100, 10)]
    [TestCase(100, 100)]
    [TestCase(100, 1000)]
    [TestCase(1000, 10)]
    [TestCase(1000, 100)]
    [TestCase(1000, 1000)]
    [Ignore("should be run manually")]
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
        var __ = _objectService.Get(new ReferenceData
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

                    var foundObj = (BusinessPerformance)_objectRepository[ord.Id];
                    var _ = new ObjectData
                    {
                        ModelId = ord.ModelId,
                        Id = foundObj.Id.ToString(CultureInfo.InvariantCulture),
                        Data = new(){
                        { "Items", new() {
                                IsList = true,
                                Values = foundObj.Items.Select(sub => new ObjectData{
                                    ModelId = subType,
                                    Id = sub.Id.ToString(CultureInfo.InvariantCulture),
                                    Display = sub.ToString(),

										#region Prop1 - Prop10
										Data = new ()
                                    {
                                        {
                                            "Prop1",
                                            new()
                                            {
                                                IsList =  false,
                                                Values = new()
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
                                            new()
                                            {
                                                IsList =  false,
                                                Values = new()
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
                                            new()
                                            {
                                                IsList =  false,
                                                Values = new()
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
                                            new()
                                            {
                                                IsList =  false,
                                                Values = new()
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
                                            new()
                                            {
                                                IsList =  false,
                                                Values = new()
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
                                            new()
                                            {
                                                IsList =  false,
                                                Values = new()
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
                                            new()
                                            {
                                                IsList =  false,
                                                Values = new()
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
                                            new()
                                            {
                                                IsList =  false,
                                                Values = new()
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
                                            new()
                                            {
                                                IsList =  false,
                                                Values = new()
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
                                            new()
                                            {
                                                IsList =  false,
                                                Values = new()
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
                    var _ = _objectService.Get(new ReferenceData
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
                    var _ = _rapp.Get(objId.ToString(CultureInfo.InvariantCulture), objType)["Items"].Get().List.Select(r => r.Display);
                }, load);
        #endregion

        Assert.LessOrEqual(engineTime / manuelTime, maxEngineOverheadRatio, "Engine over manuel is above expected");
        Assert.LessOrEqual(clientTime / engineTime, maxClientOverheadRatio, "Client over engine is above expected");
    }

    [TestCase(1000)]
    [TestCase(10000)]
    [TestCase(100000)]
    [Ignore("should be run manually")]
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
                var foundObj = (BusinessPerformance)_objectRepository[ord.Id];

                var sub = foundObj.GetSub(int.Parse("0"));
                var _ = new VariableData
                {
                    IsList = false,
                    Values = new() {
                        new()
                        {
                            Display = sub.ToString(),
                            ModelId = subType,
                            Id = sub.Id.ToString(CultureInfo.InvariantCulture),
								#region Prop1 - Prop10
								Data = new()
                            {
                                {
                                    "Prop1",
                                    new()
                                    {
                                        IsList =  false,
                                        Values = new()
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
                                    new()
                                    {
                                        IsList =  false,
                                        Values = new()
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
                                    new()
                                    {
                                        IsList =  false,
                                        Values = new()
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
                                    new()
                                    {
                                        IsList =  false,
                                        Values = new()
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
                                    new()
                                    {
                                        IsList =  false,
                                        Values = new()
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
                                    new()
                                    {
                                        IsList =  false,
                                        Values = new()
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
                                    new()
                                    {
                                        IsList =  false,
                                        Values = new()
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
                                    new()
                                    {
                                        IsList =  false,
                                        Values = new()
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
                                    new()
                                    {
                                        IsList =  false,
                                        Values = new()
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
                                    new()
                                    {
                                        IsList =  false,
                                        Values = new()
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
                var _ = _objectService.Do(ord, "GetSub", new(){
                {"index", new() {
                        IsList = false,
                        Values = new() {
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
                var rvar = _rapp.NewVar("index", _rapp.Get("0", "System.Int32"));
                var _ = _rapp.Get(objId.ToString(CultureInfo.InvariantCulture), objType).Perform("GetSub", rvar);
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
    [Ignore("should be run manually")]
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
                    "input", new()
                    {
                        IsList = false,
                        Values = Enumerable
                            .Range(0, inputCount)
                            .Select(_ =>

									#region parameter data
									new ParameterData
                                {
                                    ModelId = inputType,
                                    InitializationParameters = new ()
                                    {
                                        {
                                            "s", new()
                                            {
                                                IsList = false,
                                                Values = new()
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
                                            "i", new()
                                            {
                                                IsList = false,
                                                Values = new()
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
                                            "s2", new()
                                            {
                                                IsList = false,
                                                Values = new()
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
                                            "s3", new()
                                            {
                                                IsList = false,
                                                Values = new()
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
                                            "s4", new()
                                            {
                                                IsList = false,
                                                Values = new()
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

            var foundObj = (BusinessPerformance)_objectRepository[ord.Id];

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
                    "input", new()
                    {
                        IsList = false,
                        Values = Enumerable
                            .Range(0, inputCount)
                            .Select(_ =>

									#region parameter data
									new ParameterData
                                {
                                    ModelId = inputType,
                                    InitializationParameters = new()
                                    {
                                        {
                                            "s", new()
                                            {
                                                IsList = false,
                                                Values = new()
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
                                            "i", new()
                                            {
                                                IsList = false,
                                                Values = new()
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
                                            "s2", new()
                                            {
                                                IsList = false,
                                                Values = new()
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
                                            "s3", new()
                                            {
                                                IsList = false,
                                                Values = new()
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
                                            "s4", new()
                                            {
                                                IsList = false,
                                                Values = new()
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
            var _ = _objectService.Do(ord, "Create", parameters);
        }, load);
        #endregion

        #region client
        var clientTime = Run("client api", () =>
        {
            var rvar = _rapp.NewVarList("input",
                Enumerable
                    .Range(0, inputCount)
                    .Select(_ =>
                        _rapp.Init(inputType,
                            _rapp.NewVar("s", strIn, "System.String"),
                            _rapp.NewVar("i", intIn, "System.Int32"),
                            _rapp.NewVar("s2", strIn, "System.String"),
                            _rapp.NewVar("s3", strIn, "System.String"),
                            _rapp.NewVar("s4", strIn, "System.String")
                        )
                    )
                );
            var _ = _rapp.Get(objId.ToString(CultureInfo.InvariantCulture), objType).Perform("Create", rvar);
        }, load);

        #endregion

        Assert.LessOrEqual(engineTime / manuelTime, maxEngineOverheadRatio, "Engine over manuel is above expected");
        Assert.LessOrEqual(clientTime / engineTime, maxClientOverheadRatio, "Client over engine is above expected");
    }
}
