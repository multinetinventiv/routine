using Routine.Core.Configuration;
using Routine.Core;
using Routine.Engine;
using Routine.Test.Engine.Domain.ObjectServiceTest_GetObjectData;
using Routine.Test.Engine.Stubs.ObjectServiceInvokers;

#region Test Model

namespace Routine.Test.Engine.Domain.ObjectServiceTest_GetObjectData
{
    public interface IBusinessDataWithNoImplementor { }

    public interface IBusinessData
    {
        IBusinessData SubData { get; }
    }

    public class BusinessData : IBusinessData
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public List<string> Items { get; set; }
        public List<BusinessData> SubDatas { get; set; }
        public NotLocatable NotLocatable { get; set; }
        public int? NullableInt { get; set; }

        public void Operation() { }

        IBusinessData IBusinessData.SubData => new BusinessData { Id = $"sub_{Id}", Title = $"Sub {Title}" };

        public override string ToString() => Title;
    }

    public readonly struct BusinessValue
    {
        public static BusinessValue Parse(string value) => new(value);

        private readonly string _value;
        public BusinessValue(string value)
        {
            _value = value;
        }

        public override string ToString() => _value;
    }

    public class NotLocatable : IBusinessData
    {
        public string Title { get; set; }
        public IBusinessData SubData { get; set; }
    }
}

#endregion

namespace Routine.Test.Engine
{
    [TestFixture(typeof(Async))]
    [TestFixture(typeof(Sync))]
    public class ObjectServiceTest_GetObjectData<TObjectServiceInvoker> : ObjectServiceTestBase
        where TObjectServiceInvoker : IObjectServiceInvoker, new()
    {
        #region Setup & Helpers

        private const string ACTUAL_OMID = "Test.BusinessData";
        private const string VIEW_OMID = "Test.IBusinessData";
        private const string VIEW_WITH_NO_IMPLEMENTOR_OMID = "Test.IBusinessDataWithNoImplementor";
        private const string VALUE_OMID = "Test.BusinessValue";

        protected override string DefaultModelId => ACTUAL_OMID;
        protected override string RootNamespace => "Routine.Test.Engine.Domain.ObjectServiceTest_GetObjectData";

        private IObjectServiceInvoker _invoker;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            _codingStyle
                .Module.Set("Test", t => t.Namespace.StartsWith("Routine.Test"))
                .ValueExtractor.Set(c => c.ValueByProperty(m => m.Returns<string>("Title")))
                ;

            _invoker = new TObjectServiceInvoker();
        }

        #endregion

        [Test]
        public void Object_is_located_via_configured_locator_and_its_id_is_extracted_using_corresponding_extractor()
        {
            AddToRepository(new BusinessData { Id = "obj" });

            var actual = _invoker.InvokeGet(_testing, Id("obj"));

            Assert.AreEqual("obj", actual.Id);
        }

        [Test]
        public void When_given_actual_model_does_not_have_a_converter_for_its_view_model__configuration_exception_is_thrown_with_cannot_convert_exception_in_it()
        {
            AddToRepository(new BusinessData { Id = "obj" });

            try
            {
                _invoker.InvokeGet(_testing, Id("obj", ACTUAL_OMID, VIEW_WITH_NO_IMPLEMENTOR_OMID));
                Assert.Fail("exception not thrown");
            }
            catch (ConfigurationException ex)
            {
                Assert.IsInstanceOf<CannotConvertException>(ex.InnerException);
            }
        }

        [Test]
        public void Locating_and_id_extraction_is_done_via_actual_model_id__even_if_there_exists_a_view_model_id()
        {
            _codingStyle
                .IdExtractor.Set(c => c.Id(i => i.Constant("wrong")).When(type.of<IBusinessData>()))
                .ValueExtractor.Set(c => c.Value(v => v.Constant("dummy")).When(type.of<IBusinessData>()))
                .Locator.Set(c => c.Locator(l => l.Constant(new BusinessData { Id = "wrong" })).When(type.of<IBusinessData>()))
            ;

            AddToRepository(new BusinessData { Id = "obj" });

            var actual = _invoker.InvokeGet(_testing, Id("obj", ACTUAL_OMID, VIEW_OMID));

            Assert.AreEqual("obj", actual.Id);
        }

        [Test]
        public void Value_is_extracted_using_corresponding_extractor()
        {
            AddToRepository(new BusinessData { Id = "obj", Title = "Obj Title" });

            var actual = _invoker.InvokeGet(_testing, Id("obj"));

            Assert.AreEqual("Obj Title", actual.Display);
        }

