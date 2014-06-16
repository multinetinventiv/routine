using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Routine.Test.Module.Todo.Api
{
	public enum TodoItemPriority
	{
		Low,
		Normal,
		High,
	}

	public interface ITodoItem
	{
		Guid Uid { get; }
		string Text { get; }
		bool Done { get; }
		DateTime DueDate { get; }
		TodoItemPriority Priority { get; }
	}

	public interface ITodoItems
	{
		List<ITodoItem> All();
	}
}
