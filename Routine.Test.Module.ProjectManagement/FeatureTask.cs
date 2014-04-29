﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Routine.Test.Common;
using Routine.Test.Domain;
using Routine.Test.Module.ProjectManagement.Api;

namespace Routine.Test.Module.ProjectManagement
{
	public class FeatureTask
	{
		private IRepository<FeatureTask> repository;
		private IDomainContext ctx;

		private FeatureTask() { }
		public FeatureTask(IRepository<FeatureTask> repository, IDomainContext ctx)
		{
			this.repository = repository;
			this.ctx = ctx;
		}

		public Guid Uid { get; private set; }
		public Guid FeatureUid { get; private set; }
		public TypedGuid TaskRef { get; private set; }

		internal FeatureTask With(Feature feature, ITask task)
		{
			FeatureUid = feature.Uid;
			TaskRef = new TypedGuid(task.Uid, task.GetType());

			Uid = repository.Insert<Guid>(this);

			return this;
		}

		public Feature Feature { get { return ctx.Query<Features>().ByUid(FeatureUid); } }
		public ITask Task { get { return (ITask)ctx.Find(TaskRef); } }
	}

	public class FeatureTasks : Query<FeatureTask>
	{
		public FeatureTasks(IDomainContext ctx) : base(ctx) { }

		public List<FeatureTask> ByFeature(Feature feature)
		{
			return By(f => f.FeatureUid == feature.Uid);
		}
	}
}