        [Test]
        public void Value_is_extracted_using_corresponding_extractor_of_view_types()
        {
            _codingStyle
                .ValueExtractor.Set(c => c.Value(v => v.Constant("view value")).When(type.of<IBusinessData>()))
            ;

            AddToRepository(new BusinessData { Id = "obj", Title = "Obj Title" });

            var actual = _invoker.InvokeGet(_testing, Id("obj", ACTUAL_OMID, VIEW_OMID));

            Assert.AreEqual("view value", actual.Display);
        }

        [Test]
        public void When_view_type_does_not_have_a_value_extractor__extractor_of_actual_type_is_used()
        {
            _codingStyle
                .ValueExtractor.Set(null, type.of<IBusinessData>())
            ;

            AddToRepository(new BusinessData { Id = "obj", Title = "Obj Title" });

            var actual = _invoker.InvokeGet(_testing, Id("obj", ACTUAL_OMID, VIEW_OMID));

            Assert.AreEqual("Obj Title", actual.Display);
        }

        [Test]
        public void Value_is_id_when_model_is_value_type()
        {
            var _ = _testing.ApplicationModel;

            var actual = _invoker.InvokeGet(_testing, Id("sample", VALUE_OMID));

            Assert.AreEqual("sample", actual.Id);
            Assert.AreEqual("sample", actual.Display);
        }

        [Test]
        public void When_value_extractor_of_view_type_is_used__view_target_is_used()
        {
            var obj = new BusinessData { Id = "obj" };
            var objConverted = new BusinessData { Id = "obj_converted", Title = "Converted Obj Title" };

            _codingStyle
                .Override(cs => cs
                    .Converters.Add(c => c.Convert(cb => cb.By(type.of<IBusinessData>(), (_, _) => objConverted))
                                         .When(type.of<BusinessData>()))
                    .ValueExtractor.Set(c => c.Value(v => v.By(o => $"{o}"))
                                              .When(type.of<IBusinessData>()))
                )
            ;

            AddToRepository(obj);

            var actual = _invoker.InvokeGet(_testing, Id("obj", ACTUAL_OMID, VIEW_OMID));

            Assert.AreEqual("Converted Obj Title", actual.Display);
        }

        [Test]
        public void When_value_extractor_of_actual_type_is_used__actual_target_is_used()
        {
            var obj = new BusinessData { Id = "obj", Title = "Obj Title" };
            var objConverted = new BusinessData { Id = "obj_converted" };

            _codingStyle
                .Converters.Add(c => c.Convert(cb => cb.ToConstant(objConverted))
                                     .When(type.of<BusinessData>()))
                .ValueExtractor.Set(null, type.of<IBusinessData>())
            ;

            AddToRepository(obj);

            var actual = _invoker.InvokeGet(_testing, Id("obj", ACTUAL_OMID, VIEW_OMID));

            Assert.AreEqual("Obj Title", actual.Display);
        }

        [Test]
        public void Datas_are_fetched_using_given_view_model_id()
        {
            AddToRepository(new BusinessData { Id = "obj", Title = "Obj Title" });

            var actual = _invoker.InvokeGet(_testing, Id("obj", ACTUAL_OMID, VIEW_OMID));

            Assert.IsTrue(actual.Data.ContainsKey("SubData"));
        }

        [Test]
        public void Datas_are_fetched_using_view_target()
        {
            var obj = new BusinessData { Id = "obj" };
            var objConverted = new BusinessData { Id = "obj_converted" };

            _codingStyle
                .Override(cs => cs
                    .Converters.Add(c => c.Convert(cb => cb.By(type.of<IBusinessData>(), (_, _) => objConverted))
                                         .When(type.of<BusinessData>()))
                )
            ;

            AddToRepository(obj);

            var actual = _invoker.InvokeGet(_testing, Id("obj", ACTUAL_OMID, VIEW_OMID));
            var actualSubdata = actual.Data["SubData"].Values[0];

            Assert.AreEqual("sub_obj_converted", actualSubdata.Id);
        }

        [Test]
        public void Data_display_values_are_fetched_along_with_their_ids()
        {
            AddToRepository(new BusinessData { Id = "obj", Title = "Obj Title" });

            var actual = _invoker.InvokeGet(_testing, Id("obj", ACTUAL_OMID, VIEW_OMID));
            var actualData = actual.Data["SubData"];

            Assert.AreEqual("sub_obj", actualData.Values[0].Id);
            Assert.AreEqual("Sub Obj Title", actualData.Values[0].Display);
        }

        [Test]
        public void Null_objects_represented_as_empty_variable_data()
        {
            AddToRepository(new BusinessData { Id = "obj", Title = null, NullableInt = null });

            var actual = _invoker.InvokeGet(_testing, Id("obj"));
            var title = actual.Data["Title"];
            var nullableInt = actual.Data["NullableInt"];

            Assert.AreEqual(new VariableData(), title);
            Assert.AreEqual(new VariableData(), nullableInt);
        }

