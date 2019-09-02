using System;
using System.Linq;
using Routine.Engine;

namespace Routine.Samples.SmartObject
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            BuildRoutine.Context().AsServiceApplication(
                serviceConfiguration: sc => sc.FromBasic()
                    .RootPath.Set("api"),
                codingStyle: cs => cs.FromBasic()
                    .AddTypes(typeof(Global).Assembly, t => t.IsPublic)
                    .Module.Set(c => c.By(t => t.Namespace.After("Routine.Samples.SmartObject.")))

                    .Datas.Add(c => c.PublicProperties())
                    .Operations.Add(c => c.PublicMethods(m => !m.IsInherited(true, true)))
                    .IdExtractor.Set(c => c.IdByPublicProperty(p => p.Returns<string>()))
                    .ValueExtractor.Set(c => c.ValueByPublicProperty(p => p.Returns<string>()))
                    .Locator.Set(c => c.Locator(l => l.SingleBy(FindSmartObject)).When(t => t.Constructors.Any(ctor => ctor.Parameters.Any())))
                    .Locator.Set(c => c.Locator(l => l.Singleton(t => t.CreateInstance())))
                    .Override(c => c.Operations.AddNoneWhen(t => t.IsValueType))
            );
        }

        private object FindSmartObject(IType type, string name)
        {
            var repoTypeName = type.Name + "s";
            var ti = (TypeInfo)type;

            var queryType = ti.GetActualType().Assembly.GetType($"{ti.FullName.BeforeLast(type.Name)}{repoTypeName}");
            var methodInfo = queryType.GetMethod("Find");
            if (methodInfo == null)
            {
                throw new MissingMethodException(queryType.Name, "Find");
            }

            return methodInfo.Invoke(Activator.CreateInstance(queryType), new object[] { name });
        }
    }
}