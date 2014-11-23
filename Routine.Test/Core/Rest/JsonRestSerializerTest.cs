using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using NUnit.Framework;
using Routine.Core;
using Routine.Core.Rest;

namespace Routine.Test.Core.Rest
{
	[TestFixture]
	public class JsonRestSerializerTest
	{
		[Test]
		public void Serializes_parameters_into_dictionary_string()
		{
			var testing = new JsonRestSerializer();

			var actual = testing.Serialize(new Dictionary<string, ParameterValueData>
			{
				{
					"p1",
					new ParameterValueData
					{
						Values = new List<ParameterData> {new ParameterData {ReferenceId = "p1_id", ObjectModelId = "p1_omid"}}
					}
				},
				{
					"p2",
					new ParameterValueData
					{
						IsList = true,
						Values = new List<ParameterData> {new ParameterData {ReferenceId = "p2_id", ObjectModelId = "p2_omid"}}
					}
				},
				{
					"p3",
					new ParameterValueData
					{
						Values = new List<ParameterData> {new ParameterData {IsNull = true}}
					}
				},
			});

			Assert.AreEqual("{\"p1\":\"p1_id#p1_omid\",\"p2\":[\"p2_id#p2_omid\"],\"p3\":null}",actual);
		}
	}
}