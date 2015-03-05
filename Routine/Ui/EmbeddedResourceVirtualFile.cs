using System;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Hosting;

namespace Routine.Ui
{
	public class EmbeddedResourceVirtualFile : VirtualFile
	{
		private readonly Assembly assembly;
		private readonly string resourceName;
		private readonly Action<HttpCachePolicy> cachePolicyAction;

		public EmbeddedResourceVirtualFile(string virtualPath, Assembly assembly, string resourceName, Action<HttpCachePolicy> cachePolicyAction)
			: base(virtualPath)
		{
			this.assembly = assembly;
			this.resourceName = resourceName;
			this.cachePolicyAction = cachePolicyAction;
		}

		public override Stream Open()
		{
			cachePolicyAction(HttpContext.Current.Response.Cache);

			return assembly.GetManifestResourceStream(resourceName) ?? Stream.Null;
		}
	}
}