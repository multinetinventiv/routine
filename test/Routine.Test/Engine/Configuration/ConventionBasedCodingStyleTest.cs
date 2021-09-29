using System;
using NUnit.Framework;
using Routine.Engine;

namespace Routine.Test.Engine.Configuration
{
    [TestFixture]
    public class ConventionBasedCodingStyleTest
    {
        [Test]
        public void When_configuring_nullable_types__type_and_module_names_come_from_the_type_that_is_nullable()
        {
            var testing = BuildRoutine.CodingStyle().FromBasic() as ICodingStyle;

            Assert.AreEqual("Int32?", testing.GetName(type.of<int?>()));
            Assert.AreEqual("System", testing.GetModule(type.of<int?>()));

            Assert.AreEqual("DateTime?", testing.GetName(type.of<DateTime?>()));
            Assert.AreEqual("System", testing.GetModule(type.of<DateTime?>()));

            Assert.AreEqual("Text?", testing.GetName(type.of<Text?>()));
            Assert.AreEqual("Routine.Test", testing.GetModule(type.of<Text?>()));
        }

        [Test]
        public void When_adding_a_value_type__nullable_version_is_added_automatically()
        {
            var testing = BuildRoutine.CodingStyle().FromBasic().AddTypes(typeof(int)) as ICodingStyle;

            Assert.IsTrue(testing.ContainsType(type.of<int?>()));
        }

        [Test]
        public void When_a_ref_struct_is_added__it_is_ignored_automatically()
        {
            Assert.Fail("not implemented");
        }

        [Test]
        public void When_a_record_is_added__it_can_be_configured_like_any_other_class()
        {
            Assert.Fail("not implemented");
        }
    }
}