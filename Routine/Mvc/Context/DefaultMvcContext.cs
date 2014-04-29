using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Routine.Mvc.Context
{
	public class DefaultMvcContext : IMvcContext
    {
        public IMvcConfiguration MvcConfiguration { get; private set; }

        public DefaultMvcContext(IMvcConfiguration mvcConfiguration)
        {
			MvcConfiguration = mvcConfiguration;
        }

        public ApplicationViewModel Application { get; set; }

        public MenuViewModel CreateMenu()
        {
            return new MenuViewModel(this);
        }

        public ObjectViewModel CreateObject()
        {
            return new ObjectViewModel(this);
        }

        public MemberViewModel CreateMember()
        {
            return new MemberViewModel(this);
        }

        public OperationViewModel CreateOperation()
        {
            return new OperationViewModel(this);
        }

        public ParameterViewModel CreateParameter()
        {
            return new ParameterViewModel(this);
        }

        public VariableViewModel CreateVariable()
        {
            return new VariableViewModel(this);
        }

		public PerformInterceptionContext CreatePerformInterceptionContext(ObjectViewModel target, string operationModelId, Dictionary<string, string> parameters)
		{
			return new PerformInterceptionContext(target, operationModelId, parameters);
		}

		public GetInterceptionContext CreateGetInterceptionContext(string id, string actualModelId)
		{
			return new GetInterceptionContext(id, actualModelId);
		}

		public GetAsInterceptionContext CreateGetAsInterceptionContext(string id, string actualModelId, string viewModelId)
		{
			return new GetAsInterceptionContext(id, actualModelId, viewModelId);
		}
	}
}
