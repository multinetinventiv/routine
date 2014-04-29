using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Routine.Core.Service;

namespace Routine.Test.Core.Service
{
	[TestFixture]
	public class SerializationExtensionsTest
	{
		[Test]
		public void When_serializing__ObjectReferenceData_is_serialized_as_null_when_it_is_null()
		{
			var testing = new ObjectReferenceData { IsNull = true };

			Assert.IsNull(testing.ToSerializable());

			testing = new ObjectReferenceData { IsNull = true, Id = "id", ActualModelId = "amid", ViewModelId = "vmid" };

			Assert.IsNull(testing.ToSerializable());
		}

		[Test]
		public void When_serializing__ObjectReferenceData_is_serialized_as_one_string()
		{
			var testing = new ObjectReferenceData { Id = "id", ActualModelId = "amid", ViewModelId = "vmid" };

			Assert.AreEqual("id#amid#vmid", testing.ToSerializable());
		}

		[Test]
		public void When_serializing__ObjectReferenceData_serializes_only_ActualModelId_when_ViewModelId_is_null_or_empty()
		{
			var testing = new ObjectReferenceData { Id = "id", ActualModelId = "amid"};

			Assert.AreEqual("id#amid", testing.ToSerializable());

			testing = new ObjectReferenceData { Id = "id", ActualModelId = "amid", ViewModelId = "" };

			Assert.AreEqual("id#amid", testing.ToSerializable());
		}

		[Test]
		public void When_serializing__ObjectReferenceData_serializes_only_ActualModelId_when_it_is_the_same_with_ViewModelId()
		{
			var testing = new ObjectReferenceData { Id = "id", ActualModelId = "amid", ViewModelId = "amid" };

			Assert.AreEqual("id#amid", testing.ToSerializable());
		}

		[Test]
		public void When_serializing__ObjectData_is_serialized_as_null_when_its_reference_is_null()
		{
			var testing = new ObjectData { Reference = new ObjectReferenceData { IsNull = true } };

			Assert.IsNull(testing.ToSerializable());
		}

		[Test]
		public void When_serializing__ObjectData_serializes_only_reference_data_and_value_when_it_does_not_have_any_members()
		{
			var testing = new ObjectData
			{
				Reference = new ObjectReferenceData { Id = "id", ActualModelId = "amid" },
				Value = "value"
			};

			var actual = testing.ToSerializable();
			Assert.IsInstanceOf<Dictionary<string, object>>(actual);
			
			var dict = actual as Dictionary<string, object>;
			Assert.AreEqual(2, dict.Count, "Dictionary should contain only Ref and Value in this case.");

			Assert.IsTrue(dict.ContainsKey("Ref"));
			Assert.IsTrue(dict.ContainsKey("Value"));

			Assert.IsInstanceOf<string>(dict["Ref"]);
			Assert.IsInstanceOf<string>(dict["Value"]);

			Assert.AreEqual("id#amid", dict["Ref"] as string);
			Assert.AreEqual("value", dict["Value"] as string);
		}

		[Test]
		public void When_serializing__given_that_ObjectData_does_not_have_any_members__it_serializes_directly_reference_string_when_value_is_the_same_with_id_or_is_null_or_empty()
		{
			var testing = new ObjectData
			{
				Reference = new ObjectReferenceData { Id = "id", ActualModelId = "amid" },
				Value = "id"
			};

			var actual = testing.ToSerializable();
			Assert.IsInstanceOf<string>(actual);
			Assert.AreEqual("id#amid", actual as string);

			testing.Value = null;
			actual = testing.ToSerializable();
			Assert.IsInstanceOf<string>(actual);
			Assert.AreEqual("id#amid", actual as string);

			testing.Value = "";
			actual = testing.ToSerializable();
			Assert.IsInstanceOf<string>(actual);
			Assert.AreEqual("id#amid", actual as string);
		}

