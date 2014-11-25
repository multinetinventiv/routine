using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Core;
using Routine.Core.Configuration;

namespace Routine.Engine
{
	public class DomainParameter
	{
		private readonly ICoreContext ctx;
		private readonly IParameter parameter;
		private readonly Lazy<DomainType> lazyParameterType;

		public string Id { get; private set; }
		public Marks Marks { get; private set; }
		public List<int> Groups { get; private set; }
		public bool IsList { get; private set; }
		public DomainType ParameterType { get { return lazyParameterType.Value; } }

		public DomainParameter(ICoreContext ctx, IParameter parameter, int initialGroupIndex)
		{
			this.ctx = ctx;
			this.parameter = parameter;

			Groups = new List<int>();

			Id = parameter.Name;
			Marks = new Marks(ctx.CodingStyle.GetMarks(parameter));
			Groups.Add(initialGroupIndex);
			IsList = parameter.ParameterType.CanBeCollection();

			var parameterType = IsList ? parameter.ParameterType.GetItemType() : parameter.ParameterType;

			try
			{
				ctx.CodingStyle.GetTypeId(parameterType); //To eagerly check if type is configured
				lazyParameterType = new Lazy<DomainType>(() => ctx.GetDomainType(parameterType));
			}
			catch (ConfigurationException ex)
			{
				throw new TypeNotConfiguredException(parameterType, ex);
			}
		}

		public void AddGroup(IParameter parameter, int groupIndex)
		{
			if (Groups.Contains(groupIndex)) { throw new InvalidOperationException(string.Format("{0}.{1}(...,{2},...): Given groupIndex ({3}) was already added!", parameter.Owner.ParentType.Name, parameter.Owner.Name, parameter.Name, groupIndex)); }
			if (!this.parameter.ParameterType.Equals(parameter.ParameterType)) { throw new ParameterTypesDoNotMatchException(parameter, ParameterType.Type, parameter.ParameterType); }

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
				Id = Id,
				Marks = Marks.List,
				Groups = Groups,
				IsList = IsList,
				ViewModelId = ParameterType.Id
			};
		}

		internal object Locate(ParameterValueData parameterValueData)
		{
			if (IsList)
			{
				var result = parameter.ParameterType.CreateListInstance(parameterValueData.Values.Count);
				if (parameter.ParameterType.IsArray)
				{
					int i = 0;
					foreach (var parameterData in parameterValueData.Values)
					{
						result[i] = GetObject(parameterData);

						i++;
					}
				}
				else
				{
					foreach (var parameterData in parameterValueData.Values)
					{
						result.Add(GetObject(parameterData));
					}
				}
				return result;
			}

			if (parameterValueData.Values.Any())
			{
				return GetObject(parameterValueData.Values[0]);
			}

			return null;
		}

		private object GetObject(ParameterData parameterData)
		{
			var domainType = ParameterType;
			if (parameterData.ObjectModelId != domainType.Id && !string.IsNullOrEmpty(parameterData.ObjectModelId))
			{
				domainType = ctx.GetDomainType(parameterData.ObjectModelId);
			}

			return domainType.Locate(parameterData);
		}
	}

	public class ParameterTypesDoNotMatchException : Exception
	{
		public ParameterTypesDoNotMatchException(IParameter parameter, IType expected, IType actual)
			: base(string.Format("{0}.{1}(...,{2},...): Parameter's expected type is {3}, but given parameter has a type of {4}", parameter.Owner.ParentType.Name, parameter.Owner.Name, parameter.Name, expected, actual)) { }
	}
}
