using System;
using System.Reflection;
using System.Web;
using System.Web.Caching;

namespace Routine.Ui
{
	public class EmbeddedResource
	{
		private readonly Assembly assembly;
		private readonly string resourceName;

		public EmbeddedResource(Assembly assembly, string resourceName)
		{
			this.assembly = assembly;
			this.resourceName = resourceName;
		}

		public CacheDependency GetCacheDependency()
		{
			return new CacheDependency(assembly.Location);
		}

		public EmbeddedResourceVirtualFile CreateFile(string virtualPath, Action<HttpCachePolicy> cachePolicyAction)
		{
			return new EmbeddedResourceVirtualFile(virtualPath, assembly, resourceName, cachePolicyAction);
		}
	}
}