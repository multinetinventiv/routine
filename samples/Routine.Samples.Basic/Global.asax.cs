using System;

namespace Routine.Samples.Basic
{
	public class Global : System.Web.HttpApplication
	{

		protected void Application_Start(object sender, EventArgs e)
		{
			BuildRoutine.Context().AsServiceApplication(
				serviceConfiguration: sc => sc.FromBasic()
					.RootPath.Set("api")
					.RequestHeaders.Add("Accept-Language"),
				codingStyle: cs => cs.FromBasic()
					.AddTypes(typeof(Global).Assembly)
					.Module.Set(c => c.By(t => t.Namespace.After("Routine.Samples.Basic.")))

					//Service Configuration
					.ValueExtractor.Set(c => c.Value(v => v.By(obj => obj.GetType().Name.SplitCamelCase(' '))).When(t => t.Name.EndsWith("Service")))
					.Locator.Set(c => c.Locator(l => l.Singleton(t => t.CreateInstance())).When(t => t.Name.EndsWith("Service")))
					.StaticInstances.Add(c => c.By(t => t.CreateInstance()).When(t => t.Name.EndsWith("Service")))
					.Operations.Add(c => c.PublicMethods(m => !m.IsInherited(true, true)).When(t => t.Name.EndsWith("Service")))

					//Dto Configuration
					.Datas.Add(c => c.PublicProperties().When(t => t.Name.EndsWith("Dto")))
					.IdExtractor.Set(c => c.IdByPublicProperty(p => p.Returns<Guid>()).When(t => t.Name.EndsWith("Dto")))
					.ValueExtractor.Set(c => c.ValueByPublicProperty(p => p.Returns<string>()).When(t => t.Name.EndsWith("Dto")))
			);
		}
	}
}