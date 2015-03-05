using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace Routine.Ui
{
	public class EmbeddedResourceVirtualPathProvider : VirtualPathProvider
	{
		private readonly IMvcConfiguration configuration;
		private readonly VirtualPathProvider previous;
		private readonly Dictionary<string, EmbeddedResource> resources;
		
		public EmbeddedResourceVirtualPathProvider(IMvcConfiguration configuration, VirtualPathProvider previous)
		{
			this.configuration = configuration;
			this.previous = previous;

			var themeAssembly = configuration.GetThemeAssembly();
			var themeNamespace = configuration.GetThemeNamespace();

			resources = themeAssembly
				.GetManifestResourceNames()
				.Where(r => r.StartsWith(themeNamespace))
				.ToDictionary(r => r.After(themeNamespace), r => new EmbeddedResource(themeAssembly, r));

			foreach (var uiAssembly in configuration.GetUiAssemblies())
			{
				resources = resources
					.Union(uiAssembly
						.GetManifestResourceNames()
						.ToDictionary(r => r.After(uiAssembly.GetName().Name), r => new EmbeddedResource(uiAssembly, r)))
					.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
			}
		}

		private string ResourceName(string virtualPath)
		{
			return VirtualPathUtility.ToAppRelative(virtualPath).After("~").Replace('/', '.');
		}

		private bool ResourcesContains(string virtualPath)
		{
			return resources.ContainsKey(ResourceName(virtualPath));
		}

		private EmbeddedResource GetResource(string virtualPath)
		{
			return resources[ResourceName(virtualPath)];
		}

		public override bool FileExists(string virtualPath)
		{
			return ResourcesContains(virtualPath) || previous.FileExists(virtualPath);
		}

		public override VirtualFile GetFile(string virtualPath)
		{
			if (!ResourcesContains(virtualPath))
			{
				return previous.GetFile(virtualPath);
			}

			return GetResource(virtualPath).CreateFile(virtualPath, configuration.GetCachePolicyAction(virtualPath));
		}

		public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
		{
			if (!ResourcesContains(virtualPath))
			{
				return previous.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
			}

			return GetResource(virtualPath).GetCacheDependency();
		}
	}
}