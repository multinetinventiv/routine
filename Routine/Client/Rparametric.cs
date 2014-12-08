using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Core;

namespace Routine.Client
{
	public abstract class Rparametric
	{
		private readonly Dictionary<string, Rparameter> parameters;

		public Rtype Type { get; private set; }
		public List<List<Rparameter>> Groups { get; private set; }
		public List<string> Marks { get; private set; }

		protected Rparametric(string id, int groupCount, List<ParameterModel> parameterModels, List<string> marks, Rtype type)
		{
			Marks = new List<string>(marks);

			Type = type;

			parameters = new Dictionary<string, Rparameter>();
			Groups = Enumerable.Range(0, groupCount).Select(i => new List<Rparameter>()).ToList();

			foreach (var parameterModel in parameterModels)
			{
				parameters[parameterModel.Id] = new Rparameter(parameterModel, this);
			}

			foreach (var paramId in parameters.Keys)
			{
				var param = parameters[paramId];

				foreach (var group in param.Groups)
				{
					if (group >= Groups.Count)
					{
						throw new InvalidOperationException(string.Format("Parameter '{0}' has a group '{1}' that does not exist on '{2}'. There only {3} groups.", param.Id, group, id, Groups.Count));
					}

					Groups[group].Add(param);
				}
			}
		}

		public Rapplication Application { get { return Type.Application; } }
		public Dictionary<string, Rparameter> Parameter { get { return parameters; } }
		public List<Rparameter> Parameters { get { return parameters.Values.ToList(); } }
		public Roperation Operation { get { return this as Roperation; } }
		public Rinitializer Initializer { get { return this as Rinitializer; } }

		public bool IsOperation()
		{
			return this is Roperation;
		}

		public bool IsInitializer()
		{
			return this is Rinitializer;
		}

		public bool MarkedAs(string mark)
		{
			return Marks.Any(m => m == mark);
		}
	}
}