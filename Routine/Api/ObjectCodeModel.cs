﻿using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Client;
using Routine.Engine;

namespace Routine.Api
{
	public class ObjectCodeModel
	{
		private static string BuildNamespace(string defaultNamespace, Rtype type)
		{
			var result = defaultNamespace;
			if (!string.IsNullOrEmpty(type.Module) && !result.EndsWith(type.Module))
			{
				result += "." + type.Module;
			}

			return result;
		}

		private readonly ApplicationCodeModel application;
		private readonly bool isVoid;
		private readonly bool isList;
		private readonly string @namespace;

		private readonly IType clientType;
		private readonly string stringToValueCodeTemplate;
		private readonly string valueToStringCodeTemplate;

		public Rtype Type { get; private set; }
		public InitializerCodeModel Initializer { get; private set; }
		public List<MemberCodeModel> Members { get; private set; }
		public List<OperationCodeModel> Operations { get; private set; }

		internal ObjectCodeModel(ApplicationCodeModel application, Rtype voidType)
			: this(application, true, false, voidType, null, type.ofvoid(), null, null) { }

		internal ObjectCodeModel(ApplicationCodeModel application, Rtype type, string defaultNamespace)
			: this(application, false, false, type, BuildNamespace(defaultNamespace, type), null, null, null) { }

		internal ObjectCodeModel(ApplicationCodeModel application, ObjectCodeModel model, bool isList)
			: this(application, false, isList, model.Type, model.@namespace, model.clientType, model.stringToValueCodeTemplate, model.valueToStringCodeTemplate) { }

		internal ObjectCodeModel(ApplicationCodeModel application, Rtype type, IType clientType)
			: this(application, false, false, type, clientType.Namespace, clientType, null, null) { }

		internal ObjectCodeModel(ApplicationCodeModel application, Rtype type, IType clientType, string stringToValueCodeTemplate, string valueToStringCodeTemplate)
			: this(application, false, false, type, clientType.Namespace, clientType, stringToValueCodeTemplate, valueToStringCodeTemplate) { }

		private ObjectCodeModel(ApplicationCodeModel application, bool isVoid, bool isList, Rtype type, string @namespace, IType clientType, string stringToValueCodeTemplate, string valueToStringCodeTemplate)
		{
			this.application = application;
			this.isVoid = isVoid;
			this.isList = isList;
			this.@namespace = @namespace;
			this.clientType = clientType;
			this.stringToValueCodeTemplate = stringToValueCodeTemplate;
			this.valueToStringCodeTemplate = valueToStringCodeTemplate;

			Type = type;
		}

		public bool IsVoid { get { return isVoid; } }
		public bool IsList { get { return isList; } }
		public string Id { get { return Type.Id; } }
		public string Namespace { get { return @namespace; } }
		public string Name { get { return Type.Name; } }
		public bool IsValueModel { get { return Type.IsValueType; } }
		public bool IsViewModel { get { return Type.IsViewType; } }
		public string Module { get { return Type.Module; } }
		public List<string> StaticInstanceIds { get { return application.GetStaticInstanceIds(this); } }

		public string FullName
		{
			get
			{
				if (!isList)
				{
					return FullNameIgnoringList;
				}

				var listType = typeof(List<>);

				return listType.Namespace + "." + listType.Name.Before("`") + "<" + FullNameIgnoringList + ">";
			}
		}

		public string FullNameIgnoringList
		{
			get
			{
				if (clientType != null)
				{
					if (IsVoid)
					{
						return "void";
					}

					return clientType.FullName;
				}

				if (string.IsNullOrEmpty(Namespace)) { return Name; }

				return Namespace + "." + Name;
			}
		}

		public bool CanInitialize { get { return Initializer != null; } }

		internal void Load()
		{
			if (Type.Initializer != null && application.IsRendered(Type.Initializer) && Type.Initializer.Parameters.All(p => application.ValidateType(p.ParameterType)))
			{
				Initializer = new InitializerCodeModel(application, Type.Initializer);
			}

			Members = Type.Members
				.Where(m => application.IsRendered(m) && application.ValidateType(m.MemberType))
				.Select(m => new MemberCodeModel(application, m))
				.ToList();

			Operations = Type.Operations
				.Where(o => application.IsRendered(o) && (o.ResultIsVoid || application.ValidateType(o.ResultType)) && o.Parameters.All(p => application.ValidateType(p.ParameterType)))
				.Select(o => new OperationCodeModel(application, o))
				.ToList();
		}

		public string GetStringToValueCode(string robjectVariableName)
		{
			if (!Type.IsValueType)
			{
				throw new InvalidOperationException("Only value models can have string to value conversion");
			}

			return stringToValueCodeTemplate
					.Replace("{valueString}", robjectVariableName + ".Value")
					.Replace("{valueRobject}", robjectVariableName)
					.Replace("{type}", clientType.FullName);
		}

		public string GetValueToStringCode(string objectVariableName)
		{
			if (!Type.IsValueType)
			{
				throw new InvalidOperationException("Only value models can have string to value conversion");
			}

			return valueToStringCodeTemplate
					.Replace("{value}", objectVariableName);
		}

		public bool MarkedAs(string mark)
		{
			return Type.MarkedAs(mark);
		}

		internal ObjectCodeModel GetListType()
		{
			return new ObjectCodeModel(application, this, true);
		}
	}
}
