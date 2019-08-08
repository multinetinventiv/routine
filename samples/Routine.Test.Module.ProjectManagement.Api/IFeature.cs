using System.Collections.Generic;

namespace Routine.Test.Module.ProjectManagement.Api
{
	public interface IFeature
	{
		void AddTask(ITask task);
	}
	public interface IFeatures
	{
		List<IFeature> ByProject(IProject project);
	}
}
