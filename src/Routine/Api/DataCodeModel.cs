using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Client;

namespace Routine.Api
{
	public class DataCodeModel
	{
		private readonly ApplicationCodeModel application;
		public Rdata Data { get; private set; }

		public TypeCodeModel ReturnModel { get; private set; }

		public DataCodeModel(ApplicationCodeModel application, Rdata data)
		{
			this.application = application;
			Data = data;

			ReturnModel = application.GetModel(data.DataType, data.IsList);
		}

		public ApplicationCodeModel Application { get { return application; } }
		public string Name { get { return Data.Name; } }
		public bool IsList { get { return Data.IsList; } }

		public string GetName(int mode)
		{
			return application.Configuration.GetName(this, mode);
		}

		public List<Type> GetAttributes(int mode)
		{
			return application.Configuration.GetAttributes(this, mode);
		}

		public string RenderAttributes(int mode)
		{
			return string.Join("\r\n", GetAttributes(mode).Select(t => string.Format("[{0}]", t.ToCSharpString())));
		}

		public bool MarkedAs(string mark)
		{
			return Data.MarkedAs(mark);
		}

		#region Equality & Hashcode

		protected bool Equals(DataCodeModel other)
		{
			return Equals(Data, other.Data);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((DataCodeModel)obj);
		}

		public override int GetHashCode()
		{
			return (Data != null ? Data.GetHashCode() : 0);
		}

		#endregion
	}
}
