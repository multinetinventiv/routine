using System;
using System.Collections.Generic;
using Routine.Core;

namespace Routine.Soa
{
	public static class SerializationExtensions
	{
		private const string REF_SPLITTER = "#";
		private const string REF_KEY = "Ref";
		private const string VAL_KEY = "Value";
		private const string MEM_KEY = "Members";

		public static object ToSerializable(this ObjectReferenceData source)
		{
			if (source.IsNull) { return null; }

			var result = source.Id;
			result += REF_SPLITTER + source.ActualModelId;
			if (!string.IsNullOrEmpty(source.ViewModelId) && source.ViewModelId != source.ActualModelId)
			{
				result += REF_SPLITTER + source.ViewModelId;
			}

			return result;
		}

		public static object ToSerializable(this ObjectData source)
		{
			if (source.Reference.IsNull) { return null; }

			var result = new Dictionary<string, object>();

			if (!source.ValueShouldBeExplicit() && source.Members.Count <= 0)
			{
				return source.Reference.ToSerializable();
			}

			result[REF_KEY] = source.Reference.ToSerializable();
			if (source.ValueShouldBeExplicit())
			{
				result[VAL_KEY] = source.Value;
			}

			if (source.Members.Count > 0)
			{
				var members = new Dictionary<string, object>();

				foreach (var member in source.Members)
				{
					members.Add(member.Key, member.Value.ToSerializable());
				}

				result[MEM_KEY] = members;
			}

			return result;
		}

		private static bool ValueShouldBeExplicit(this ObjectData source)
		{
			return !string.IsNullOrEmpty(source.Value) && source.Reference.Id != source.Value;
		}

		public static object ToSerializable(this ValueData source)
		{
			if (!source.IsList)
			{
				if (source.Values.Count <= 0) { return null; }

				return source.Values[0].ToSerializable();
			}

			var result = new List<object>();
			foreach (var value in source.Values)
			{
				result.Add(value.ToSerializable());
			}
			return result;
		}

		public static ObjectReferenceData DeserializeObjectReferenceData(object @object)
		{
			if (@object == null) { return new ObjectReferenceData { IsNull = true }; }

			if (!(@object is string)) { throw new ArgumentException("Given parameter value should be null or string, but was " + @object, "@object"); }

			string refString = @object as string;

			if (!refString.Contains(REF_SPLITTER)) { throw new ArgumentException(string.Format("Given string should contain at least one '{0}' to split id and actual model id", REF_SPLITTER), "@object"); }
			
			var result = new ObjectReferenceData();
			result.Id = refString.Before(REF_SPLITTER);
			
			var midString = refString.After(REF_SPLITTER);
			if (midString.Contains(REF_SPLITTER))
			{
				result.ActualModelId = midString.Before(REF_SPLITTER);
				result.ViewModelId = midString.After(REF_SPLITTER);
			}
			else
			{
				result.ActualModelId = midString;
				result.ViewModelId = midString;
			}

			return result;
		}

		public static ObjectData DeserializeObjectData(object @object)
		{
			var result = new ObjectData();

			if (@object == null || @object is string) 
			{ 
				result.Reference = DeserializeObjectReferenceData(@object);
				result.Value = result.Reference.Id;
				return result;
			}

			if (!(@object is Dictionary<string, object>)) { throw new ArgumentException("Given parameter value should be null, string or Dictionary<string, object>, but was " + @object, "@object"); }

			var dict = @object as Dictionary<string, object>;

			if (!dict.ContainsKey(REF_KEY)) { throw new ArgumentException("Given dictionary does not contain reference", "@object"); }

			result.Reference = DeserializeObjectReferenceData(dict[REF_KEY]);

			if (dict.ContainsKey(VAL_KEY)) 
			{
				if (!(dict[VAL_KEY] is string)) { throw new ArgumentException("Given dictionary contains value, but it was not string, it was " + @object, "@object"); }

				result.Value = dict[VAL_KEY] as string; 
			}
			else { result.Value = result.Reference.Id; }

			if (!dict.ContainsKey(MEM_KEY)) { return result; }

			if (!(dict[MEM_KEY] is Dictionary<string, object>)) { throw new ArgumentException("Given dictionary contains members, but it was not Dictionary<string, object>, it was " + @object, "@object"); }

			var members = dict[MEM_KEY] as Dictionary<string, object>;

			foreach(var member in members.Keys)
			{
				result.Members.Add(member, DeserializeValueData(members[member]));
			}

			return result;
		}

		public static ValueData DeserializeValueData(object @object)
		{
			if (@object == null || @object is string || @object is Dictionary<string, object>) 
			{
				return new ValueData { Values = new List<ObjectData> { DeserializeObjectData(@object) } };
			}

			if (!(@object is object[])) { throw new ArgumentException("Given parameter value should be null, string, Dictionary<string, object> or List<object>, but was " + @object, "@object"); }

			var list = @object as object[];

			var result = new ValueData { IsList = true };

			foreach (var value in list)
			{
				result.Values.Add(DeserializeObjectData(value));
			}

			return result;
		}
	}
}
