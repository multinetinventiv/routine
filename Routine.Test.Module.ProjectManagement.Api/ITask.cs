using System;

namespace Routine.Test.Module.ProjectManagement.Api
{
	public interface ITask
	{
		Guid Uid { get; }
		string Text { get; }
		string State { get; }
	}
}
