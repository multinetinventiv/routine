using System.Collections.Generic;
using System.Linq;
using Routine.Client;
using Routine.Engine;

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
			return string.Format("@{0}", Application.Configuration.GetName(this, mode));
		}

		public List<IType> GetAttributes(int mode)
		{
			return Application.Configuration.GetAttributes(this, mode);
		}

		public string RenderAttributes(int mode)
		{
			return string.Join("\r\n", GetAttributes(mode).Select(t => string.Format("[{0}]", t.ToCSharpString())));
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
