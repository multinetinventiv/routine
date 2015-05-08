using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Core;
using Routine.Core.Configuration;
using Routine.Core.Rest;

namespace Routine.Engine
{
	public class DomainType
	{
		private readonly ICoreContext ctx;

		public IType Type { get; private set; }

		private readonly List<DomainType> actualTypes;
		private readonly List<DomainType> viewTypes;

		public DomainObjectInitializer Initializer { get; private set; }

		public Dictionary<string, DomainMember> Member { get; private set; }
		public ICollection<DomainMember> Members { get { return Member.Values; } }

		public Dictionary<string, DomainOperation> Operation { get; private set; }
		public ICollection<DomainOperation> Operations { get { return Operation.Values; } }

		private readonly ILocator locator;
		public IIdExtractor IdExtractor { get; private set; }
		public IValueExtractor ValueExtractor { get; private set; }
		private readonly IConverter converter;

		private readonly List<object> staticInstances;

		public int MaxFetchDepth { get; private set; }
		public string Id { get; private set; }
		public Marks Marks { get; private set; }
		public string Name { get; private set; }
		public string Module { get; private set; }
		public bool IsValueModel { get; private set; }
		public bool IsViewModel { get; private set; }

		public bool Initializable { get { return Initializer != null; } }
		public bool Locatable { get { return locator != null; } }

		public DomainType(ICoreContext ctx, IType type)
		{
			this.ctx = ctx;

			Type = type;
			Member = new Dictionary<string, DomainMember>();
			Operation = new Dictionary<string, DomainOperation>();

			Id = ctx.CodingStyle.GetTypeId(type);
			MaxFetchDepth = ctx.CodingStyle.GetMaxFetchDepth();

			if (Id.StartsWith(SerializationExtensions.REF_SPLITTER))
			{
				throw new ConfigurationException("TypeId", type, new Exception(string.Format("TypeId cannot start with the escape character '{0}', but configured TypeId is {1}", SerializationExtensions.REF_SPLITTER, Id)));
			}

			locator = ctx.CodingStyle.GetLocator(Type);
			IdExtractor = ctx.CodingStyle.GetIdExtractor(Type);
			ValueExtractor = ctx.CodingStyle.GetValueExtractor(Type);
			converter = ctx.CodingStyle.GetConverter(Type);

			staticInstances = ctx.CodingStyle.GetStaticInstances(Type);

			Marks = new Marks(ctx.CodingStyle.GetMarks(Type));
			Name = Type != null ? Type.Name : null;
			Module = ctx.CodingStyle.GetModuleName(Type);
			IsValueModel = ctx.CodingStyle.IsValue(Type);
			IsViewModel = ctx.CodingStyle.IsView(Type);

			actualTypes = new List<DomainType>();
			viewTypes = new List<DomainType>();

			foreach (var initializer in ctx.CodingStyle.GetInitializers(Type))
			{
				try
				{
					if (!type.Equals(initializer.InitializedType)) { throw new InitializedTypeDoNotMatchException(initializer, type, initializer.InitializedType); }

					if (Initializer == null)
					{
						Initializer = new DomainObjectInitializer(ctx, initializer);
					}
					else
					{
						Initializer.AddGroup(initializer);
					}
				}
				catch (TypeNotConfiguredException)
				{
					Console.WriteLine("{0}.{1} initializer is skipped. All parameter types and return type should be configured.", Type.Name, initializer.Name);
					Console.WriteLine();
				}
				catch (InitializedTypeDoNotMatchException ex)
				{
					Console.WriteLine("{0}.{1} initializer is skipped. Given initializer should initialize the expected type ({2}) but it initializes another type ({3}). Exception is; {4}",
						Type.Name, initializer.Name, Name, initializer.InitializedType.Name, ex.Message);
					Console.WriteLine();
				}
				catch (ParameterTypesDoNotMatchException ex)
				{
					Console.WriteLine("{0}.{1} initializer is skipped. On initialier groups (constructor overloading) parameters with the same name but different types are not allowed. Exception is; {2}", Type.Name, initializer.Name, ex.Message);
					Console.WriteLine();
				}
				catch (IdenticalSignatureAlreadyAddedException ex)
				{
					Console.WriteLine("{0}.{1} initializer is skipped. An initializer with same signature is already added. Exception is; {2}", Type.Name, initializer.Name, ex.Message);
					Console.WriteLine();
				}
			}

			foreach (var member in ctx.CodingStyle.GetMembers(Type))
			{
				try
				{
					Member.Add(member.Name, new DomainMember(ctx, member));
				}
				catch (TypeNotConfiguredException)
				{
					Console.WriteLine("{0}.{1} member is skipped. Member type should be configured.", Type.Name, member.Name);
					Console.WriteLine();
				}
			}

			foreach (var operation in ctx.CodingStyle.GetOperations(Type))
			{
				try
				{
					if (Operation.ContainsKey(operation.Name))
					{
						Operation[operation.Name].AddGroup(operation);
					}
					else
					{
						Operation.Add(operation.Name, new DomainOperation(ctx, operation));
					}
				}
				catch (TypeNotConfiguredException ex)
				{
					Console.WriteLine("{0}.{1} operation is skipped. All parameter types and return type should be configured. Exception is; {2}", Type.Name, operation.Name, ex.Message);
					Console.WriteLine();
				}
				catch (ReturnTypesDoNotMatchException ex)
				{
					Console.WriteLine("{0}.{1} operation is skipped. Return type should be the same with the others. Exception is; {2}", Type.Name, operation.Name, ex.Message);
					Console.WriteLine();
				}
				catch (ParameterTypesDoNotMatchException ex)
				{
					Console.WriteLine("{0}.{1} operation is skipped. On operation groups (method overloading) parameters with the same name but different types are not allowed. Exception is; {2}", Type.Name, operation.Name, ex.Message);
					Console.WriteLine();
				}
				catch (IdenticalSignatureAlreadyAddedException ex)
				{
					Console.WriteLine("{0}.{1} operation is skipped. An operation with same signature is already added. Exception is; {2}", Type.Name, operation.Name, ex.Message);
					Console.WriteLine();
				}
			}
		}

