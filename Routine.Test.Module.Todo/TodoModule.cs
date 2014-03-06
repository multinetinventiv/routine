using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Test.Common.Domain;

namespace Routine.Test.Module.Todo
{
	public class TodoModule
	{
		private readonly IDomainContext ctx;

		public TodoModule(IDomainContext ctx)
		{
			this.ctx = ctx;
		}

		public List<TodoItem> UndoneItems { get { return ctx.Get<TodoItemSearch>().UndoneItems(); } }
		public List<Assignee> Assignees{get{return ctx.Get<AssigneeSearch>().All();}}
		public int DoneItemCount { get { return ctx.Get<TodoItemSearch>().DoneItems().Count; } }

		public void NewAssignee(string name)
		{
			ctx.New<Assignee>().With(name);
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

		public bool CanPurgeDoneItems () { return DoneItemCount > 0; }
		public void PurgeDoneItems() 
		{
			foreach(var item in ctx.Get<TodoItemSearch>().DoneItems())
			{
				item.Delete();
			}
		}

		public void CreateTestData()
		{
			NewTodoList("Toplanti", 10, DateTime.Now, 7);
			NewAssignee("Cihan Deniz");
			NewAssignee("Zafer Tokcanli");
			NewAssignee("Caglayan Yildirim");

			UndoneItems[0].Assign(Assignees[0]);
			UndoneItems[1].Assign(Assignees[1]);
			UndoneItems[2].Assign(Assignees[2]);

			Assignees[0].AddEmail("is", "cdeniz@multinet.com.tr");
			Assignees[0].AddEmail("ev", "cih.deniz@gmail.com");
			Assignees[0].AddPhone("cep", "532", "2049860");
		}	
	}
}

