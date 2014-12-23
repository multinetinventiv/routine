using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Test.Common;
using Routine.Test.Domain;
using Routine.Test.Module.Todo.Api;

namespace Routine.Test.Module.Todo
{
	public class TodoModule : ITodoModule
	{
		private readonly IDomainContext ctx;

		public TodoModule(IDomainContext ctx)
		{
			this.ctx = ctx;
		}

		public int DoneItemCount { get { return ctx.Query<TodoItems>().DoneItems().Count; } }
		public List<Assignee> Assignees { get { return ctx.Query<Assignees>().All(); } }
		public List<TodoItem> UndoneItems { get { return ctx.Query<TodoItems>().UndoneItems(); } }

		public void NewAssignee(string name, FatString address)
		{
			ctx.New<Assignee>().With(name, address);
		}

		public void NewTodoList(DateTime firstDueDate, int periodInDays, string todoSuffix, int count)
		{
			NewTodoList(firstDueDate, periodInDays, Enumerable.Range(1, count).Select(i => i + ". " + todoSuffix).ToList());
		}

		public void NewTodoList(DateTime firstDueDate, int periodInDays, List<string> todos)
		{
			for (int i = 0; i < todos.Count; i++)
			{
				NewTodo(todos[i], firstDueDate.AddDays(i * periodInDays));
			}
		}

		public void NewTodo(string todo, DateTime dueDate)
		{
			ctx.New<TodoItem>().With(todo, dueDate);
		}

		public void PurgeDoneItems() 
		{
			foreach (var item in ctx.Query<TodoItems>().DoneItems())
			{
				item.Delete();
			}
		}

		public void CreateTestData()
		{
			NewTodoList(DateTime.Now, 7, "Toplanti", 10);
			NewAssignee("Cihan Deniz", new FatString("Suadiye"));
			NewAssignee("Zafer Tokcanli", new FatString("Maslak"));
			NewAssignee("Caglayan Yildirim", new FatString("Maltepe"));

			UndoneItems[0].Assign(Assignees[0]);
			UndoneItems[1].Assign(Assignees[1]);
			UndoneItems[2].Assign(Assignees[2]);

			Assignees[0].AddEmail("is", "cdeniz@multinet.com.tr");
			Assignees[0].AddEmail("ev", "cih.deniz@gmail.com");
			Assignees[0].AddPhone("cep", "532", "2049860");
		}

		#region ITodoModule implementation
		IAssignee ITodoModule.GetAssignee(Guid assigneeUid)
		{
			return ctx.Query<Assignees>().ByUid(assigneeUid);
		} 
		#endregion
	}
}

