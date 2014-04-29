using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Test.Domain;
using Routine.Test.Module.ProjectManagement.Api;
using Routine.Test.Module.Todo.Api;

namespace Routine.Test.Module.Todo
{
	public class TodoItem : ITodoItem, ITask
	{
		#region Construction
		private readonly IDomainContext ctx;
		private readonly IRepository<TodoItem> repository;

		private TodoItem(){}
		public TodoItem (IDomainContext ctx, IRepository<TodoItem> repository)
		{
			this.ctx = ctx;
			this.repository = repository;
		}

		internal TodoItem With(string text, DateTime dueDate)
		{
			Text = text;
			DueDate = dueDate;
			Priority = TodoItemPriority.Normal;

			Uid = repository.Insert<Guid>(this);

			return this;
		}
		#endregion

		#region Data
		public Guid Uid{get; private set;}
		public string Text { get; private set; }
		public bool Done { get; private set; }
		public DateTime DueDate {get; private set;}
		public Guid AssigneeUid {get; private set;}
		public TodoItemPriority Priority {get;private set;}
		#endregion

		public Assignee AssignedTo { get { return ctx.Query<Assignees>().ByUid(AssigneeUid); } }
		public bool Passed {get{return DueDate <= DateTime.Now;}}
		public bool Active{get{return !Passed && !Done;}}

		public bool CanMarkAsDone() { return !Done; }
		public void MarkAsDone() 
		{
			Done = true;

			repository.Update(this);
		}

		public void MarkAs(bool done)
		{
			Done = done;

			repository.Update(this);
		}

		public bool CanAssign() { return AssigneeUid == default(Guid);}
		public void Assign(Assignee to)
		{
			AssigneeUid = to.Uid;

			repository.Update(this);
		}

		public void BindToFeature(IFeature feature)
		{
			feature.AddTask(this);
		}

		public void Release()
		{
			AssigneeUid = default(Guid);
		}

		public bool CanBringForward(){return DueDate > DateTime.Now;}
		public void BringForward(int days)
		{
			DueDate = DueDate.AddDays(-days);
			if(DueDate < DateTime.Now)
			{
				DueDate = DateTime.Now;
			}

			repository.Update(this);
		}

		public void Postpone(int days)
		{
			DueDate = DueDate.AddDays(days);

			repository.Update(this);
		}

		public void PostponeOneWeek()
		{
			Postpone(7);
		}

		public void Update(string text)
		{
			Text = text;

			repository.Update(this);
		}

		public void UpdatePriority(TodoItemPriority priority)
		{
			Priority = priority;

			repository.Update(this);
		}

		internal void Delete()
		{
			repository.Delete(this);
		}

		public override string ToString()
		{
			return Text + " (in " + (int)DueDate.Subtract(DateTime.Now).TotalDays + " days)";
		}

		#region ITask implementation

		string ITask.State { get { return Done ? "Done" : "Todo"; } }

		#endregion
	}

	public class TodoItems : Query<TodoItem>
	{
		public TodoItems(IDomainContext context)
			: base(context) {}

		public List<TodoItem> DoneItems() 
		{ 
			return By(i => i.Done)
					.OrderBy(i => i.DueDate)
					.ToList();
		}

		public List<TodoItem> UndoneItems()
		{
			return By(i => !i.Done)
					.OrderBy(i => i.DueDate)
					.ToList();
		}

		internal List<TodoItem> ByAssigneeUidAndDone(Guid assigneeUid, bool done)
		{
			return By(i => i.AssigneeUid == assigneeUid && i.Done == done);
		}
	}
}

