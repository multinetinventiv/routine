using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Core;
using Routine.Core.Configuration;

namespace Routine.Engine
{
	public class DomainType
	{
		private readonly ICoreContext ctx;

		public IType Type { get; private set; }

		public DomainObjectInitializer Initializer { get; private set; }

		public Dictionary<string, DomainMember> Member { get; private set; }
		public ICollection<DomainMember> Members { get { return Member.Values; } }

		public Dictionary<string, DomainOperation> Operation { get; private set; }
		public ICollection<DomainOperation> Operations { get { return Operation.Values; } }

		private ILocator Locator { get; set; }
		public IIdExtractor IdExtractor { get; private set; }
		public IValueExtractor ValueExtractor { get; private set; }

		private readonly List<object> staticInstances;

		public string Id { get; private set; }
		public Marks Marks { get; private set; }
		public string Name { get; private set; }
		public string Module { get; private set; }
		public bool IsValueModel { get; private set; }
		public bool IsViewModel { get; private set; }
		public bool Initializable { get { return Initializer != null; } }

		public DomainType(ICoreContext ctx, IType type)
		{
			this.ctx = ctx;

			Type = type;
			Member = new Dictionary<string, DomainMember>();
			Operation = new Dictionary<string, DomainOperation>();

			Id = ctx.CodingStyle.GetTypeId(type);
			Locator = ctx.CodingStyle.GetObjectLocator(Type);
			IdExtractor = ctx.CodingStyle.GetIdExtractor(Type);
			ValueExtractor = ctx.CodingStyle.GetValueExtractor(Type);
			staticInstances = ctx.CodingStyle.GetStaticInstances(Type);
			Marks = new Marks(ctx.CodingStyle.GetMarks(Type));
			Name = Type != null ? Type.Name : null;
			Module = ctx.CodingStyle.GetModuleName(Type);
			IsValueModel = ctx.CodingStyle.IsValue(Type);
			IsViewModel = ctx.CodingStyle.IsView(Type);

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
				Initializer = Initializer != null ? Initializer.GetModel() : new InitializerModel(),
				Members = Members.Select(m => m.GetModel()).ToList(),
				Operations = Operations.Select(o => o.GetModel()).ToList(),
				StaticInstances = staticInstances.Select(o => ctx.CreateDomainObject(o, this).GetObjectData(false)).ToList()
			};
		}

		public object Locate(ParameterData parameterData)
		{
			if (!parameterData.IsNull && string.IsNullOrEmpty(parameterData.ReferenceId) && Initializable)
			{
				return Initializer.Initialize(parameterData.InitializationParameters);
			}

			if (parameterData.IsNull)
			{
				return null;
			}

			return Locate(parameterData.ReferenceId);
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
			if (Locator == null)
			{
				throw new CannotLocateException(Type, id);
			}

			return Locator.Locate(Type, id);
		}
	}

	internal class TypeNotConfiguredException : Exception
	{
		public TypeNotConfiguredException(IType type, ConfigurationException configurationException)
			: base(string.Format("Type '{0}' is not configured.", type), configurationException) { }
	}
}

