using System;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;
using NHibernate.Driver;
using NHibernate.Tool.hbm2ddl;
using Routine.Core.Reflection;
using Routine.Test.Common.Domain;

namespace Routine.Test.Common.Configuration
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
			return Fluently.Configure()
					#if DEBUG
					.Database(SQLiteConfiguration.Standard
						.UsingCrossPlatformDriver()
						.UsingFile("routine.test.db")
						.ShowSql())
					#else
					//production connection
					#endif
					.Mappings(m => 
						m.AutoMappings.Add(
							AutoMap.Assemblies(this, assemblies)
							.Conventions.Add(ConventionBuilder.Id.Always(x => x.GeneratedBy.Guid()))
							.Conventions.Add(ConventionBuilder.Id.Always(x => x.Unique()))
							.Conventions.Add(ConventionBuilder.Id.Always(x => x.Column("p_" + x.Name)))
							.Conventions.Add(DefaultLazy.Never())
							.Conventions.Add(ConventionBuilder.Property.Always(x => x.Column("c_" + x.Name)))
							.Conventions.Add(ConventionBuilder.Class.Always(x => x.Table("t_" + x.EntityType.Name)))
						)
					)
					.ExposeConfiguration(c => c.SetInterceptor(new NHibernateIFactoryInterceptor(domainContext)))
					#if DEBUG
					.ExposeConfiguration(c => new SchemaUpdate(c).Execute(true, true))
					#endif
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
				(member.IsProperty || member.IsAutoProperty) && 
				(member.MemberInfo as System.Reflection.PropertyInfo).PropertyType == typeof(Guid) &&
				member.MemberInfo.Name == "Uid";
		}

		public bool IsId(PropertyInfo propertyInfo)
		{
			return IsId(propertyInfo.GetActualProperty().ToMember());
		}

		public override bool ShouldMap(Member member)
		{
			return (member.IsProperty || member.IsAutoProperty) && (member.MemberInfo as System.Reflection.PropertyInfo).CanWrite;
		}

		private class NHibernateIFactoryInterceptor : EmptyInterceptor
		{
			private readonly IDomainContext domainContext;
			public NHibernateIFactoryInterceptor(IDomainContext domainContext) { this.domainContext = domainContext; }

			private ISession session;
			public override void SetSession(ISession session) { this.session = session; }

			public override object Instantiate(string clazz, EntityMode entityMode, object id)
			{
				var metaData = session.SessionFactory.GetClassMetadata(clazz);

				var instance = domainContext.New(metaData.GetMappedClass(EntityMode.Poco));

				metaData.SetIdentifier(instance, id, entityMode);

				return instance;
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

