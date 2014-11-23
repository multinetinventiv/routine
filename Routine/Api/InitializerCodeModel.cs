using System.Collections.Generic;
using System.Linq;
using Routine.Client;

namespace Routine.Api
{
	public class InitializerCodeModel
	{
		public Rinitializer Initializer { get; private set; }

		public List<ParameterCodeModel> Parameters { get; private set; }
		public List<List<ParameterCodeModel>> Groups { get; private set; }

		public InitializerCodeModel(ApplicationCodeModel application, Rinitializer initializer)
		{
			Initializer = initializer;

			Parameters = initializer.Parameters.Select(p => new ParameterCodeModel(application, p)).ToList();
			Groups = Enumerable.Range(0, initializer.Groups.Count).Select(i => new List<ParameterCodeModel>()).ToList();

			foreach (var param in Parameters)
			{
				foreach (var group in param.Groups)
				{
					Groups[group].Add(param);
				}
			}
		}

		public bool MarkedAs(string mark)
		{
			return Initializer.MarkedAs(mark);
		}
	}
}
