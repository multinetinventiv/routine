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

		List<IProject> AllByOther(IProjects projects);
	}

	public class EmptyProjects : IProjects
	{
		public List<IProject> All()
		{
			return new List<IProject>();
		}

		public List<IProject> AllByOther(IProjects projects)
		{
			return new List<IProject>();
		}
	}
}

