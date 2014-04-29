using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Helpers;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using NHibernate;
using NHibernate.Cache;
using NHibernate.Driver;
using NHibernate.Tool.hbm2ddl;
using NHibernate.UserTypes;
using Routine.Core.Reflection;
using Routine.Test.Domain;
using Routine.Test.Domain.NHibernate.UserType;

namespace Routine.Test.Domain.Configuration
{
	internal class OrmConfiguration : DefaultAutomappingConfiguration
	{
		static OrmConfiguration()
		{
			Instance = new OrmConfiguration();
		}

		public static OrmConfiguration Instance{ get; private set;}

		public ISessionFactory BuildSessionFactory(IDomainContext domainContext, IEnumerable<System.Reflection.Assembly> assemblies)
		{
			var result = Fluently.Configure()
					.Database(SQLiteConfiguration.Standard
						.UsingCrossPlatformDriver()
						.UsingFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "routine.test.db"))
						.ShowSql())
					.Cache(c => c.UseQueryCache().UseSecondLevelCache().ProviderClass<HashtableCacheProvider>())
					.Mappings(m => 
						m.AutoMappings.Add(
							AutoMap.Assemblies(this, assemblies)
							.Conventions.Add(ConventionBuilder.Id.Always(x => x.GeneratedBy.Guid()))
							.Conventions.Add(ConventionBuilder.Id.Always(x => x.Unique()))
							.Conventions.Add(ConventionBuilder.Id.Always(x => x.Column("p_" + x.Name)))
							.Conventions.Add(DefaultLazy.Never())
							.Conventions.Add(ConventionBuilder.Property.Always(x => x.Column("c_" + x.Name)))
							.Conventions.Add(ConventionBuilder.Class.Always(x => x.Table("t_" + x.EntityType.Name)))
							.Conventions.Add(new CustomUserTypeConvention())
							.Conventions.Add(ConventionBuilder.Class.When(x => x.Expect(c => c.EntityType.ToTypeInfo().Has<CachedAttribute>()), 
																		  c => c.Cache.NonStrictReadWrite()))
						)
					)
					.ExposeConfiguration(c => c.SetInterceptor(new NHibernateIDomainContextInterceptor(domainContext)))
				;

			return result
					.ExposeConfiguration(c => new SchemaUpdate(c).Execute(true, true))
					.BuildSessionFactory();
		}

		public bool ShouldMap(TypeInfo type)
		{
			return ShouldMap(type.GetActualType());
		}

		public override bool ShouldMap(Type type)
		{
			return type.GetConstructors()
					.Any(c => c.IsPublic && 
						c.GetParameters().Any(p => typeof(IRepository<>).MakeGenericType(type).IsAssignableFrom(p.ParameterType)));
		}

		public override bool IsId(Member member)
		{
			return 
				member.IsAutoProperty && 
				member.PropertyType == typeof(Guid) &&
				member.MemberInfo.Name == "Uid";
		}

		public bool IsId(PropertyInfo propertyInfo)
		{
			return IsId(propertyInfo.GetActualProperty().ToMember());
		}

		public override bool ShouldMap(Member member)
		{
			return member.IsAutoProperty;
		}

		private class NHibernateIDomainContextInterceptor : EmptyInterceptor
		{
			private readonly IDomainContext domainContext;
			public NHibernateIDomainContextInterceptor(IDomainContext domainContext) { this.domainContext = domainContext; }

			private ISession session;
			public override void SetSession(ISession session) { this.session = session; }

			public override object Instantiate(string clazz, EntityMode entityMode, object id)
			{
				var metaData = session.SessionFactory.GetClassMetadata(clazz);

				var instance = domainContext.Resolve(metaData.GetMappedClass(EntityMode.Poco));

				metaData.SetIdentifier(instance, id, entityMode);

				return instance;
			}
		}

		//TODO refactor duplication
		private class CustomUserTypeConvention : IUserTypeConvention
		{
			public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
			{
				criteria.Expect(x =>
				{
					var type = x.Type.GetUnderlyingSystemType();
					var userType = GetType().Assembly.GetTypes().SingleOrDefault(t => t.Name == type.Name + "UserType");

					return
						(userType != null &&
						(typeof(IUserType).IsAssignableFrom(userType) || (typeof(ICompositeUserType).IsAssignableFrom(userType)))) ||
						(type.Assembly.FullName.StartsWith(GetType().Assembly.FullName.Before(".")) &&
						TypeInfo.Get(type).CanParse());
				});
			}

			public void Apply(IPropertyInstance instance)
			{
				var type = instance.Type.GetUnderlyingSystemType();
				var userType = GetType().Assembly.GetTypes().SingleOrDefault(t => t.Name == type.Name + "UserType");
				if (userType != null &&
					(typeof(IUserType).IsAssignableFrom(userType) || (typeof(ICompositeUserType).IsAssignableFrom(userType))))
				{
					instance.CustomType(userType);
				}
				else if (type.Assembly.FullName.StartsWith(GetType().Assembly.FullName.Before(".")) &&
					TypeInfo.Get(type).CanParse())
				{
					instance.CustomType(typeof(ParseableValueUserType<>).MakeGenericType(type));
				}
			}
		}
	}

	internal static class SQLiteConfigurationExtension
	{
		public static SQLiteConfiguration UsingCrossPlatformDriver(this SQLiteConfiguration source)
		{
			if (IsOnMono)
			{
				return source.Driver<MonoSqliteDriver>();
			}

			return source;
		}

		private static bool IsOnMono { get { return Type.GetType("Mono.Runtime") != null; } }

		private class MonoSqliteDriver : ReflectionBasedDriver
		{
			public MonoSqliteDriver()
				: base("Mono.Data.Sqlite",
					"Mono.Data.Sqlite",
					"Mono.Data.Sqlite.SqliteConnection",
					"Mono.Data.Sqlite.SqliteCommand") { }

			public override bool UseNamedPrefixInParameter { get { return true; } }
			public override bool UseNamedPrefixInSql { get { return true; } }
			public override string NamedPrefix { get { return "@"; } }
			public override bool SupportsMultipleOpenReaders { get { return false; } }
		}
	}
}