		[Test]
		public void When_serializing__ValueData_serializes_directly_its_only_ObjectData_when_it_is_not_a_list()
		{
			var testing = new ValueData { IsList = false };
			testing.Values.Add(new ObjectData { Reference = new ObjectReferenceData { Id = "id", ActualModelId = "amid" } });

			var actual = testing.ToSerializable();
			Assert.IsInstanceOf<string>(actual);
			Assert.AreEqual("id#amid", actual as string);

			testing.Values[0].Value = "value";
			actual = testing.ToSerializable();
			Assert.IsInstanceOf<Dictionary<string, object>>(actual);
			
			testing.Values[0].Reference.IsNull = true;
			actual = testing.ToSerializable();
			Assert.IsNull(actual);
		}

		[Test]
		public void When_serializing__ValueData_is_serialized_as_null_when_it_is_not_a_list_and_it_does_not_have_any_value()
		{
			var testing = new ValueData { IsList = false };

			var actual = testing.ToSerializable();
			Assert.IsNull(actual);
		}

		[Test]
		public void When_serializing__ValueData_is_serialized_as_List_when_it_is_list()
		{
			var testing = new ValueData { IsList = true };
			testing.Values.Add(new ObjectData { Reference = new ObjectReferenceData { Id = "id", ActualModelId = "amid" } });
			testing.Values.Add(new ObjectData { Reference = new ObjectReferenceData { Id = "id2", ActualModelId = "amid2" } });

			var actual = testing.ToSerializable();
			Assert.IsInstanceOf<List<object>>(actual);

			var list = actual as List<object>;
			Assert.AreEqual(2, list.Count);
			Assert.AreEqual("id#amid", list[0] as string);
			Assert.AreEqual("id2#amid2", list[1] as string);
		}

		[Test]
		public void When_serializing__ValueData_is_serialized_as_an_empty_list_when_it_is_an_empty_list()
		{
			var testing = new ValueData { IsList = true };

			var actual = testing.ToSerializable();
			Assert.IsInstanceOf<List<object>>(actual);

			var list = actual as List<object>;
			Assert.AreEqual(0, list.Count);
		}

		[Test]
		public void When_serializing__ValueData_still_contains_null_references_when_it_is_serialized_as_list()
		{
			var testing = new ValueData { IsList = true };
			testing.Values.Add(new ObjectData { Reference = new ObjectReferenceData { Id = "id", ActualModelId = "amid" } });
			testing.Values.Add(new ObjectData { Reference = new ObjectReferenceData { IsNull = true } });

			var actual = testing.ToSerializable();
			Assert.IsInstanceOf<List<object>>(actual);

			var list = actual as List<object>;
			Assert.AreEqual(2, list.Count);
			Assert.AreEqual("id#amid", list[0] as string);
			Assert.IsNull(list[1]);
		}

		[Test]
		public void When_serializing__ObjectData_serializes_its_members_in_a_separate_dictionary()
		{
			var testing = new ObjectData
			{
				Reference = new ObjectReferenceData { Id = "id", ActualModelId = "amid" },
				Value = "value",
				Members = new Dictionary<string, ValueData>
				{
					{"mmid1", new ValueData{Values=new List<ObjectData>{new ObjectData{Reference=new ObjectReferenceData{Id="mmid1_id",ActualModelId="amid"}}}}},
					{"mmid2", new ValueData{Values=new List<ObjectData>{new ObjectData{Reference=new ObjectReferenceData{Id="mmid2_id",ActualModelId="amid"}}}}}
				}
			};

			var actual = testing.ToSerializable();
			Assert.IsInstanceOf<Dictionary<string, object>>(actual);
			
			var dict = actual as Dictionary<string, object>;
			Assert.IsTrue(dict.ContainsKey("Ref"));
			Assert.IsTrue(dict.ContainsKey("Value"));
			Assert.IsTrue(dict.ContainsKey("Members"));

			Assert.IsInstanceOf<Dictionary<string, object>>(dict["Members"]);

			var members = dict["Members"] as Dictionary<string, object>;
			Assert.IsTrue(members.ContainsKey("mmid1"));
			Assert.IsTrue(members.ContainsKey("mmid2"));

			Assert.IsInstanceOf<string>(members["mmid1"]);
			Assert.IsInstanceOf<string>(members["mmid2"]);

			var mmid1 = members["mmid1"] as string;
			var mmid2 = members["mmid2"] as string;

			Assert.AreEqual("mmid1_id#amid", mmid1);
			Assert.AreEqual("mmid2_id#amid", mmid2);
		}

