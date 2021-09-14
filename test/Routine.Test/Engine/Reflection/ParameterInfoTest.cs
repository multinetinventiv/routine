using NUnit.Framework;
using Routine.Engine.Reflection;
using Routine.Test.Engine.Reflection.Domain;

namespace Routine.Test.Engine.Reflection
{
    [TestFixture]
    public class ParameterInfoTest : ReflectionTestBase
    {
        private System.Reflection.ParameterInfo parameterInfo;
        private ParameterInfo testing;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            parameterInfo = typeof(ReflectedParameter).GetMethod("AMethod")?.GetParameters()[0];
            testing = type.of<ReflectedParameter>().GetMethod("AMethod").GetParameters()[0];
        }

        [Test]
        public void System_ParameterInfo_is_wrapped_by_Routine_ParameterInfo()
        {
            Assert.AreEqual(parameterInfo.Name, testing.Name);
            Assert.AreSame(parameterInfo.ParameterType, testing.ParameterType.GetActualType());
            Assert.AreEqual(parameterInfo.Position, testing.Position);
        }

        [Test]
        public void Routine_ParameterInfo_caches_wrapped_properties()
        {
            Assert.AreSame(testing.Name, testing.Name);
            Assert.AreSame(testing.ParameterType, testing.ParameterType);
            Assert.AreSame(testing.GetCustomAttributes(), testing.GetCustomAttributes());
        }

        [Test]
        public void Routine_ParameterInfo_lists_custom_attributes_with_inherit_behaviour()
        {
            var actual = testing.GetCustomAttributes();

            Assert.AreEqual(1, actual.Length);
            Assert.IsInstanceOf<TestClassAttribute>(actual[0]);
        }

        [Test]
        public void Extension_Has()
        {
            Assert.IsTrue(testing.Has<TestClassAttribute>());
            Assert.IsTrue(testing.Has(type.of<TestClassAttribute>()));
        }
    }
}

