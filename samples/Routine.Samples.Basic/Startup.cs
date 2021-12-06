using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;

namespace Routine.Samples.Basic
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRoutine();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseRoutineInDevelopmentMode();
            }

            app.UseRoutine(
                codingStyle: cs => cs.FromBasic()
                    .AddTypes(typeof(Startup).Assembly, t => t.IsPublic)
                    .Module.Set(c => c.By(t => t.Namespace.After("Routine.Samples.Basic.")))

                    //Service Configuration
                    .ValueExtractor.Set(c => c.Value(v => v.By(obj => obj.GetType().Name.SplitCamelCase(' '))).When(t => t.Name.EndsWith("Service")))
                    .Locator.Set(c => c.Locator(l => l.Singleton(t => t.CreateInstance())).When(t => t.Name.EndsWith("Service")))
                    .StaticInstances.Add(c => c.By(t => t.CreateInstance()).When(t => t.Name.EndsWith("Service")))
                    .Operations.Add(c => c.PublicMethods(m => !m.IsInherited(true, true)).When(t => t.Name.EndsWith("Service")))

                    //Dto Configuration
                    .Initializers.Add(c => c.PublicConstructors().When(t => t.Name.EndsWith("Dto")))
                    .Datas.Add(c => c.PublicProperties().When(t => t.Name.EndsWith("Dto")))
                    .IdExtractor.Set(c => c.Id(id => id.Constant("Dto")).When(t => t.Name.EndsWith("Dto")))
                    .ValueExtractor.Set(c => c.ValueByPublicProperty(p => p.Returns<string>()).When(t => t.Name.EndsWith("Dto"))),

                interceptionConfiguration: ic => ic.FromBasic()
                    .ServiceInterceptors.Add(c => c.Interceptor(i => i
                        .ByDecoratingAsync(Stopwatch.StartNew)
                        .After(sw => Console.WriteLine($@"End in ""{sw.Elapsed}"""))
                    ))
                    .ServiceInterceptors.Add(c => c.Interceptor(i => i
                        .Before(ctx => Console.WriteLine($@"Executing ""{ctx.OperationName}"""))
                        .Success(() => Console.WriteLine("\tsuccess!"))
                        .Fail(() => Console.WriteLine("\tfail!"))
                    ))
            );
        }
    }
}
