using System.Collections.Generic;
using System.Linq;
using Routine.Core;

namespace Routine.Client
{
	public class Robject
	{
		#region Data Adapter Methods

		private static ObjectReferenceData Ord(string id, string amid, string vmid, bool isNull) { return new ObjectReferenceData { Id = id, ActualModelId = amid, ViewModelId = vmid, IsNull = isNull }; }
		private static ObjectData Od(ObjectReferenceData ord) { return Od(ord, null); }
		private static ObjectData Od(ObjectReferenceData ord, string value) { return new ObjectData { Reference = ord, Value = value }; }

		#endregion

		private readonly ObjectReferenceData objectReferenceData;
		private readonly List<Rvariable> initializationParameters;
		private readonly Dictionary<string, MemberValue> members;

		private string value;

		public Rtype ActualType { get; private set; }
		public Rtype ViewType { get; private set; }
		public Rtype Type { get { return ViewType ?? ActualType; } }

		public Robject() : this(new ObjectReferenceData { IsNull = true }, null, null) { }
		public Robject(IEnumerable<Rvariable> initializationParameters, Rtype type) : this(initializationParameters.ToList(), Od(Ord(null, type.Id, type.Id, false)), type, type) { }
		public Robject(string id, Rtype type) : this(id, type, type) { }
		public Robject(string id, Rtype actualType, Rtype viewType) : this(Ord(id, actualType.Id, viewType.Id, id == null), actualType, viewType) { }
		internal Robject(ObjectReferenceData objectReferenceData, Rtype actualType, Rtype viewType) : this(Od(objectReferenceData), actualType, viewType) { }
		internal Robject(ObjectReferenceData objectReferenceData, Rtype actualType, Rtype viewType, string value) : this(Od(objectReferenceData, value), actualType, viewType) { }
		internal Robject(ObjectData objectData, Rtype actualType, Rtype viewType) : this(new List<Rvariable>(), objectData, actualType, viewType) { }
		private Robject(List<Rvariable> initializationParameters, ObjectData objectData, Rtype actualType, Rtype viewType)
		{
			if (actualType != null && actualType.IsViewType)
			{
				throw new CannotCreateRobjectException(string.Format("Cannot create object with a view type '{0}' given as the actual type", actualType));
			}

			if (actualType != null && !actualType.CanBe(viewType))
			{
				throw new CannotCreateRobjectException(string.Format("{0} cannot be {1}", actualType, viewType));
			}

			ActualType = actualType;
			ViewType = viewType;

			objectReferenceData = objectData.Reference;

			this.initializationParameters = initializationParameters;

			members = new Dictionary<string, MemberValue>();

			if (!IsNull)
			{
				FillObject(objectData);
			}
		}

		private void FillObject(ObjectData data)
		{
			value = data.Value;

			if (data.Members.Count <= 0) { return; }

			LoadMembersIfNecessary();

			foreach (var memberModelId in data.Members.Keys)
			{
				members[memberModelId].SetData(data.Members[memberModelId]);
			}
		}

		private void LoadMembersIfNecessary()
		{
			if (IsNull) { return; }
			if (Type.IsValueType) { return; }
			if (members.Any()) { return; }

			foreach (var member in Type.Members)
			{
				members.Add(member.Id, new MemberValue(this, member));
			}
		}

		private void FetchValueIfNecessary()
		{
			if (value != null) { return; }

			if (IsNull)
			{
				value = "";

				return;
			}

			if (Type.IsValueType)
			{
				value = objectReferenceData.Id;

				return;
			}

			if (IsInitializedOnClient)
			{
				throw new RobjectIsInitializedOnClientException();
			}

			value = Application.Service.GetValue(objectReferenceData);
		}

		internal void LoadObject()
		{
			if (IsNull) { return; }
			if (Type.IsValueType) { return; }

			FillObject(Application.Service.Get(objectReferenceData));
		}

		internal ObjectReferenceData ObjectReferenceData { get { return objectReferenceData; } }

		internal ParameterData GetParameterData()
		{
			var result = new ParameterData
			{
				IsNull = IsNull,
				ObjectModelId = objectReferenceData.ActualModelId,
				ReferenceId = objectReferenceData.Id
			};

			foreach (var initializationParameter in initializationParameters)
			{
				result.InitializationParameters.Add(initializationParameter.Name, initializationParameter.GetParameterValueData());
			}

			return result;
		}

		public Rapplication Application { get { return Type.Application; } }
		public string Id { get { return objectReferenceData.Id; } }
		public bool IsNull { get { return objectReferenceData.IsNull; } }
		public bool IsNaked { get { return Equals(ActualType, ViewType); } }
		public bool IsInitializedOnClient { get { return string.IsNullOrEmpty(objectReferenceData.Id); } }
		public string Value { get { FetchValueIfNecessary(); return value; } }

		public MemberValue this[string memberModelId] { get { LoadMembersIfNecessary(); return members[memberModelId]; } }
		public List<MemberValue> MemberValues { get { LoadMembersIfNecessary(); return members.Values.ToList(); } }

		public Robject As(Rtype viewType)
		{
			return Type.Get(Id, viewType);
		}

		public Rvariable Perform(string operationModelId, params Rvariable[] parameters) { return Perform(operationModelId, parameters.ToList()); }
		public Rvariable Perform(string operationModelId, List<Rvariable> parameters)
		{
			if (IsNull) { return new Rvariable(); }

			return Type.Operation[operationModelId].Perform(this, parameters);
		}

		public void Invalidate()
		{
			value = null;

			foreach (var member in members.Values)
			{
				member.Invalidate();
			}
		}

		public class MemberValue
		{
			public Robject Object { get; private set; }
			public Rmember Member { get; private set; }

			internal MemberValue(Robject @object, Rmember member)
			{
				Object = @object;
				Member = member;
			}

			private Rvariable value;
			private ValueData data;

			internal void SetData(ValueData data)
			{
				this.data = data;

				value = new Rvariable(Object.Type.Application, data);
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
			return string.Format("{0}({1})", Id, Type.Id);
		}

		#region Equality & Hashcode

		protected bool Equals(Robject other)
		{
			if (IsInitializedOnClient || other.IsInitializedOnClient) { return false; }

			return Equals(objectReferenceData, other.objectReferenceData);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) { return false; }
			if (ReferenceEquals(this, obj)) { return true; }
			if (obj.GetType() != GetType()) { return false; }

			return Equals((Robject)obj);
		}

		public override int GetHashCode()
		{
			return (objectReferenceData != null ? objectReferenceData.GetHashCode() : 0);
		}

		#endregion
	}
}
