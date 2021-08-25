using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Routine.Engine;
using Routine.Engine.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Routine.Samples.Basic
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRoutineDependencies();

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
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseRoutineInDevelopmentMode();
            }

            app.UseRoutine(
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
            );
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
        public object[] GetCustomAttributes() => Array.Empty<object>();

        public List<IParameter> Parameters => Type.Properties.Where(p => p.IsPublic).Select((p, ix) => new PropertyParameter(this, p, ix)).ToList<IParameter>();

        public bool IsPublic => true;
        public IType InitializedType => Type;

        public object Initialize(params object[] parameters)
        {
            var result = Type.CreateInstance();
            var properties = Parameters.Cast<PropertyParameter>().Select(p => p.Property).Cast<PropertyInfo>().ToList();

            for (var i = 0; i < parameters.Length; i++)
            {
                properties[i].SetValue(result, parameters[i]);
            }

            return result;
        }
    }
}
