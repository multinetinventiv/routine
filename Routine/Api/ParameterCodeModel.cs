using System.Collections.Generic;
using Routine.Client;

namespace Routine.Api
{
	public class ParameterCodeModel
	{
		public ApplicationCodeModel Application { get; private set; }
		public Rparameter Parameter { get; private set; }
		public TypeCodeModel ParameterModel { get; private set; }

		public ParameterCodeModel(ApplicationCodeModel application, Rparameter parameter)
		{
			Application = application;
			Parameter = parameter;

			ParameterModel = application.GetModel(parameter.ParameterType, parameter.IsList);
		}

		public string Id { get { return Parameter.Id; } }
		public List<int> Groups { get { return Parameter.Groups; } }

		public string GetName(int mode)
		{
			return Application.Configuration.GetName(this, mode);
		}

		public bool MarkedAs(string mark)
		{
			return Parameter.MarkedAs(mark);
		}

		#region Equality & Hashcode

		protected bool Equals(ParameterCodeModel other)
		{
			return Equals(Parameter, other.Parameter);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((ParameterCodeModel)obj);
		}

		public override int GetHashCode()
		{
			return (Parameter != null ? Parameter.GetHashCode() : 0);
		} 

		#endregion
	}
}
