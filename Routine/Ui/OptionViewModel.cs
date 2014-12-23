using Routine.Client;
using Routine.Core.Rest;

namespace Routine.Ui
{
	public class OptionViewModel : ViewModelBase
	{
		private readonly IMvcConfiguration configuration;

		public ParameterViewModel ParameterViewModel { get; private set; }
		public Robject Object { get; private set; }

		public OptionViewModel(IMvcConfiguration configuration, Robject robj) : this(configuration, null, robj) { }
		public OptionViewModel(IMvcConfiguration configuration, ParameterViewModel parameterViewModel, Robject robj)
			: base(configuration)
		{
			this.configuration = configuration;

			ParameterViewModel = parameterViewModel;
			Object = robj;
		}

		public string Id { get { return Object.ObjectReferenceData.ToSerializable().ToString(); } }
		public string Value { get { return Object.Value; } }
		public int Order { get { return configuration.GetOrder(this); } }
	}
}