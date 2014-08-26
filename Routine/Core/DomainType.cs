using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Routine.Core
{
	public class DomainType
	{
		private readonly ICoreContext ctx;

		public DomainType(ICoreContext context)
		{
			this.ctx = context;
		}

		public TypeInfo Type {get; private set;}

		public DomainObjectInitializer Initializer { get; private set; }

		public Dictionary<string, DomainMember> Member{ get; private set;}
		public ICollection<DomainMember> Members {get{return Member.Values;}}

		public Dictionary<string, DomainOperation> Operation{ get; private set;}
		public ICollection<DomainOperation> Operations {get{return Operation.Values;}}

		public string Id { get; private set; }
		public Marks Marks { get; private set; }
		public string Name { get; private set; }
		public string Module { get; private set; }
		public bool IsValueModel { get; private set; }
		public bool IsViewModel { get; private set; }

		internal DomainType For(string objectModelId)
		{			
			Type = ctx.CodingStyle.ModelIdSerializer.Deserialize(objectModelId);
			Member = new Dictionary<string, DomainMember>();
			Operation = new Dictionary<string, DomainOperation>();

			Id = objectModelId;
			Marks = new Marks(ctx.CodingStyle.ModelMarkSelector.Select(Type));
			Name = Type.Name;
			Module = ctx.CodingStyle.ModelModuleExtractor.Extract(Type);
			IsValueModel = ctx.CodingStyle.ModelIsValueExtractor.Extract(Type);
			IsViewModel = ctx.CodingStyle.ModelIsViewExtractor.Extract(Type);

			foreach (var initializer in ctx.CodingStyle.InitializerSelector.Select(Type))
			{
				try
				{
					if (Initializer == null)
					{
						Initializer = ctx.CreateDomainObjectInitializer(this, initializer);
					}
					else
					{
						Initializer.AddGroup(initializer);
					}
				}
				catch (CannotSerializeDeserializeException ex)
				{
					Debug.WriteLine("Initializer is skipped. Message: " + ex.Message);
					continue;
				}
				catch (InitializedTypeDoNotMatchException ex)
				{
					Debug.WriteLine("Initializer is skipped. Message: " + ex.Message);
					continue;
				}
				catch (ParameterTypesDoNotMatchException ex)
				{
					Debug.WriteLine("Initializer is skipped. Message: " + ex.Message);
					continue;
				}
			}

			foreach(var member in ctx.CodingStyle.MemberSelector.Select(Type))
			{
				try
				{
					Member.Add(member.Name, ctx.CreateDomainMember(this, member));
				}
				catch(CannotSerializeDeserializeException ex)
				{
					Debug.WriteLine(Type.Name + "." + member.Name + " member is skipped. Message: " + ex.Message);
					continue;
				}
			}

			foreach(var operation in ctx.CodingStyle.OperationSelector.Select(Type))
			{
				try
				{
					if (Operation.ContainsKey(operation.Name))
					{
						Operation[operation.Name].AddGroup(operation);
					}
					else
					{
						Operation.Add(operation.Name, ctx.CreateDomainOperation(this, operation));
					}
				}
				catch (CannotSerializeDeserializeException ex)
				{
					Debug.WriteLine(Type.Name + "." + operation.Name + " operation is skipped. Message: " + ex.Message);
					continue;
				}
				catch (ReturnTypesDoNotMatchException ex)
				{
					Debug.WriteLine(Type.Name + "." + operation.Name + " operation is skipped. Message: " + ex.Message);
					continue;
				}
				catch (ParameterTypesDoNotMatchException ex)
				{
					Debug.WriteLine(Type.Name + "." + operation.Name + " operation is skipped. Message: " + ex.Message);
					continue;
				}
			}

			return this;
		}

		public bool MarkedAs(string mark)
		{
			return Marks.Has(mark);
		}

		public ObjectModel GetModel()
		{
			return new ObjectModel {
				Id = Id,
				Marks = Marks.List,
				Name = Name,
				Module = Module,
				IsViewModel = IsViewModel,
				IsValueModel = IsValueModel,
				Initializer = Initializer != null ? Initializer.GetModel() : new InitializerModel(),
				Members = Members.Select(m => m.GetModel()).ToList(),
				Operations = Operations.Select(o => o.GetModel()).ToList()
			};
		}

		public List<DomainObject> GetAvailableObjects()
		{
			var result = ctx.CodingStyle.AvailableIdsExtractor.Extract(Type);

			return result.Select(id => ctx.GetDomainObject(id, Id)).ToList();
		}

		public object Initialize(Dictionary<string, ParameterValueData> parameterValues)
		{
			if (Initializer == null)
			{
				throw new InvalidOperationException(string.Format("Cannot initialize {0}. Domain type does not have an initializer.", Type));
			}

			return Initializer.Initialize(parameterValues);
		}
	}
}

