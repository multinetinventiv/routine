using System.Collections.Generic;
using System.Linq;
using Routine.Client;
using Routine.Engine;

namespace Routine.Api
{
	public class InitializerCodeModel
	{
		private readonly ApplicationCodeModel application;

		public Rinitializer Initializer { get; private set; }

		public List<ParameterCodeModel> Parameters { get; private set; }
		public List<List<ParameterCodeModel>> Groups { get; private set; }

		public InitializerCodeModel(ApplicationCodeModel application, Rinitializer initializer)
		{
			this.application = application;
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

		public List<IType> GetAttributes(int mode)
		{
			return application.Configuration.GetAttributes(this, mode);
		}

		public string RenderAttributes(int mode)
		{
			return string.Join("\r\n", GetAttributes(mode).Select(t => string.Format("[{0}]", t.ToCSharpString())));
		}

		public bool MarkedAs(string mark)
		{
			return Initializer.MarkedAs(mark);
		}

		#region Equality & Hashcode

		protected bool Equals(InitializerCodeModel other)
		{
			return Equals(Initializer, other.Initializer);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((InitializerCodeModel)obj);
		}

		public override int GetHashCode()
		{
			return (Initializer != null ? Initializer.GetHashCode() : 0);
		}

		#endregion
	}
}
