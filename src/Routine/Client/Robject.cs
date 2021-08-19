using System.Collections.Generic;
using System.Linq;
using Routine.Core;

namespace Routine.Client
{
	public class Robject
	{
		#region Data Adapter Methods

		private static ReferenceData Rd(bool isNull) => Rd(null, null, null, isNull);
        private static ReferenceData Rd(string id, string mid, string vmid, bool isNull) => isNull ? null : new ReferenceData { Id = id, ModelId = mid, ViewModelId = vmid };
        private static ObjectData Od(ReferenceData rd) => Od(rd, null);
        private static ObjectData Od(ReferenceData rd, string display) => rd == null ? null : new ObjectData { Id = rd.Id, ModelId = rd.ModelId, Display = display };

        #endregion

		private readonly string id;
		private readonly List<Rvariable> initializationParameters;
		private readonly Dictionary<string, DataValue> datas;

		private string display;

		public Rtype ActualType { get; }
		public Rtype ViewType { get; }
		public Rtype Type => ViewType ?? ActualType;

        public Robject() : this(Rd(true), null, null) { }
		public Robject(IEnumerable<Rvariable> initializationParameters, Rtype type) : this(initializationParameters.ToList(), Od(Rd(null, type.Id, type.Id, false)), type, type) { }
		public Robject(string id, Rtype type) : this(id, type, type) { }
		public Robject(string id, Rtype actualType, Rtype viewType) : this(Rd(id, actualType.Id, viewType.Id, id == null), actualType, viewType) { }
		internal Robject(ReferenceData referenceData, Rtype actualType, Rtype viewType) : this(Od(referenceData), actualType, viewType) { }
		internal Robject(ReferenceData referenceData, Rtype actualType, Rtype viewType, string value) : this(Od(referenceData, value), actualType, viewType) { }
		internal Robject(ObjectData objectData, Rtype actualType, Rtype viewType) : this(null, objectData, actualType, viewType) { }
		private Robject(List<Rvariable> initializationParameters, ObjectData objectData, Rtype actualType, Rtype viewType)
		{
			if (actualType != null && actualType.IsViewType)
			{
				throw new CannotCreateRobjectException($"Cannot create object with a view type '{actualType}' given as the actual type");
			}

			if (actualType != null && !actualType.CanBe(viewType))
			{
				throw new CannotCreateRobjectException($"{actualType} cannot be {viewType}");
			}

			this.initializationParameters = initializationParameters;
			
			id = objectData != null ? objectData.Id : null;

			ActualType = actualType;
			ViewType = viewType;

			datas = new Dictionary<string, DataValue>();

			if (!IsNull)
			{
				FillObject(objectData);
			}
		}

		private void FillObject(ObjectData objectData)
		{
			display = objectData.Display;

			if (objectData.Data.Count <= 0) { return; }

			LoadDataIfNecessary();

			foreach (var dataName in objectData.Data.Keys)
			{
				datas[dataName].SetData(objectData.Data[dataName]);
			}
		}

		private void LoadDataIfNecessary()
		{
			if (IsNull) { return; }
			if (Type.IsValueType) { return; }
			if (datas.Any()) { return; }

			foreach (var data in Type.Datas)
			{
				datas.Add(data.Name, new DataValue(this, data));
			}
		}

		private void FetchValueIfNecessary()
		{
			if (display != null) { return; }

			if (IsNull)
			{
				display = "";

				return;
			}

			if (Type.IsValueType)
			{
				display = id;

				return;
			}

			if (IsInitializedOnClient)
			{
				throw new RobjectIsInitializedOnClientException();
			}

			LoadObject();
		}

		internal void LoadObject()
		{
			if (IsNull) { return; }
			if (Type.IsValueType) { return; }

			FillObject(Application.Service.Get(ReferenceData));
		}

		internal ParameterData GetParameterData()
		{
			if (IsNull) { return null; }

			if (!IsInitializedOnClient)
			{
				return new ParameterData
				{
					ModelId = ActualType.Id,
					Id = id
				};
			}

			var result = new ParameterData { ModelId = ActualType.Id };

			foreach (var initializationParameter in initializationParameters)
			{
				result.InitializationParameters.Add(initializationParameter.Name, initializationParameter.GetParameterValueData());
			}

			return result;
		}

		public Rapplication Application => Type.Application;
        public string Id => id;
        public bool IsNull => id == null && !IsInitializedOnClient;
        public bool IsNaked => Equals(ActualType, ViewType);
        public bool IsInitializedOnClient => initializationParameters != null;
        public string Display { get { FetchValueIfNecessary(); return display; } }
		internal ReferenceData ReferenceData => Rd(id, ActualType.Id, ViewType.Id, IsNull);

        public DataValue this[string dataName] { get { LoadDataIfNecessary(); return datas[dataName]; } }
		public List<DataValue> DataValues { get { LoadDataIfNecessary(); return datas.Values.ToList(); } }

		public Robject As(Rtype viewType)
		{
			return Type.Get(Id, viewType);
		}

		public Rvariable Perform(string operationName, params Rvariable[] parameters) { return Perform(operationName, parameters.ToList()); }
		public Rvariable Perform(string operationName, List<Rvariable> parameters)
		{
			if (IsNull) { return new Rvariable(); }

			return Type.Operation[operationName].Perform(this, parameters);
		}

		public void Invalidate()
		{
			display = null;

			foreach (var data in datas.Values)
			{
				data.Invalidate();
			}
		}

		public class DataValue
		{
			public Robject Object { get; }
			public Rdata Data { get; }

			internal DataValue(Robject @object, Rdata data)
			{
				Object = @object;
				Data = data;
			}

			private Rvariable value;
			private VariableData data;

			internal void SetData(VariableData data)
			{
				this.data = data;

				value = new Rvariable(Object.Type.Application, data, Data.DataType.Id);
			}

			public Rvariable Get()
			{
				if (data == null)
				{
					Object.LoadObject();
				}

				return value;
			}

			internal void Invalidate()
			{
				value = null;
				data = null;
			}
		}

		public override string ToString()
		{
			return $"{Id}({Type.Id})";
		}

		#region Equality & Hashcode

		protected bool Equals(Robject other)
		{
			if (IsInitializedOnClient || other.IsInitializedOnClient)
			{
				return false;
			}

			return string.Equals(id, other.id) && Equals(ActualType, other.ActualType) && Equals(ViewType, other.ViewType);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;

			return Equals((Robject)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (id != null ? id.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (ActualType != null ? ActualType.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (ViewType != null ? ViewType.GetHashCode() : 0);
				return hashCode;
			}
		}

		#endregion
	}
}
