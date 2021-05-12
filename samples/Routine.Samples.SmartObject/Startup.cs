using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Routine.Core.Rest;
using Routine.Engine;
using System;
using System.Linq;

namespace Routine.Samples.SmartObject
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<IJsonSerializer, JsonSerializerAdapter>();

            // If using Kestrel:
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            // If using IIS:
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseRoutine(httpContextAccessor, cb => cb.AsServiceApplication(
                serviceConfiguration: sc => sc.FromBasic()
                    .RootPath.Set("api"),
                codingStyle: cs => cs.FromBasic()
                    .AddTypes(typeof(Startup).Assembly, t => t.IsPublic)
                    .Module.Set(c => c.By(t => t.Namespace.After("Routine.Samples.SmartObject.")))

                    .Datas.Add(c => c.PublicProperties())
                    .Operations.Add(c => c.PublicMethods(m => !m.IsInherited(true, true)))
                    .IdExtractor.Set(c => c.IdByPublicProperty(p => p.Returns<string>()))
                    .ValueExtractor.Set(c => c.ValueByPublicProperty(p => p.Returns<string>()))
                    .Locator.Set(c => c.Locator(l => l.SingleBy(FindSmartObject)).When(t => t.Constructors.Any(ctor => ctor.Parameters.Any())))
                    .Locator.Set(c => c.Locator(l => l.Singleton(t => t.CreateInstance())))
                    .Override(c => c.Operations.AddNoneWhen(t => t.IsValueType))
                ));

            // app.UseEndpoints(endpoints =>
            // {
            //     endpoints.MapGet("/", async context =>
            //     {
            //         await context.Response.WriteAsync("Hello World!");
            //     });
            // });
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
