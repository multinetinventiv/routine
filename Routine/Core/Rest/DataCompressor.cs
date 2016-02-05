using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Engine;
using Routine.Engine.Context;

namespace Routine.Core.Rest
{
	public class DataCompressor
	{
		private const string ID_KEY = "Id";
		private const string MODEL_ID_KEY = "ModelId";
		private const string VIEW_MODEL_ID_KEY = "ViewModelId";
		private const string DISPLAY_KEY = "Display";
		private const string DATA_KEY = "Data";
		private const string PARAMS_KEY = "Data";

		private readonly ApplicationModel model;
		private readonly string knownViewModelId;

		public DataCompressor(ApplicationModel model) : this(model, null) { }
		public DataCompressor(ApplicationModel model, string knownViewModelId)
		{
			this.model = model;
			this.knownViewModelId = knownViewModelId;
		}

		public object Compress(ReferenceData source)
		{
			if (source == null || source.Id == null)
			{
				return null;
			}

			if (string.IsNullOrEmpty(source.ModelId)) { throw new ArgumentException("ModelId value of a ReferenceData cannot be null or empty. If null value was plannig to be sent, then the given ReferenceData itself should be null"); }

			if (string.IsNullOrEmpty(source.ViewModelId))
			{
				source.ViewModelId = knownViewModelId;
			}

			if (source.ModelId == knownViewModelId)
			{
				return source.Id;
			}

			var result = new Dictionary<string, object>();

			result[ID_KEY] = source.Id;
			result[MODEL_ID_KEY] = source.ModelId;

			if (source.ViewModelId != knownViewModelId && source.ViewModelId != source.ModelId)
			{
				result[VIEW_MODEL_ID_KEY] = source.ViewModelId;
			}

			return result;
		}

		public object Compress(ObjectData source)
		{
			if (source == null || source.Id == null)
			{
				return null; 
			}

			if (string.IsNullOrEmpty(source.ModelId)) { throw new ArgumentException("ModelId value of a ObjectData cannot be null or empty. If null value was plannig to be sent, then the given ObjectData itself should be null"); }

			var result = new Dictionary<string, object>();

			if (ShouldBeSerializedAsReference(source))
			{
				return Compress(new ReferenceData { Id = source.Id, ModelId = source.ModelId });
			}

			result[ID_KEY] = source.Id;
			result[DISPLAY_KEY] = source.Display;

			if (source.ModelId != knownViewModelId)
			{
				result[MODEL_ID_KEY] = source.ModelId;
			}

			if (source.Data.Count > 0)
			{
				var dataDict = new Dictionary<string, object>();

				foreach (var data in source.Data)
				{
					var dataModel = GetDataModel(knownViewModelId ?? source.ModelId, data.Key);
					if (dataModel == null) { continue; }

					dataDict.Add(data.Key, new DataCompressor(model, dataModel.ViewModelId).Compress(data.Value));
				}

				result[DATA_KEY] = dataDict;
			}

			return result;
		}

		public object Compress(VariableData source)
		{
			if (source == null) { return null; }

			if (!source.IsList)
			{
				if (source.Values.Count <= 0) { return null; }

				return Compress(source.Values[0]);
			}

			return source.Values.Select(value => Compress(value)).ToList();
		}

		public object Compress(ParameterData source)
		{
			if (source == null) { return null; }

			if (source.Id == null && source.InitializationParameters.Count > 0)
			{
				var result = new Dictionary<string, object>();

				if (source.ModelId != knownViewModelId)
				{
					result.Add(MODEL_ID_KEY, source.ModelId);
				}

				var data = new Dictionary<string, object>();
				
				foreach (var paramName in source.InitializationParameters.Keys)
				{
					var paramModel = GetInitializationParameterModel(source.ModelId, paramName);
					if (paramModel == null){continue;}

					data.Add(paramName, new DataCompressor(model, paramModel.ViewModelId).Compress(source.InitializationParameters[paramName]));
				}

				result[PARAMS_KEY] = data;

				return result;
			}