		internal void LoadSubTypes()
		{
			if (Initializable) { Initializer.LoadSubTypes(); }

			foreach (var operation in Operations)
			{
				operation.LoadSubTypes();
			}

			foreach (var member in Members)
			{
				member.LoadSubTypes();
			}
		}

		internal void LoadCrossTypeRelations()
		{
			foreach (var viewType in ctx.CodingStyle.GetViewTypes(Type).Where(t => !Equals(t, Type)))
			{
				try
				{
					var dt = ctx.GetDomainType(viewType);

					if (!IsViewModel)
					{
						dt.actualTypes.Add(this);
					}

					viewTypes.Add(dt);
				}
				catch (ConfigurationException)
				{
					Console.WriteLine("View type is skipped. {0} does not have TypeId configured", viewType);
					Console.WriteLine();
				}
			}
		}

		public bool MarkedAs(string mark)
		{
			return Marks.Has(mark);
		}

		public ObjectModel GetModel()
		{
			return new ObjectModel
			{
				Id = Id,
				Marks = Marks.List,
				Name = Name,
				Module = Module,
				IsViewModel = IsViewModel,
				IsValueModel = IsValueModel,
				ActualModelIds = actualTypes.Select(t => t.Id).ToList(),
				ViewModelIds = viewTypes.Select(t => t.Id).ToList(),
				Initializer = Initializer != null ? Initializer.GetModel() : new InitializerModel(),
				Members = Members.Select(m => m.GetModel()).ToList(),
				Operations = Operations.Select(o => o.GetModel()).ToList(),
				StaticInstances = staticInstances.Select(o => ctx.CreateDomainObject(o, this).GetObjectData(false)).ToList()
			};
		}

		internal object Locate(ParameterData parameterData)
		{
			return LocateMany(new List<ParameterData> { parameterData })[0];
		}

		internal List<object> LocateMany(List<ParameterData> parameterDatas)
		{
			var result = new object[parameterDatas.Count];

			var locateIdsWithOriginalIndex = new List<Tuple<int, string>>();

			for (int i = 0; i < parameterDatas.Count; i++)
			{
				var parameterData = parameterDatas[i];
				if (Initializable && !parameterData.IsNull && string.IsNullOrEmpty(parameterData.ReferenceId))
				{
					result[i] = Initializer.Initialize(parameterData.InitializationParameters);
				}
				else if (parameterData.IsNull)
				{
					result[i] = null;
				}
				else
				{
					locateIdsWithOriginalIndex.Add(new Tuple<int, string>(i, parameterData.ReferenceId));
				}
			}

			if (locateIdsWithOriginalIndex.Any())
			{
				var locateIds = locateIdsWithOriginalIndex.Select(t => t.Item2).ToList();
				var located = LocateMany(locateIds);

				if (located.Count != locateIdsWithOriginalIndex.Count)
				{
					throw new InvalidOperationException(
						string.Format("Locator returned a result with different number of objects ({0}) than given number of ids ({1}) when locating ids {2} of type {3}",
						located.Count,
						locateIds.Count,
						locateIds.ToItemString(),
						Type));
				}

				for (int i = 0; i < located.Count; i++)
				{
					result[locateIdsWithOriginalIndex[i].Item1] = located[i];
				}
			}

			return result.ToList();
		}

		public object Locate(ObjectReferenceData objectReferenceData)
		{
			if (objectReferenceData.IsNull)
			{
				return null;
			}

			return Locate(objectReferenceData.Id);
		}

		public object Locate(string id)
		{
			return LocateMany(new List<string> { id })[0];
		}

		public List<object> LocateMany(List<string> ids)
		{
			if (!Locatable)
			{
				throw new CannotLocateException(Type, ids);
			}

			if (!ids.Any())
			{
				return new List<object>();
			}

			var notNullIds = ids.Select(id => id ?? string.Empty).ToList();

			return locator.Locate(Type, notNullIds);
		}

		public object Convert(object target, DomainType viewDomainType)
		{
			if (Equals(viewDomainType)) { return target; }
			if (Id == Constants.NULL_MODEL_ID) { return target; }

			if (!viewTypes.Contains(viewDomainType))
			{
				throw new CannotConvertException(target, viewDomainType.Type);
			}

			if (converter == null)
			{
				throw new ConfigurationException("Converter", Type);
			}

			return converter.Convert(target, viewDomainType.Type);
		}

		#region Formatting & Equality

		protected bool Equals(DomainType other)
		{
			return string.Equals(Id, other.Id);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((DomainType)obj);
		}

		public override int GetHashCode()
		{
			return (Id != null ? Id.GetHashCode() : 0);
		}

		public override string ToString()
		{
			return string.Format("Id: {0}", Id);
		}

		#endregion
	}

	internal class TypeNotConfiguredException : Exception
	{
		public TypeNotConfiguredException(IType type, ConfigurationException configurationException)
			: base(string.Format("Type '{0}' is not configured.", type), configurationException) { }
	}
}

