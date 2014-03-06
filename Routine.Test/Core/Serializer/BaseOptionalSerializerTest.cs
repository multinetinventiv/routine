using NUnit.Framework;
using Routine.Core;
using Routine.Core.Serializer;

namespace Routine.Test.Core.Serializer
{
	[TestFixture]
	public class BaseOptionalSerializerTest : CoreTestBase
	{
		public override string[] DomainTypeRootNamespaces{get{return new[]{"Routine.Test.Core.Serializer"};}}

		private class TestOptionalSerializer : BaseOptionalSerializer<TestOptionalSerializer, TypeInfo>
		{
			protected override string Serialize(TypeInfo obj)
			{
				return obj.FullName;
			}

			protected override TypeInfo Deserialize(string objString)
			{
				return objString.ToType();
			}
		}

		[Test]
		public void WhenNoWhenDelegateIsDefinedByDefaultCanSerializeReturnsTrue()
		{
			IOptionalSerializer<TypeInfo> testing = new TestOptionalSerializer();

			Assert.IsTrue(testing.CanSerialize(type.of<string>()));
		}

		[Test]
		public void WhenGivenObjectSatisfiesGivenWhenDelegateCanSerializeReturnsTrue()
		{
			IOptionalSerializer<TypeInfo> testing = new TestOptionalSerializer().SerializeWhen(t => t.CanBe<string>());

			Assert.IsTrue(testing.CanSerialize(type.of<string>()));
		}

		[Test]
		public void WhenGivenObjectDoesNotSatisfyGivenWhenDelegateCanSerializeReturnsFalse()
		{
			IOptionalSerializer<TypeInfo> testing = new TestOptionalSerializer().SerializeWhen(t => !t.CanBe<string>());

			Assert.IsFalse(testing.CanSerialize(type.of<string>()));
		}

		[Test]
		public void WhenNoWhenDelegateIsDefinedByDefaultCanDeserializeReturnsTrue()
		{			
			IOptionalSerializer<TypeInfo> testing = new TestOptionalSerializer();

			Assert.IsTrue(testing.CanDeserialize("System.String"));
		}

		[Test]
		public void WhenGivenStringSatisfiesGivenWhenDelegateCanDeserializeReturnsTrue()
		{
			IOptionalSerializer<TypeInfo> testing = new TestOptionalSerializer().DeserializeWhen(s => s.StartsWith("System."));

			Assert.IsTrue(testing.CanDeserialize("System.String"));
		}

		[Test]
		public void WhenGivenStringDoesNotSatisfyGivenWhenDelegateCanDeserializeReturnsFalse()
		{
			IOptionalSerializer<TypeInfo> testing = new TestOptionalSerializer().DeserializeWhen(s => !s.StartsWith("System."));

			Assert.IsFalse(testing.CanDeserialize("System.String"));
		}

		[Test]
		public void ValidatesBeforeSerialization()
		{
			IOptionalSerializer<TypeInfo> testing = new TestOptionalSerializer().SerializeWhen(t => t != null);

			try
			{
				testing.Serialize(null);
				Assert.Fail("exception not thrown");
			}
			catch(CannotSerializeDeserializeException) {}
		}

		[Test]
		public void ValidatesBeforeDeserialization()
		{
			IOptionalSerializer<TypeInfo> testing = new TestOptionalSerializer().DeserializeWhen(s => s != null);

			try
			{
				testing.Deserialize(null);
				Assert.Fail("exception not thrown");
			}
			catch(CannotSerializeDeserializeException) {}
		}
	}
}

