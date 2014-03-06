using Routine.Core.Service;

namespace Routine.Api
{
	public class Rmember
	{
        private readonly IApiContext context;

		public Rmember(IApiContext context)
		{
            this.context = context;
		}

		private Robject parentObject;
		private MemberModel model;

		internal Rmember With(Robject parentObject, MemberModel model) 
		{
			this.parentObject = parentObject;
			this.model = model;

			return this;
		}

		private Rvariable value;
		private MemberData data;
		internal void SetData(MemberData data)
		{
			this.data = data;
			this.value = context.CreateRvariable().With(data.Value);
		}

		private Rvariable Value {get{FetchDataIfNecessary(); return value;}}

		private void FetchDataIfNecessary()
		{
			if(data == null)
			{
				if(model.IsHeavy)
				{
					SetData(context.ObjectService.GetMember(parentObject.ObjectReferenceData, model.Id));
				}
				else
				{
					parentObject.LoadObject();
				}
			}
		}

		public Robject ParentObject{get{return parentObject;}}
		public string Id {get{return model.Id;}}
		public bool IsList {get{return model.IsList;}}
		public bool IsHeavy {get{return model.IsHeavy;}}

		public Rvariable GetValue() { return Value; }

		internal void Invalidate()
		{
			value = null;
			data = null;
		}
	}
}
