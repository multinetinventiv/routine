using System;
namespace Routine.Test.Module.Todo.Api
{
	public interface ITodoModule
	{
		IAssignee GetAssignee(Guid assigneeUid);
	}
}
