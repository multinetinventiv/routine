using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Routine.Core
{
	public class DomainParameter
	{
		private static ObjectReferenceData ORD(ParameterData pd) { return new ObjectReferenceData { Id = pd.ReferenceId, ActualModelId = pd.ObjectModelId, ViewModelId = pd.ObjectModelId, IsNull = pd.IsNull }; }

		private readonly ICoreContext ctx;

		public DomainParameter(ICoreContext ctx)
		{
			this.ctx = ctx;
		}

		private DomainOperation domainOperation;
		private TypeInfo type;

		public string Id { get; private set; }
		public Marks Marks { get; private set; }
		public List<int> Groups { get; private set; }
		public string ViewModelId{get;private set;}

		private bool IsList { get { return type.CanBeCollection(); } }

		public DomainParameter For(DomainOperation domainOperation, IParameter parameter, int initialGroupIndex)
		{
			this.domainOperation = domainOperation;
			this.type = parameter.ParameterType;

			Groups = new List<int>();

			Id = parameter.Name;
			Marks = new Marks(ctx.CodingStyle.ParameterMarkSelector.Select(parameter));
			Groups.Add(initialGroupIndex);
			ViewModelId = ctx.CodingStyle.ModelIdSerializer.Serialize(IsList ? parameter.ParameterType.GetItemType() : parameter.ParameterType);

			return this;
		}

		public void AddGroup(IParameter parameter, int groupIndex)
		{
			if (Groups.Contains(groupIndex)) { throw new InvalidOperationException("Given groupIndex (" + groupIndex + ") was already added!"); }
			if (parameter.ParameterType != type) { throw new ParameterTypesDoNotMatchException(type, parameter.ParameterType); }

			Groups.Add(groupIndex);

			Marks.Join(ctx.CodingStyle.ParameterMarkSelector.Select(parameter));
		}

		public bool MarkedAs(string mark)
		{
			return Marks.Has(mark);
		}

		public ParameterModel GetModel()
		{
			return new ParameterModel {
				Id = Id,
				Marks = Marks.List,
				Groups = Groups,
				IsList = IsList,
				ViewModelId = ViewModelId
			};
		}

		internal object Locate(ParameterValueData parameterValueData)
		{
			object result = null;
			if (IsList)
			{
				var parameterValue = type.CreateInstance() as IList;
				foreach (var parameterData in parameterValueData.Values)
				{
					parameterValue.Add(ctx.Locate(ORD(parameterData)));
				}
				result = parameterValue;
			}
			else if (parameterValueData.Values.Any())
			{
				result = ctx.Locate(ORD(parameterValueData.Values[0]));
			}

			return result;
		}
	}

	public class ParameterTypesDoNotMatchException : Exception
	{
		public ParameterTypesDoNotMatchException(TypeInfo expected, TypeInfo actual)
			: base(string.Format("Parameter's expected type is {0}, but given parameter has a type of {1}", expected, actual)) { }
	}
}