		[Test]
		public void When_serializing__ObjectData_does_not_serialize_value_when_it_has_members_and_its_value_is_null_or_empty_or_same_with_reference_id()
		{
			var testing = new ObjectData
			{
				Reference = new ObjectReferenceData { Id = "id", ActualModelId = "amid" },
				Members = new Dictionary<string, ValueData>
				{
					{"mmid1", new ValueData{Values=new List<ObjectData>{new ObjectData{Reference=new ObjectReferenceData{Id="mmid1_id",ActualModelId="amid"}}}}}
				}
			};

			var actual = testing.ToSerializable();
			var dict = actual as Dictionary<string, object>;
			Assert.IsFalse(dict.ContainsKey("Value"));
		}

		[Test]
		public void When_deserializing__ObjectReferenceData_deserializes_reference_string()
		{
			var expected = new ObjectReferenceData { Id = "id", ActualModelId = "amid", ViewModelId = "vmid" };
			var actual = SerializationExtensions.DeserializeObjectReferenceData("id#amid#vmid");

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void When_deserializing__ObjectReferenceData_uses_ActualModelId_as_ViewModelId_when_ValueModelId_is_not_specified()
		{
			var expected = new ObjectReferenceData { Id = "id", ActualModelId = "amid", ViewModelId = "amid" };
			var actual = SerializationExtensions.DeserializeObjectReferenceData("id#amid");

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void When_deserializing__ObjectReferenceData_sets_IsNull_when_given_object_is_null()
		{
			var expected = new ObjectReferenceData { IsNull = true };
			var actual = SerializationExtensions.DeserializeObjectReferenceData(null);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void When_deserializing__ObjectReferenceData_throws_ArgumentException_when_given_object_is_not_a_string()
		{
			Assert.Throws<ArgumentException>(() => SerializationExtensions.DeserializeObjectReferenceData(new object()));
		}

		[Test]
		public void When_deserializing__ObjectReferenceData_throws_ArgumentException_when_ActualModelId_is_not_specified()
		{
			Assert.Throws<ArgumentException>(() => SerializationExtensions.DeserializeObjectReferenceData("id"));
		}

		[Test]
		public void When_deserializing__ObjectData_deserializes_Dictionary()
		{
			var expected = new ObjectData 
			{ 
				Reference = new ObjectReferenceData { Id = "id", ActualModelId = "amid", ViewModelId = "amid" }, 
				Value = "value" 
			};

			var actual = SerializationExtensions.DeserializeObjectData(
				new Dictionary<string, object> { { "Ref", "id#amid" }, { "Value", "value" } });

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void When_deserializing__ObjectData_sets_Value_with_reference_id_if_value_was_not_given()
		{
			var expected = new ObjectData
			{
				Reference = new ObjectReferenceData { Id = "id", ActualModelId = "amid", ViewModelId = "amid" },
				Value = "id"
			};

			var actual = SerializationExtensions.DeserializeObjectData(new Dictionary<string, object> { { "Ref", "id#amid" } });

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void When_deserializing__ObjectData_deserializes_null_to_an_ObjectData_instance_having_a_null_reference()
		{
			var expected = new ObjectData { Reference = new ObjectReferenceData { IsNull = true } };

			var actual = SerializationExtensions.DeserializeObjectData(null);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void When_deserializing__ObjectData_deserializes_string_to_an_ObjectData_instance_having_a_reference_with_the_given_string()
		{
			var expected = new ObjectData 
			{ 
				Reference = new ObjectReferenceData { Id = "id", ActualModelId = "amid", ViewModelId = "amid" }, 
				Value = "id" 
			};

			var actual = SerializationExtensions.DeserializeObjectData("id#amid");

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void When_deserializing__ObjectData_throws_ArgumentException_when_given_object_is_not_null__string_or_dictionary()
		{
			Assert.Throws<ArgumentException>(() => SerializationExtensions.DeserializeObjectReferenceData(new object()));
		}

		[Test]
		public void When_deserializing__ObjectData_throws_ArgumentException_when_given_dictionary_does_not_contain_reference_string()
		{
			Assert.Throws<ArgumentException>(() => SerializationExtensions.DeserializeObjectReferenceData(new Dictionary<string, object>()));
		}

		[Test]
		public void When_deserializing__ObjectData_throws_ArgumentException_when_given_value_is_not_a_string()
		{
			Assert.Throws<ArgumentException>(() => SerializationExtensions.DeserializeObjectData(
				new Dictionary<string, object> { { "Ref", "id#amid" }, { "Value", new object() } }));
		}

		[Test]
		public void When_deserializing__ValueData_deserializes_List()
		{
			var expected = new ValueData
			{
				IsList = true,
				Values = new List<ObjectData>{
					new ObjectData
					{
						Reference = new ObjectReferenceData { Id = "id", ActualModelId = "amid", ViewModelId = "amid" },
						Value = "value"
					}
				}
			};

			var actual = SerializationExtensions.DeserializeValueData(
				new object[] { new Dictionary<string, object> { { "Ref", "id#amid" }, { "Value", "value" } } });

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void When_deserializing__ValueData_deserializes_Dictionary_to_a_nonlist_ValueData()
		{
			var expected = new ValueData
			{
				IsList = false,
				Values = new List<ObjectData>{
					new ObjectData
					{
						Reference = new ObjectReferenceData { Id = "id", ActualModelId = "amid", ViewModelId = "amid" },
						Value = "value"
					}
				}
			};

			var actual = SerializationExtensions.DeserializeValueData(
				new Dictionary<string, object> { { "Ref", "id#amid" }, { "Value", "value" } });

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void When_deserializing__ValueData_deserializes_string_to_a_nonlist_ValueData()
		{
			var expected = new ValueData
			{
				IsList = false,
				Values = new List<ObjectData>{
					new ObjectData
					{
						Reference = new ObjectReferenceData { Id = "id", ActualModelId = "amid", ViewModelId = "amid" },
						Value = "id"
					}
				}
			};

			var actual = SerializationExtensions.DeserializeValueData("id#amid");

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void When_deserializing__ValueData_deserializes_null_to_a_nonlist_null_ValueData()
		{
			var expected = new ValueData
			{
				IsList = false,
				Values = new List<ObjectData> { new ObjectData { Reference = new ObjectReferenceData { IsNull = true } } }
			};

			var actual = SerializationExtensions.DeserializeValueData(null);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void When_deserializing__ValueData_throws_ArgumentException_when_given_object_is_not_null__string_or_List()
		{
			Assert.Throws<ArgumentException>(() => SerializationExtensions.DeserializeValueData(new object()));
		}

		[Test]
		public void When_deserializing__ObjectData_deserializes_Members_to_a_Dictionary_of_ValueData()
		{
			var expected = new ObjectData
			{
				Reference = new ObjectReferenceData { Id = "id", ActualModelId = "amid", ViewModelId = "amid" },
				Value = "id",
				Members = new Dictionary<string, ValueData>
				{
					{"mmid1", new ValueData{Values=new List<ObjectData>{new ObjectData{Reference=new ObjectReferenceData{Id="mmid1_id", ActualModelId="amid", ViewModelId="amid"}, Value="mmid1_id"}}}},
					{"mmid2", new ValueData{Values=new List<ObjectData>{new ObjectData{Reference=new ObjectReferenceData{Id="mmid2_id", ActualModelId="amid", ViewModelId="amid"}, Value="mmid2_id"}}}}
				}
			};

			var actual = SerializationExtensions.DeserializeObjectData(
				new Dictionary<string, object> 
				{ 
					{ "Ref", "id#amid" },
					{"Members", new Dictionary<string, object>{{"mmid1", "mmid1_id#amid"},{"mmid2","mmid2_id#amid"}}}
				});

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void When_deserializing__ObjectData_throws_ArgumentException_when_given_members_is_not_a_dictionary()
		{
			Assert.Throws<ArgumentException>(() => SerializationExtensions.DeserializeObjectData(
				new Dictionary<string, object> { { "Ref", "id#amid" }, { "Members", new object() } }));
		}
	}
}
