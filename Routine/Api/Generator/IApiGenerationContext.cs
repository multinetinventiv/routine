using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Routine.Api.Generator
{
	public interface IApiGenerationContext
	{
		IApiGenerationConfiguration ApiGenerationConfiguration { get; }

		ApplicationCodeModel Application { get; }

		ObjectCodeModel CreateObject();
		MemberCodeModel CreateMember();
		OperationCodeModel CreateOperation();
		ParameterCodeModel CreateParameter();
	}
}
