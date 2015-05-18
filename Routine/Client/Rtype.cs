using System.Collections.Generic;
using System.Linq;
using Routine.Core;

namespace Routine.Client
{
	public class Rtype
	{
		public static readonly Rtype Void = new Rtype();

		private readonly ObjectModel model;

		public Rapplication Application { get; private set; }

		public List<Rtype> ViewTypes { get; private set; }
		public List<Rtype> ActualTypes { get; private set; }
		public Rinitializer Initializer { get; private set; }
		public Dictionary<string, Rmember> Member { get; private set; }
		public Dictionary<string, Roperation> Operation { get; private set; }

		private Rtype() : this(null, new ObjectModel { Id = Constants.VOID_MODEL_ID, IsValueModel = true }) { }
		public Rtype(Rapplication application, ObjectModel model)
		{
			Application = application;
			this.model = model;

			ViewTypes = new List<Rtype>();
			ActualTypes = new List<Rtype>();
			Initializer = null;
			Member = new Dictionary<string, Rmember>();
			Operation = new Dictionary<string, Roperation>();
		}

		internal void Load()
		{
			foreach (var viewModelId in model.ViewModelIds)
			{
				ViewTypes.Add(Application[viewModelId]);
			}

			foreach (var actualModelId in model.ActualModelIds)
			{
				ActualTypes.Add(Application[actualModelId]);
			}

			if (model.Initializer.GroupCount > 0)
			{
				Initializer = new Rinitializer(model.Initializer, this);
			}

			foreach (var member in model.Members)
			{
				Member.Add(member.Id, new Rmember(member, this));
			}

			foreach (var operation in model.Operations)
			{
				Operation.Add(operation.Id, new Roperation(operation, this));
			}
		}

		public string Id { get { return model.Id; } }
		public string Name { get { return model.Name; } }
		public string Module { get { return model.Module; } }
		public bool IsValueType { get { return model.IsValueModel; } }
		public bool IsViewType { get { return model.IsViewModel; } }
		public bool IsVoid { get { return Id == Constants.VOID_MODEL_ID; } }
		public bool Initializable { get { return Initializer != null; } }
		public List<Rmember> Members { get { return Member.Values.ToList(); } }
		public List<Roperation> Operations { get { return Operation.Values.ToList(); } }

		public List<string> Marks { get { return model.Marks; } }

		public bool MarkedAs(string mark)
		{
			return model.Marks.Contains(mark);
		}

		public bool CanBe(Rtype viewType)
		{
			if (Equals(this, viewType)) { return true; }

			return ViewTypes.Contains(viewType);
		}

		public List<Robject> StaticInstances
		{
			get
			{
				return model
					.StaticInstances
					.Select(od => new Robject(od, Application[od.Reference.ActualModelId], Application[od.Reference.ViewModelId]))
					.ToList();
			}
		}

		public Robject Get(string id)
		{
			return new Robject(id, this);
		}

		public Robject Get(string id, Rtype viewType)
		{
			return new Robject(id, this, viewType);
		}

		public Robject Init(params Rvariable[] initializationParameters) { return Init(initializationParameters.AsEnumerable()); }
		public Robject Init(IEnumerable<Rvariable> initializationParameters)
		{
			return new Robject(initializationParameters, this);
		}

		public override string ToString()
		{
			return string.Format("{0}({1}.{2})", model.Id, model.Module, model.Name);
		}

		#region Equality & Hashcode

		protected bool Equals(Rtype other)
		{
			return Equals(model, other.model);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) { return false; }
			if (ReferenceEquals(this, obj)) { return true; }
			if (obj.GetType() != GetType()) { return false; }

			return Equals((Rtype)obj);
		}

		public override int GetHashCode()
		{
			return (model != null ? model.GetHashCode() : 0);
		}

		#endregion
	}
}