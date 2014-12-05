using NUnit.Framework;
using Routine.Engine.Virtual;

namespace Routine.Test.Engine.Virtual
{
	[TestFixture]
	public class VirtualObjectTest
	{
		#region Helpers

		private VirtualType VirtualType()
		{
			return VirtualType("type");
		}

		private VirtualType VirtualType(string name)
		{
			return VirtualType(name, "namespace");
		}

		private VirtualType VirtualType(string name, string @namespace)
		{
			return BuildRoutine.VirtualType().FromBasic().Name.Set(name).Namespace.Set(@namespace);
		}

		#endregion

		[Test]
		public void Has_id_and_type()
		{
			var testing = new VirtualObject("virtual", VirtualType("type"));

			Assert.AreEqual("type", testing.Type.Name);
			Assert.AreEqual("virtual", testing.Id);
		}

		[Test]
		public void To_string_uses_type_to_get_string_value()
		{
			var testing = new VirtualObject("virtual", VirtualType().ToStringMethod.Set(o => o.Id));

			Assert.AreEqual("virtual", testing.ToString());
		}
	}
}