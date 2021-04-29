using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Routine.Core.Rest;
using Routine.Engine;
using Routine.Engine.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace Routine.Samples.Basic
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<IJsonSerializer, JavaScriptSerializerAdapter>();

            services.AddControllers();
            services.AddRouting();

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
                    .RootPath.Set("api")
                    .RequestHeaders.Add("Accept-Language"),
                codingStyle: cs => cs.FromBasic()
                    .AddTypes(typeof(Startup).Assembly, t => t.IsPublic)
                    .Module.Set(c => c.By(t => t.Namespace.After("Routine.Samples.Basic.")))

                    //Service Configuration
                    .ValueExtractor.Set(c => c.Value(v => v.By(obj => obj.GetType().Name.SplitCamelCase(' '))).When(t => t.Name.EndsWith("Service")))
                    .Locator.Set(c => c.Locator(l => l.Singleton(t => t.CreateInstance())).When(t => t.Name.EndsWith("Service")))
                    .StaticInstances.Add(c => c.By(t => t.CreateInstance()).When(t => t.Name.EndsWith("Service")))
                    .Operations.Add(c => c.PublicMethods(m => !m.IsInherited(true, true)).When(t => t.Name.EndsWith("Service")))

                    //Dto Configuration
                    .Initializers.Add(c => c.By(t => new PublicPropertyConstructor(t)).When(t => t.Name.EndsWith("Dto")))
                    .Datas.Add(c => c.PublicProperties().When(t => t.Name.EndsWith("Dto")))
                    .IdExtractor.Set(c => c.Id(id => id.Constant("Dto")).When(t => t.Name.EndsWith("Dto")))
                    .ValueExtractor.Set(c => c.ValueByPublicProperty(p => p.Returns<string>()).When(t => t.Name.EndsWith("Dto")))
            ));

        }
    }

    internal class PropertyParameter : IParameter
    {
        public IProperty Property { get; }
        public IParametric Owner { get; }
        public int Index { get; }

        public PropertyParameter(IParametric owner, IProperty property, int index)
        {
            Owner = owner;
            Property = property;
            Index = index;
        }

        public string Name => Property.Name.ToLowerInitial();
        public IType ParentType => Property.ParentType;
        public object[] GetCustomAttributes() => Property.GetCustomAttributes();
        public IType ParameterType => Property.ReturnType;
    }

    internal class PublicPropertyConstructor : IConstructor
    {
        public IType Type { get; }

        public PublicPropertyConstructor(IType type)
        {
            Type = type;
        }

        public string Name => "_ctor";
        public IType ParentType => Type;
        public object[] GetCustomAttributes() => new object[0];

        public List<IParameter> Parameters => Type.Properties.Where(p => p.IsPublic).Select((p, ix) => new PropertyParameter(this, p, ix)).ToList<IParameter>();

        public bool IsPublic => true;
        public IType InitializedType => Type;

        public object Initialize(params object[] parameters)
        {
            var result = Type.CreateInstance();
            var properties = Parameters.Cast<PropertyParameter>().Select(p => p.Property).Cast<PropertyInfo>().ToList();

            for (int i = 0; i < parameters.Length; i++)
            {
                properties[i].SetValue(result, parameters[i]);
            }

            return result;
        }
    }
}
