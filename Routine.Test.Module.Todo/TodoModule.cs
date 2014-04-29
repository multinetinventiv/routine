using System;
using System.Collections.Generic;
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

		public List<TodoItem> UndoneItems { get { return ctx.Query<TodoItems>().UndoneItems(); } }
		public List<Assignee> Assignees { get { return ctx.Query<Assignees>().All(); } }
		public int DoneItemCount { get { return ctx.Query<TodoItems>().DoneItems().Count; } }

		public void NewAssignee(string name, FatString address)
		{
			ctx.New<Assignee>().With(name, address);
		}

		public void NewTodo(string todo, DateTime dueDate)
		{
			ctx.New<TodoItem>().With(todo, dueDate);
		}

		public void NewTodoList(string todo, int count, DateTime startDate, int periodInDays)
		{
			for(int i=0; i<count; i++)
			{
				NewTodo((i+1) + ". " + todo, startDate.AddDays(i*periodInDays) );
			}
		}

		public void NewNamedTodoList(List<string> todos, DateTime startDate, int periodInDays)
		{
			for (int i = 0; i < todos.Count; i++)
			{
				NewTodo(todos[i], startDate.AddDays(i * periodInDays));
			}
		}

		public bool CanPurgeDoneItems () { return DoneItemCount > 0; }
		public void PurgeDoneItems() 
		{
			foreach (var item in ctx.Query<TodoItems>().DoneItems())
			{
				item.Delete();
			}
		}

		public void CreateTestData()
		{
			NewTodoList("Toplanti", 10, DateTime.Now, 7);
			NewAssignee("Cihan Deniz", "Suadiye");
			NewAssignee("Zafer Tokcanli", "Maslak");
			NewAssignee("Caglayan Yildirim", "Maltepe");

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

