using System.Collections.Generic;

namespace Routine.Test.Module.ProjectManagement.Api
{
	public interface IProject
	{
		string Name{ get; }
	}

	public interface IProjects
	{
		List<IProject> All();
	}
}

