using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Core;

namespace Routine.Engine
{
	public class DomainParameter
	{
		private readonly ICoreContext ctx;
		private readonly IParameter parameter;

		public string Name { get; private set; }
		public Marks Marks { get; private set; }
		public List<int> Groups { get; private set; }
		public bool IsList { get; private set; }
		public DomainType ParameterType { get; private set; }

		public DomainParameter(ICoreContext ctx, IParameter parameter, int initialGroupIndex)
		{
			this.ctx = ctx;
			this.parameter = parameter;

			Groups = new List<int>();
			
			Name = ctx.CodingStyle.GetName(parameter);
			Marks = new Marks(ctx.CodingStyle.GetMarks(parameter));
			Groups.Add(initialGroupIndex);
			IsList = parameter.ParameterType.CanBeCollection();

			var parameterType = IsList ? parameter.ParameterType.GetItemType() : parameter.ParameterType;

			if (!ctx.CodingStyle.ContainsType(parameterType))
			{
				throw new TypeNotConfiguredException(parameterType);
			}

			ParameterType = ctx.GetDomainType(parameterType);
		}

		public void AddGroup(IParameter parameter, int groupIndex)
		{
			if (Groups.Contains(groupIndex))
			{
				throw new InvalidOperationException(string.Format("{0}.{1}(...,{2},...): Given groupIndex ({3}) was already added!", parameter.Owner.ParentType.Name, parameter.Owner.Name, parameter.Name, groupIndex));
			}

			if (!this.parameter.ParameterType.Equals(parameter.ParameterType))
			{
				throw new ParameterTypesDoNotMatchException(parameter, ParameterType.Type, parameter.ParameterType);
			}

			Groups.Add(groupIndex);

			Marks.Join(ctx.CodingStyle.GetMarks(parameter));
		}

		public bool MarkedAs(string mark)
		{
			return Marks.Has(mark);
		}

		public ParameterModel GetModel()
		{
			return new ParameterModel
			{
				Name = Name,
				Marks = Marks.List,
				Groups = Groups,
				IsList = IsList,
				ViewModelId = ParameterType.Id
			};
		}

		internal object Locate(ParameterValueData parameterValueData)
		{
			if (!IsList)
			{
				return GetObject(parameterValueData);
			}

			var result = parameter.ParameterType.CreateListInstance(parameterValueData.Values.Count);

			var objects = GetObjects(parameterValueData);

			for (int i = 0; i < objects.Count; i++)
			{
				if (parameter.ParameterType.IsArray)
				{
					result[i] = objects[i];
				}
				else
				{
					result.Add(objects[i]);
				}
			}

			return result;
		}

		private object GetObject(ParameterValueData parameterValueData)
		{
			if (!parameterValueData.Values.Any())
			{
				return null;
			}

			var parameterData = parameterValueData.Values[0];

			return GetDomainType(parameterData).Locate(parameterData);
		}

		private List<object> GetObjects(ParameterValueData parameterValueData)
		{
			if (!parameterValueData.Values.Any())
			{
				return new List<object>();
			}

			var result = new List<object>();

			var domainTypes = parameterValueData.Values.Select(pd => GetDomainType(pd)).ToList();

			if (domainTypes.Any(dt => !Equals(dt, ParameterType)))
			{
				for (int i = 0; i < parameterValueData.Values.Count; i++)
				{
					var parameterData = parameterValueData.Values[i];
					var domainType = domainTypes[i];

					result.Add(domainType.Locate(parameterData));
				}
			}
			else
			{
				result.AddRange(ParameterType.LocateMany(parameterValueData.Values));
			}

			return result;
		}

		private DomainType GetDomainType(ParameterData parameterData)
		{
			if (parameterData == null)
			{
				return ctx.GetDomainType((IType)null);
			}

			var domainType = ParameterType;

			if (parameterData.ModelId != domainType.Id && !string.IsNullOrEmpty(parameterData.ModelId))
			{
				domainType = ctx.GetDomainType(parameterData.ModelId);
			}

			return domainType;
		}

		#region Formatting & Equality

		protected bool Equals(DomainParameter other)
		{
			return string.Equals(Name, other.Name);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;

			return Equals((DomainParameter)obj);
		}

		public override int GetHashCode()
		{
			return (Name != null ? Name.GetHashCode() : 0);
		}

		public override string ToString()
		{
			return string.Format("{1} {0}", Name, ParameterType);
		}

		#endregion
	}
}
