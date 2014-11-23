using System.Collections.Generic;
using Routine.Client;

namespace Routine.Api
{
	public class ParameterCodeModel
	{
		public Rparameter Parameter { get; private set; }
		public ObjectCodeModel Model { get; private set; }

		public ParameterCodeModel(ApplicationCodeModel application, Rparameter parameter)
		{
			Parameter = parameter;

			Model = application.GetModel(parameter.ParameterType, parameter.IsList);
		}

		public string Id { get { return Parameter.Id; } }

		internal List<int> Groups { get { return Parameter.Groups; } }

		public bool MarkedAs(string mark)
		{
			return Parameter.MarkedAs(mark);
		}
	}
}
