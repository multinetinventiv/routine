using Routine.Core;

namespace Routine.Api
{
	public class MemberCodeModel : CodeModelBase
	{
		public MemberCodeModel(IApiGenerationContext context)
			: base(context) { }

		private MemberModel model;

		internal MemberCodeModel With(MemberModel model)
		{
			this.model = model;

			return this;
		}

		public string Id { get { return model.Id; } }
		public bool IsHeavy { get { return model.IsHeavy; } }
		public ObjectCodeModel ReturnModel { get { return CreateObject().With(model.ViewModelId, model.IsList); } }
	}
}