			return Compress(new ReferenceData { Id = source.Id, ModelId = source.ModelId, ViewModelId = source.ModelId });
		}

		public object Compress(ParameterValueData source)
		{
			if (source == null) { return null; }

			if (!source.IsList)
			{
				if (source.Values.Count <= 0) { return null; }

				return Compress(source.Values[0]);
			}

			return source.Values.Select(Compress).ToList();
		}

		public ReferenceData DecompressReferenceData(object @object)
		{
			if (@object == null) { return null; }

			if (@object is string)
			{
				if (string.IsNullOrEmpty(knownViewModelId)) { throw new ArgumentException("Cannot deserialize a string to a ReferenceData instance when model id is not known", "object"); }

				return new ReferenceData { Id = (string)@object, ModelId = knownViewModelId, ViewModelId = knownViewModelId };
			}

			if (!(@object is Dictionary<string, object>)) { throw new ArgumentException("Given parameter value should be null, string or Dictionary<string, object>, but was " + @object, "object"); }

			var dict = (Dictionary<string, object>)@object;
			var result = new ReferenceData();

			object id;
			if (!dict.TryGetValue(ID_KEY, out id)) { throw new ArgumentException("Given dictionary does not contain Id", "object"); }
			if (id == null) { return null; }

			result.Id = id.ToString();

			object modelId;
			if (!dict.TryGetValue(MODEL_ID_KEY, out modelId))
			{
				modelId = knownViewModelId;
			}

			if (modelId == null) { throw new ArgumentException("ModelId in the given dictionary should not be null when model id is not known"); }
			result.ModelId = modelId.ToString();

			if (string.IsNullOrEmpty(result.ModelId)) { throw new ArgumentException("ModelId in the given dictionary should not be null or empty when model id is not known"); }

			object viewModelId;
			if (!dict.TryGetValue(VIEW_MODEL_ID_KEY, out viewModelId) || string.IsNullOrEmpty(viewModelId.ToString()))
			{
				viewModelId = !string.IsNullOrEmpty(knownViewModelId) ? knownViewModelId : result.ModelId;
			}

			result.ViewModelId = viewModelId.ToString();

			return result;
		}

		public ObjectData DecompressObjectData(object @object)
		{
			if (@object == null) { return null; }

			var reference = DecompressReferenceData(@object);
			var result = new ObjectData();

			result.Id = reference.Id;
			result.ModelId = reference.ModelId;

			if (@object is string)
			{
				result.Display = result.Id;

				return result;
			}

			if (!(@object is Dictionary<string, object>)) { throw new ArgumentException("Given parameter value should be null, string or Dictionary<string, object>, but was " + @object, "object"); }

			var dict = (Dictionary<string, object>)@object;

			object display;
			if (dict.TryGetValue(DISPLAY_KEY, out display) && display != null)
			{
				result.Display = display.ToString();
			}
			else
			{
				result.Display = result.Id;
			}

			object data;
			if (!dict.TryGetValue(DATA_KEY, out data)) { return result; }

			if (!(data is Dictionary<string, object>)) { throw new ArgumentException("Given dictionary contains data, but it was not Dictionary<string, object>, it was " + @object, "object"); }

			var dataDict = (Dictionary<string, object>)data;

			foreach (var dataKey in dataDict.Keys)
			{
				var dataModel = GetDataModel(reference.ViewModelId, dataKey);
				if (dataModel == null) { continue; }

				result.Data.Add(dataKey, new DataCompressor(model, dataModel.ViewModelId).DecompressValueData(dataDict[dataKey]));
			}

			return result;
		}