        [Test]
        public void Datas_are_fetched_eagerly_when_configured_so()
        {
            _codingStyle
                .DataFetchedEagerly.Set(true, m => m.Name == "SubDatas")
                ;

            AddToRepository(new BusinessData { Id = "sub1", Items = new List<string> { "sub1_1", "sub1_2" } });
            AddToRepository(new BusinessData { Id = "sub2", Items = new List<string> { "sub2_1", "sub2_2" } });
            AddToRepository(new BusinessData
            {
                Id = "obj",
                SubDatas = new()
                {
                    _objectRepository["sub1"] as BusinessData,
                    _objectRepository["sub2"] as BusinessData
                }
            });

            var actual = _invoker.InvokeGet(_testing, Id("obj"));
            var actualData = actual.Data["SubDatas"];

            Assert.AreEqual("sub1_1", actualData.Values[0].Data["Items"].Values[0].Id);
            Assert.AreEqual("sub1_2", actualData.Values[0].Data["Items"].Values[1].Id);
            Assert.AreEqual("sub2_1", actualData.Values[1].Data["Items"].Values[0].Id);
            Assert.AreEqual("sub2_2", actualData.Values[1].Data["Items"].Values[1].Id);
        }

        [Test]
        public void When_data_type_cannot_be_located__it_is_fetched_eagerly_no_matter_what_configuration_was_given()
        {
            _codingStyle.DataFetchedEagerly.Set(false, m => m.Returns<NotLocatable>("NotLocatable"));

            AddToRepository(new BusinessData { Id = "obj", NotLocatable = new NotLocatable { Title = "fetched eagerly" } });

            var actual = _invoker.InvokeGet(_testing, Id("obj"));
            var actualDataValue = actual.Data["NotLocatable"].Values[0];

            Assert.IsTrue(actualDataValue.Data.ContainsKey("Title"), "Member was not fetched eagerly");
            Assert.AreEqual("fetched eagerly", actualDataValue.Data["Title"].Values[0].Id);
            Assert.AreEqual(string.Empty, actualDataValue.Id);
        }

        [Test]
        public void When_data_type_is_locatable_but_actual_type_of_that_data_is_not_locatable__it_is_fetched_eagerly_no_matter_what_configuration_was_given()
        {
            _codingStyle.DataFetchedEagerly.Set(false, m => m.Returns<NotLocatable>("NotLocatable"));

            var subBusinessdata = new BusinessData { Id = "sub_businessdata" };
            var subNotlocatable = new NotLocatable { SubData = subBusinessdata, Title = "sub not locatable" };
            var notlocatable = new NotLocatable { SubData = subNotlocatable, Title = "notlocatable" };
            var rootBusinessdata = new BusinessData { Id = "root_businessdata", NotLocatable = notlocatable };

            AddToRepository(subBusinessdata);
            AddToRepository(rootBusinessdata);

            var actualRootBusinessdata = _invoker.InvokeGet(_testing, Id("root_businessdata"));
            var actualNotlocatable = actualRootBusinessdata.Data["NotLocatable"].Values[0];

            var actualSubNotlocatable = actualNotlocatable.Data["SubData"].Values[0];

            Assert.AreEqual("sub not locatable", actualSubNotlocatable.Display);
            Assert.IsTrue(actualSubNotlocatable.Data.ContainsKey("SubData"), "Member was not fetched eagerly, instance was not locatable and supposed to be fetched eagerly");

            var actualSubBusinessdata = actualSubNotlocatable.Data["SubData"].Values[0];

            Assert.AreEqual("sub_businessdata", actualSubBusinessdata.Id);
            Assert.IsFalse(actualSubBusinessdata.Data.ContainsKey("SubData"), "Member was fetched eagerly, instance was locatable and not supposed to be fetched eagerly");
        }

        [Test]
        public void Eager_fetching_is_allowed_to_a_max_depth_to_prevent_infinite_recursion()
        {
            _codingStyle.MaxFetchDepth.Set(2);

            var notlocatable2 = new NotLocatable { Title = "notlocatable2" };
            var notlocatable1 = new NotLocatable { Title = "notlocatable1", SubData = notlocatable2 };
            var rootBusinessdata = new BusinessData { Id = "root_businessdata", NotLocatable = notlocatable1 };

            AddToRepository(rootBusinessdata);

            Assert.Throws<MaxFetchDepthExceededException>(() => _invoker.InvokeGet(_testing, Id("root_businessdata")));
        }
    }
}
