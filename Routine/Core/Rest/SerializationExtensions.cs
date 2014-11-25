using System;
using System.Collections.Generic;
using System.Linq;

namespace Routine.Core.Rest
{
	public static class SerializationExtensions
	{
		private const string REF_SPLITTER = "#";
		private const string ESCAPED_REF_SPLITTER = "##";
		private const string OMID_KEY = "Id";
		private const string PARAMS_KEY = "Params";
		private const string REF_KEY = "Ref";
		private const string VAL_KEY = "Value";
		private const string MEM_KEY = "Members";

		public static object ToSerializable(this ObjectReferenceData source)
		{
			if (source.IsNull) { return null; }

			var result = source.Id.Replace(REF_SPLITTER, ESCAPED_REF_SPLITTER);
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

			return source.Values.Select(value => value.ToSerializable()).ToList();
		}

		public static object ToSerializable(this ParameterData source)
		{
			if (source.IsNull) { return null; }

			if (string.IsNullOrEmpty(source.ReferenceId) && source.InitializationParameters.Count > 0)
			{
				var result = new Dictionary<string, object>();

				result.Add(OMID_KEY, source.ObjectModelId);
				result.Add(PARAMS_KEY, source.InitializationParameters.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToSerializable()));

				return result;
			}

			return source.ReferenceId + REF_SPLITTER + source.ObjectModelId;
		}

		public static object ToSerializable(this ParameterValueData source)
		{
			if (!source.IsList)
			{
				if (source.Values.Count <= 0) { return null; }

				return source.Values[0].ToSerializable();
			}

			return source.Values.Select(value => value.ToSerializable()).ToList();
		}

		public static ObjectReferenceData DeserializeObjectReferenceData(object @object)
		{
			if (@object == null) { return new ObjectReferenceData { IsNull = true }; }

			if (!(@object is string)) { throw new ArgumentException("Given parameter value should be null or string, but was " + @object, "object"); }

			var refString = @object as string;

			if (!refString.Contains(REF_SPLITTER)) { throw new ArgumentException(string.Format("Given string should contain at least one '{0}' to split id and actual model id", REF_SPLITTER), "object"); }

			var result = new ObjectReferenceData();
			const string tempEscape = "__r!s@e_";
			refString = refString.Replace(ESCAPED_REF_SPLITTER, tempEscape);
			result.Id = refString.Before(REF_SPLITTER).Replace(tempEscape, REF_SPLITTER);

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

			if (!(@object is Dictionary<string, object>)) { throw new ArgumentException("Given parameter value should be null, string or Dictionary<string, object>, but was " + @object, "object"); }

			var dict = @object as Dictionary<string, object>;

			if (!dict.ContainsKey(REF_KEY)) { throw new ArgumentException("Given dictionary does not contain reference", "object"); }

			result.Reference = DeserializeObjectReferenceData(dict[REF_KEY]);

			if (dict.ContainsKey(VAL_KEY))
			{
				if (!(dict[VAL_KEY] is string)) { throw new ArgumentException("Given dictionary contains value, but it was not string, it was " + @object, "object"); }

				result.Value = dict[VAL_KEY] as string;
			}
			else { result.Value = result.Reference.Id; }

			if (!dict.ContainsKey(MEM_KEY)) { return result; }

			if (!(dict[MEM_KEY] is Dictionary<string, object>)) { throw new ArgumentException("Given dictionary contains members, but it was not Dictionary<string, object>, it was " + @object, "object"); }

			var members = dict[MEM_KEY] as Dictionary<string, object>;

			foreach (var member in members.Keys)
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

			if (!(@object is object[])) { throw new ArgumentException("Given parameter value should be null, string, Dictionary<string, object> or object[], but was " + @object, "object"); }

			var list = @object as object[];

			var result = new ValueData { IsList = true };

			foreach (var value in list)
			{
				result.Values.Add(DeserializeObjectData(value));
			}

			return result;
		}

		public static ParameterData DeserializeParameterData(object @object)
		{
			if (@object == null) { return new ParameterData { IsNull = true }; }

			if (@object is string)
			{
				var refString = @object as string;

				if (!refString.Contains(REF_SPLITTER)) { throw new ArgumentException(string.Format("Given string should contain '{0}' to split id and object model id", REF_SPLITTER), "object"); }

				return new ParameterData
				{
					ReferenceId = refString.Before(REF_SPLITTER),
					ObjectModelId = refString.After(REF_SPLITTER)
				};
			}

			if (!(@object is Dictionary<string, object>)) { throw new ArgumentException("Given parameter value should be null or string, but was " + @object, "object"); }

			var dict = @object as Dictionary<string, object>;

			if (!dict.ContainsKey(OMID_KEY)) { throw new ArgumentException(string.Format("Given dictionary should contain object model id with key '{0}'", OMID_KEY), "object"); }
			var omid = dict[OMID_KEY];
			if (!(omid is string)) { throw new ArgumentException(string.Format("Given omid should be string, but was '{0}'", omid), "object"); }

			var result = new ParameterData();
			result.ObjectModelId = omid as string;


			if (!dict.ContainsKey(PARAMS_KEY)) { throw new ArgumentException(string.Format("Given dictionary should contain parameters with key '{0}'", PARAMS_KEY), "object"); }
			var parameters = dict[PARAMS_KEY];
			if (!(parameters is Dictionary<string, object>)) { throw new ArgumentException(string.Format("Given parameters should be Dictionary<string, object>, but was '{0}'", parameters), "object"); }
			
			var paramsDict = parameters as Dictionary<string, object>;

			result.InitializationParameters = paramsDict.ToDictionary(kvp => kvp.Key, kvp => DeserializeParameterValueData(kvp.Value));

			return result;
		}

		public static ParameterValueData DeserializeParameterValueData(object @object)
		{
			if (@object == null || @object is string || @object is Dictionary<string, object>)
			{
				return new ParameterValueData { Values = new List<ParameterData> { DeserializeParameterData(@object) } };
			}

			if (!(@object is object[])) { throw new ArgumentException("Given parameter value should be null, string, Dictionary<string, object> or object[], but was " + @object, "object"); }

			var list = @object as object[];

			var result = new ParameterValueData { IsList = true };

			foreach (var value in list)
			{
				result.Values.Add(DeserializeParameterData(value));
			}

			return result;
		}
	}
}
