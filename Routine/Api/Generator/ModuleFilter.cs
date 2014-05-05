using System.Collections.Generic;

namespace Routine.Api.Generator
{
	public class ModuleFilter
	{
		private readonly List<string> includeFilters;
		private readonly List<string> excludeFilters;

		public ModuleFilter()
		{
			includeFilters = new List<string>();
			excludeFilters = new List<string>();
		}

		public void Include(string includeFilter)
		{
			includeFilters.Add(includeFilter);
		}

		public void Exclude(string excludeFilter)
		{
			excludeFilters.Add(excludeFilter);
		}

		public bool IsModuleIncluded(string moduleName)
		{
			bool wasIncluded = includeFilters.Count == 0;
			foreach(var includeFilter in includeFilters)
			{
				if(moduleName.Matches(includeFilter))
				{
					wasIncluded = true;
					break;
				}
			}

			if(!wasIncluded){return false;}

			foreach(var excludeFilter in excludeFilters)
			{
				if(moduleName.Matches(excludeFilter))
				{
					return false;
				}
			}

			return true;
		}

		public void Merge(ModuleFilter moduleFilter)
		{
			includeFilters.AddRange(moduleFilter.includeFilters);
			excludeFilters.AddRange(moduleFilter.excludeFilters);
		}
	}
}