		public VariableData DecompressValueData(object @object)
		{
			if (@object == null || @object is string || @object is Dictionary<string, object>)
			{
				return new VariableData { Values = new List<ObjectData> { DecompressObjectData(@object) } };
			}

			if (!(@object is object[])) { throw new ArgumentException("Given parameter value should be null, string, Dictionary<string, object> or object[], but was " + @object, "object"); }

			var list = (object[])@object;

			var result = new VariableData { IsList = true };

			foreach (var value in list)
			{
				result.Values.Add(DecompressObjectData(value));
			}

			return result;
		}

		public ParameterData DecompressParameterData(object @object)
		{
			if (@object == null) { return null; }

			if (@object is string)
			{
				var referenceData = DecompressReferenceData(@object);

				return new ParameterData
				{
					Id = referenceData.Id,
					ModelId = referenceData.ModelId
				};
			}

			if (!(@object is Dictionary<string, object>)) { throw new ArgumentException("Given parameter value should be null, string or dictionary, but was " + @object, "object"); }

			var result = new ParameterData();
			var dict = (Dictionary<string, object>)@object;

			object modelId;
			if (!dict.TryGetValue(MODEL_ID_KEY, out modelId))
			{
				modelId = knownViewModelId;
			}

			if (modelId == null) { throw new ArgumentException("ModelId in the given dictionary should not be null when model id is not known"); }
			result.ModelId = modelId.ToString();

			if (string.IsNullOrEmpty(result.ModelId)) { throw new ArgumentException("ModelId in the given dictionary should not be null or empty when model id is not known"); }

			object id;
			if (dict.TryGetValue(ID_KEY, out id))
			{
				if (id == null) { throw new ArgumentException("When id is given in a parameter, it should not be null. If the attempt was to pass null as parameter then ParameterData itself should've been null. "); }

				result.Id = id.ToString();

				return result;
			}

			object parameters;
			if (!dict.TryGetValue(PARAMS_KEY, out parameters))
			{
				parameters = new Dictionary<string, object>();
			}

			if (!(parameters is Dictionary<string, object>)) { throw new ArgumentException(string.Format("Given parameters should be Dictionary<string, object>, but was '{0}'", parameters), "object"); }

			var paramsDict = (Dictionary<string, object>)parameters;

			foreach (var paramName in paramsDict.Keys)
			{
				var paramModel = GetInitializationParameterModel(result.ModelId, paramName);
				if (paramModel == null) { continue; }

				result.InitializationParameters[paramName] = new DataCompressor(model, paramModel.ViewModelId)
					.DecompressParameterValueData(paramsDict[paramName]);
			}

			return result;
		}

		public ParameterValueData DecompressParameterValueData(object @object)
		{
			if (@object == null || @object is string || @object is Dictionary<string, object>)
			{
				return new ParameterValueData { Values = new List<ParameterData> { DecompressParameterData(@object) } };
			}

			if (!(@object is object[])) { throw new ArgumentException("Given parameter value should be null, string, Dictionary<string, object> or object[], but was " + @object, "object"); }

			var list = (object[])@object;

			var result = new ParameterValueData { IsList = true };

			foreach (var value in list)
			{
				result.Values.Add(DecompressParameterData(value));
			}

			return result;
		}

		private DataModel GetDataModel(string modelId, string dataModelName)
		{
			DataModel result;

			GetObjectModel(modelId).Data.TryGetValue(dataModelName, out result);

			return result;
		}

		private ParameterModel GetInitializationParameterModel(string modelId, string initializationParameterName)
		{
			ParameterModel result;

			GetObjectModel(modelId).Initializer.Parameter.TryGetValue(initializationParameterName, out result);

			return result;
		}

		private ObjectModel GetObjectModel(string modelId)
		{
			ObjectModel result;

			if (!model.Model.TryGetValue(modelId, out result))
			{
				throw new TypeNotFoundException(modelId);
			}

			return result;
		}

		private bool ShouldBeSerializedAsReference(ObjectData source)
		{
			return (string.IsNullOrEmpty(source.Display) || source.Id == source.Display) && source.Data.Count <= 0;
		}
	}
}
