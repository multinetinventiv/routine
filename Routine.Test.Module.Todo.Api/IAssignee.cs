using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Routine.Test.Module.Todo.Api
{
	public interface IAssignee
	{
		Guid Uid { get; }
		string Name { get; }

		List<ITodoItem> ItemsToBeDone { get; }
	}
}
