using NUnit.Framework;
using Routine.Core;
using Routine.Core.Serializer;

namespace Routine.Test.Core.Serializer
{
	[TestFixture]
	public class DelegateSerializerTest : CoreTestBase
	{
		public override string[] DomainTypeRootNamespaces{get{return new[]{"Routine.Test.Core.Serializer"};}}

		[Test]
		public void CannotSerializeWhenNoSerializerDelegateWasDefined()
		{
			IOptionalSerializer<string> testing = new DelegateSerializer<string>();

			Assert.IsFalse(testing.CanSerialize("any"));
		}

		[Test]
		public void CannotDeserializeWhenNoDeserializerDelegateWasDefined()
		{
			IOptionalSerializer<string> testing = new DelegateSerializer<string>();

			Assert.IsFalse(testing.CanDeserialize("any"));
		}

		[Test]
		public void SerializesGivenObjectUsingGivenSerializerDelegate()
		{
			IOptionalSerializer<string> testing = new DelegateSerializer<string>().SerializeBy(s => s.Replace(".", "-"));

			Assert.AreEqual("a-b", testing.Serialize("a.b"));
		}

		[Test]
		public void DeserializesGivenObjectUsingGivenDeserializerDelegate()
		{
			IOptionalSerializer<string> testing = new DelegateSerializer<string>().DeserializeBy(s => s.Replace("-", "."));

			Assert.AreEqual("a.b", testing.Deserialize("a-b"));
		}
	}
}

