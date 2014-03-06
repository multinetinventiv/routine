using NUnit.Framework;
using Routine.Core;
using Moq;
using Routine.Core.Serializer;
using Routine.Core.CodingStyle;
using System;

namespace Routine.Test.Core.Serializer
{
	[TestFixture]
	public class MultipleSerializerTest : CoreTestBase
	{
		public override string[] DomainTypeRootNamespaces{get{return new[]{"Routine.Test.Core.Serializer"};}}

		private Mock<IOptionalSerializer<string>> serializerMock1;
		private Mock<IOptionalSerializer<string>> serializerMock2;
		private Mock<IOptionalSerializer<string>> serializerMock3;

		private MultipleSerializer<GenericCodingStyle, string> testing;
		private ISerializer<string> testingInterface;
		private MultipleSerializer<GenericCodingStyle, string> testingOther;
		//private ISerializer<string> testingOtherInterface;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			serializerMock1 = new Mock<IOptionalSerializer<string>>();
			serializerMock2 = new Mock<IOptionalSerializer<string>>();
			serializerMock3 = new Mock<IOptionalSerializer<string>>();

			testingInterface = testing = new MultipleSerializer<GenericCodingStyle, string>(new GenericCodingStyle());
			/*	testingOtherInterface =*/ testingOther = new MultipleSerializer<GenericCodingStyle, string>(new GenericCodingStyle());

			testing.Add(serializerMock1.Object)
					.Done(serializerMock2.Object);

			testingOther.Done(serializerMock3.Object);
		}

		private void SerializerSerializes(Mock<IOptionalSerializer<string>> serializerMock, string result)
		{
			serializerMock.Setup(o => o.CanSerialize(It.IsAny<string>())).Returns(true);
			serializerMock.Setup(o => o.Serialize(It.IsAny<string>())).Returns(result);

			string outResult = result;
			serializerMock.Setup(o => o.TrySerialize(It.IsAny<string>(), out outResult)).Returns(true);
		}

		private void SerializerDeserializes(Mock<IOptionalSerializer<string>> serializerMock, string result)
		{
			serializerMock.Setup(o => o.CanDeserialize(It.IsAny<string>())).Returns(true);
			serializerMock.Setup(o => o.Deserialize(It.IsAny<string>())).Returns(result);

			string outResult = result;
			serializerMock.Setup(o => o.TryDeserialize(It.IsAny<string>(), out outResult)).Returns(true);
		}

		private void SerializerCannotSerialize(Mock<IOptionalSerializer<string>> serializerMock, Exception reason)
		{
			serializerMock.Setup(o => o.CanSerialize(It.IsAny<string>())).Returns(true);
			serializerMock.Setup(o => o.Serialize(It.IsAny<string>())).Throws(reason);

			string outResult;
			serializerMock.Setup(o => o.TrySerialize(It.IsAny<string>(), out outResult)).Throws(reason);
		}

		private void SerializerCannotDeserialize(Mock<IOptionalSerializer<string>> serializerMock, Exception reason)
		{
			serializerMock.Setup(o => o.CanDeserialize(It.IsAny<string>())).Returns(true);
			serializerMock.Setup(o => o.Deserialize(It.IsAny<string>())).Throws(reason);

			string outResult;
			serializerMock.Setup(o => o.TryDeserialize(It.IsAny<string>(), out outResult)).Throws(reason);
		}

		[Test]
		public void SerializesUsingTheFirstSerializerThatCanSerialize()
		{
			SerializerSerializes(serializerMock2, "serializer2");

			Assert.AreEqual("serializer2", testingInterface.Serialize("any"));
		}

		[Test]
		public void DeserializesUsingTheFirstSerializerThatCanDeserialize()
		{
			SerializerDeserializes(serializerMock2, "serializer2");

			Assert.AreEqual("serializer2", testingInterface.Deserialize("any"));
		}

		[Test]
		public void ThrowsExceptionWhenNoSerializerCanSerialize()
		{
			try
			{
				testingInterface.Serialize("dummy");
				Assert.Fail("exception not thrown");
			}
			catch(CannotSerializeDeserializeException ex)
			{
				Assert.IsTrue(ex.Message.Contains("dummy"), ex.Message);
			}
		}

		[Test]
		public void ThrowsExceptionWhenNoSerializerCanDeserialize()
		{
			try
			{
				testingInterface.Deserialize("dummy");
				Assert.Fail("exception not thrown");
			}
			catch(CannotSerializeDeserializeException ex)
			{
				Assert.IsTrue(ex.Message.Contains("dummy"), ex.Message);
			}
		}

		[Test]
		public void ExceptionOnSerializationAndDeserializationFailCanBeOverridden()
		{
			object obj = "dummy";
			var expected = new CannotSerializeDeserializeException("generic message");
			testing.OnFailThrow(expected);

			try
			{
				testingInterface.Serialize((string)obj);
				Assert.Fail("exception not thrown");
			}
			catch(CannotSerializeDeserializeException actual)
			{
				Assert.AreSame(expected, actual);
			}
		}

		[Test]
		public void ExceptionOnSerializationFailCanBeOverridden()
		{
			testing.OnSerializationFailThrow(o => new CannotSerializeDeserializeException("obj is -> " + o));

			try
			{
				testingInterface.Serialize("dummy");
				Assert.Fail("exception not thrown");
			}
			catch(CannotSerializeDeserializeException ex)
			{
				Assert.IsTrue(ex.Message.Contains("obj is -> "), ex.Message);
			}
		}

		[Test]
		public void ExceptionOnDeserializationFailCanBeOverridden()
		{
			testing.OnDeserializationFailThrow(o => new CannotSerializeDeserializeException("obj is -> " + o));

			try
			{
				testingInterface.Deserialize("dummy");
				Assert.Fail("exception not thrown");
			}
			catch(CannotSerializeDeserializeException ex)
			{
				Assert.IsTrue(ex.Message.Contains("obj is -> "), ex.Message);
			}
		}

		[Test]
		public void AnyExceptionDuringSerializationIsWrappedWithCannotSerializeException()
		{
			SerializerCannotSerialize(serializerMock1, new Exception("inner exception"));

			try
			{
				testingInterface.Serialize("dummy");
				Assert.Fail("exception not thrown");
			}
			catch(CannotSerializeDeserializeException ex)
			{
				Assert.AreEqual("inner exception", ex.InnerException.Message);
			}
		}

		[Test]
		public void AnyExceptionDuringDeserializationIsWrappedWithCannotSerializeException()
		{
			SerializerCannotDeserialize(serializerMock1, new Exception("inner exception"));

			try
			{
				testingInterface.Deserialize("dummy");
				Assert.Fail("exception not thrown");
			}
			catch(CannotSerializeDeserializeException ex)
			{
				Assert.AreEqual("inner exception", ex.InnerException.Message);
			}
		}

		[Test]
		public void WhenACannotSerializeExceptionIsThrownDuringSerializationItIsNotWrapped()
		{
			var expected = new CannotSerializeDeserializeException("test");

			SerializerCannotSerialize(serializerMock1, expected);

			try
			{
				testingInterface.Serialize("dummy");
				Assert.Fail("exception not thrown");
			}
			catch(CannotSerializeDeserializeException ex)
			{
				Assert.AreSame(expected, ex);
			}
		}

		[Test]
		public void WhenACannotSerializeExceptionIsThrownDuringDeserializationItIsNotWrapped()
		{
			var expected = new CannotSerializeDeserializeException("test");
			SerializerCannotDeserialize(serializerMock1, expected);

			try
			{
				testingInterface.Deserialize("dummy");
				Assert.Fail("exception not thrown");
			}
			catch(CannotSerializeDeserializeException ex)
			{
				Assert.AreSame(expected, ex);
			}
		}

		[Test]
		public void MergeAddsGivenSerializersInTheGivenOrder()
		{
			SerializerSerializes(serializerMock3, "serializer3");

			testing.Merge(testingOther);

			Assert.AreEqual("serializer3", testingInterface.Serialize("dummy"));
		}
	}
}

